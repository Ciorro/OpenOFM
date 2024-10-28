using OpenOFM.Core.Streaming.Chunks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OpenOFM.Core.Streaming.Middlewares
{
    public class FFmpegDecoderMiddleware : IChunkMiddleware, IDisposable
    {
        private readonly ConcurrentQueue<IChunk> _inBuffer = new();
        private readonly ConcurrentQueue<IChunk> _outBuffer = new();

        private readonly CancellationTokenSource _cts;
        private readonly Task _inputReaderTask;
        private readonly Task _outputReaderTask;
        private readonly Process _ffmpeg;
        private readonly Stream _inStream;
        private readonly Stream _outStream;

        public FFmpegDecoderMiddleware(string ffmpegPath)
        {
            _ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = "-hide_banner -loglevel error -i - -f s16le -",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            })!;

            _cts = new CancellationTokenSource();

            _inStream = _ffmpeg.StandardInput.BaseStream;
            _outStream = _ffmpeg.StandardOutput.BaseStream;

            _inputReaderTask = Task.Run(async () => await FFmpegWriteLoop(_cts.Token));
            _outputReaderTask = Task.Run(async () => await FFmpegReadLoop(_cts.Token));
        }

        public TimeSpan BufferedDuration
        {
            get => _inBuffer.Aggregate(TimeSpan.Zero, (ts, ch) => ts.Add(ch.Duration)) +
                   _outBuffer.Aggregate(TimeSpan.Zero, (ts, ch) => ts.Add(ch.Duration));
        }

        public Task WriteChunkAsync(IChunk chunk, CancellationToken ct = default)
        {
            if (chunk is not DataChunk dataChunk)
            {
                throw new ArgumentException(
                    $"Invalid input chunk type. Expected {typeof(DataChunk)}", nameof(chunk));
            }

            _inBuffer.Enqueue(chunk);
            return Task.CompletedTask;
        }

        public Task<IChunk?> ReadChunkAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_outBuffer.TryDequeue(out var chunk) ? chunk : null);
        }

        private async Task FFmpegWriteLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_inBuffer.TryDequeue(out var c) && c is DataChunk chunk)
                {
                    await _inStream.WriteAsync(chunk.Data, ct);
                }

                await Task.Delay(100, ct);
            }
        }

        private async Task FFmpegReadLoop(CancellationToken ct)
        {
            byte[] buffer = new byte[192000];
            int sequence = 0;

            while (!ct.IsCancellationRequested)
            {
                int read = 0;
                while (read < 192000)
                {
                    int toRead = Math.Min(192000 - read, 4096);
                    read += await _outStream.ReadAsync(buffer, read, toRead, ct);
                }

                byte[] bufferCopy = new byte[192000];
                Array.Copy(buffer, bufferCopy, 192000);

                _outBuffer.Enqueue(
                    new DataChunk(sequence++, TimeSpan.FromSeconds(1), bufferCopy));

                await Task.Delay(100, ct);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();

            _ffmpeg.Kill();
            _ffmpeg.Dispose();

            Task.WaitAll(
                _inputReaderTask.WaitAsync(CancellationToken.None),
                _outputReaderTask.WaitAsync(CancellationToken.None)
            );
        }
    }
}
