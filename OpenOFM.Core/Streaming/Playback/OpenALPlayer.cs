using OpenOFM.Core.Streaming.Chunks;
using OpenTK.Audio.OpenAL;
using System.Collections.Concurrent;

namespace OpenOFM.Core.Streaming.Playback
{
    public class OpenALPlayer : IChunkSink, IDisposable
    {
        private readonly ConcurrentQueue<IChunk> _buffer = new();

        private const int NumBuffers = 4;
        private const int BufferSize = 192000;

        private readonly ALDevice _device;
        private readonly ALContext _context;

        private readonly int[] _buffers;
        private readonly int _source;

        private CancellationTokenSource? _cts;
        private Task? _playbackLoopTask;

        public OpenALPlayer()
        {
            //Initialize OpenAL
            _device = ALC.OpenDevice(null);
            _context = ALC.CreateContext(_device, new ALContextAttributes());
            ALC.MakeContextCurrent(_context);

            //Create buffers
            _buffers = new int[NumBuffers];
            AL.GenBuffers(_buffers);

            //Create source
            AL.GenSource(out _source);
            AL.Source(_source, ALSourcef.Gain, 1);
        }

        public bool IsPlaying
        {
            get => _playbackLoopTask?.IsCompleted == false &&
                AL.GetSource(_source, ALGetSourcei.SourceState) == (int)ALSourceState.Playing;
        }

        public bool IsPaused
        {
            get => AL.GetSource(_source, ALGetSourcei.SourceState) == (int)ALSourceState.Paused;
            set
            {
                if (value)
                {
                    AL.SourcePause(_source);
                }
                else
                {
                    AL.SourcePlay(_source);
                }
            }
        }

        public float Volume
        {
            get => AL.GetSource(_source, ALSourcef.Gain);
            set => AL.Source(_source, ALSourcef.Gain, value);
        }

        public void Play()
        {
            _cts = new CancellationTokenSource();
            _playbackLoopTask = Task.Run(async () =>
            {
                try
                {
                    await PlaybackLoop(_cts.Token);
                }
                catch
                {
                    AL.SourceStop(_source);
                    AL.SourceUnqueueBuffers(_source, _buffers);
                }
            });
        }

        public void Stop()
        {
            _cts?.Cancel();
            _playbackLoopTask?.Wait();
            _buffer.Clear();
        }

        public Task WriteChunkAsync(IChunk chunk, CancellationToken ct = default)
        {
            _buffer.Enqueue(chunk);

            if (AL.GetSource(_source, ALGetSourcei.SourceState) == (int)ALSourceState.Stopped)
            {
                AL.SourcePlay(_source);
            }

            return Task.CompletedTask;
        }

        private async Task PlaybackLoop(CancellationToken cancellationToken)
        {
            byte[] dataBuffer = new byte[BufferSize];

            //Initialize buffers
            for (int i = 0; i < NumBuffers;)
            {
                if (_buffer.TryDequeue(out var c) && c is DataChunk chunk)
                {
                    AL.BufferData(_buffers[i], ALFormat.Stereo16, chunk.Data, 48000);
                    AL.SourceQueueBuffer(_source, _buffers[i]);
                    i++;
                }
            }

            AL.SourcePlay(_source);

            //Update buffers
            while (!cancellationToken.IsCancellationRequested)
            {
                while (AL.GetSource(_source, ALGetSourcei.BuffersProcessed) > 0)
                {
                    if (_buffer.TryDequeue(out var c) && c is DataChunk chunk)
                    {
                        int alBuffer = AL.SourceUnqueueBuffer(_source);

                        AL.BufferData(alBuffer, ALFormat.Stereo16, chunk.Data, 48000);
                        AL.SourceQueueBuffer(_source, alBuffer);
                    }
                }

                await Task.Delay(100);
            }

            AL.SourceStop(_source);
            AL.SourceUnqueueBuffers(_source, _buffers);
        }

        public void Dispose()
        {
            Stop();

            //OpenAL cleanup
            AL.DeleteSource(_source);
            AL.DeleteBuffers(_buffers);
            ALC.CloseDevice(_device);
            ALC.DestroyContext(_context);
        }
    }
}
