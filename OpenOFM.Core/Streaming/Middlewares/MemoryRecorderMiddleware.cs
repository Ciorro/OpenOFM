namespace OpenOFM.Core.Streaming.Middlewares
{
    public class MemoryRecorderMiddleware : IChunkMiddleware
    {
        private readonly Queue<IChunk> _buffer = new();
        private readonly TimeSpan _maxBufferDuration;

        public bool IsPaused { get; set; }

        public MemoryRecorderMiddleware()
            : this(TimeSpan.FromHours(1))
        { }

        public MemoryRecorderMiddleware(TimeSpan maxBufferDuration)
        {
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
