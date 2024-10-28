using OpenOFM.Core.Streaming.Chunks;

namespace OpenOFM.Core.Streaming.Middlewares
{
    public class ChunkDownloaderMiddleware : IChunkMiddleware
    {
        private readonly Queue<IChunk> _buffer = new();
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUrl;

        public ChunkDownloaderMiddleware(HttpClient httpClient, Uri baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
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

            string chunkUrl = _baseUrl + "/" + m3UChunk.Filename;
            var data = await _httpClient.GetByteArrayAsync(chunkUrl, ct);

            _buffer.Enqueue(new DataChunk(m3UChunk.SequenceNumber, m3UChunk.Duration, data));
        }

        public Task<IChunk?> ReadChunkAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_buffer.TryDequeue(out var chunk) ? chunk : null);
        }
    }
}
