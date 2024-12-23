using OpenOFM.Core.Streaming.Chunks;

namespace OpenOFM.Core.Streaming.Middlewares
{
    public class ChunkDownloaderMiddleware : IChunkMiddleware
    {
        private readonly Queue<IChunk> _buffer = new();
        private readonly HttpClient _httpClient;

        public ChunkDownloaderMiddleware(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public TimeSpan BufferedDuration
        {
            get => _buffer.Aggregate(TimeSpan.Zero, (ts, ch) => ts.Add(ch.Duration));
        }

        public async Task WriteChunkAsync(IChunk chunk, CancellationToken ct = default)
        {
            if (chunk is not M3UChunk m3UChunk)
            {
                throw new ArgumentException(
                    $"Invalid input chunk type. Expected {typeof(M3UChunk)}", nameof(chunk));
            }

            var data = await _httpClient.GetByteArrayAsync(m3UChunk.ChunkUrl, ct);
            _buffer.Enqueue(new DataChunk(m3UChunk.SequenceNumber, m3UChunk.Duration, data));
        }

        public Task<IChunk?> ReadChunkAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_buffer.TryDequeue(out var chunk) ? chunk : null);
        }
    }
}
