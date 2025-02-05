using OpenOFM.Core.Streaming.M3U;
using OpenOFM.Core.Streaming.Middlewares;

namespace OpenOFM.Core.Streaming.Playback
{
    public class Player : IDisposable
    {
        private readonly StreamingPipeline _pipeline;
        private readonly TimeSpan _maxDelay;

        private CancellationTokenSource? _cts;
        private Task? _processingLoopTask;

        private TimeSpan _delay;
        private DateTime _pauseStartedAt;

        public Player(Uri streamUrl, HttpClient httpClient, TimeSpan maxDelay)
        {
            _pipeline = new StreamingPipeline(new M3UWebSource(streamUrl, httpClient), new OpenALSink())
                .AddMiddleware(new ChunkDownloaderMiddleware(httpClient))
                .AddMiddleware(new MemoryRecorderMiddleware(maxDelay))
                .AddMiddleware(new FFmpegDecoderMiddleware("ffmpeg"));
            _maxDelay = maxDelay; 
        }

        private bool _isPaused;
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (value == _isPaused) return;

                GetMiddleware<MemoryRecorderMiddleware>().IsPaused = value;

                if (value)
                {
                    GetMiddleware<OpenALSink>().Pause();
                    _pauseStartedAt = DateTime.Now;
                }
                else
                {
                    GetMiddleware<OpenALSink>().Play();
                    _delay += DateTime.Now - _pauseStartedAt;
                }

                _isPaused = value;
            }
        }

        public float Volume
        {
            get => GetMiddleware<OpenALSink>().Volume;
            set => GetMiddleware<OpenALSink>().Volume = value;
        }

        public TimeSpan Delay
        {
            get
            {
                var actualDelay = _delay + (IsPaused ? DateTime.Now - _pauseStartedAt : TimeSpan.Zero);

                return actualDelay < _maxDelay ?
                    actualDelay : _maxDelay;
            }
        }

        public void Play()
        {
            if (_cts is not null)
            {
                throw new InvalidOperationException("Cannot restart the player.");
            }

            _cts = new CancellationTokenSource();
            _processingLoopTask = Task.Run(async () =>
            {
                try
                {
                    await ProcessingLoop(_cts.Token);
                }
                catch (OperationCanceledException) { }
                catch(Exception e)
                {
                    //TODO: Log
                    Console.WriteLine(e.ToString());
                }
            });
        }

        public void Stop()
        {
            if (_processingLoopTask?.Status == TaskStatus.Running)
            {
                _cts?.Cancel();
                _processingLoopTask?.Wait();
            }
        }

        private async Task ProcessingLoop(CancellationToken ct)
        {
            GetMiddleware<OpenALSink>().Play();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _pipeline.Process(ct);
                    await Task.Delay(1000, ct);
                }
                catch (HttpRequestException) { }
            }
        }

        private T GetMiddleware<T>() where T : class
        {
            return (_pipeline.Middlewares.Single(x => x is T) as T)!;
        }

        public void Dispose()
        {
            foreach (var middleware in _pipeline.Middlewares)
            {
                (middleware as IDisposable)?.Dispose();
            }
        }
    }
}
