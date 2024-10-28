namespace OpenOFM.Core.Streaming.Middlewares
{
    public class PauseBufferMiddleware : IChunkMiddleware
    {
        private readonly Queue<IChunk> _buffer = new();
        private readonly TimeSpan _maxBufferDuration;

        public bool IsPaused { get; set; }

        public PauseBufferMiddleware(TimeSpan maxBufferDuration)
        {
            if (maxBufferDuration < TimeSpan.FromSeconds(10))
            {
                throw new ArgumentException("The minimum buffer duration is 10s", nameof(maxBufferDuration));
            }

            _maxBufferDuration = maxBufferDuration;
        }

        public TimeSpan BufferedDuration
        {
            get => _buffer.Aggregate(TimeSpan.Zero, (ts, ch) => ts.Add(ch.Duration));
        }

        public Task WriteChunkAsync(IChunk chunk, CancellationToken ct = default)
        {
            while (BufferedDuration + chunk.Duration > _maxBufferDuration)
            {
                _buffer.TryDequeue(out _);
            }

            _buffer.Enqueue(chunk);
            return Task.CompletedTask;
        }

        public Task<IChunk?> ReadChunkAsync(CancellationToken ct = default)
        {
            if (!IsPaused)
            {
                return Task.FromResult(_buffer.TryDequeue(out var chunk) ? chunk : null);
            }

            return Task.FromResult<IChunk?>(null);
        }
    }
}
