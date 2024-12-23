using OpenOFM.Core.Streaming.Chunks;

namespace OpenOFM.Core.Streaming.M3U
{
    public class M3UWebSource : IChunkSource
    {
        private readonly Queue<M3UChunk> _buffer = new();

        private readonly HttpClient _http;
        private readonly Uri _m3uPlaylistUrl;
        private readonly Uri _streamBaseUrl;

        private M3UPlaylist? _m3uPlaylist;
        private DateTime _lastDownloadTime;
        private int _lastSequence;

        public M3UWebSource(Uri m3uPlaylistUrl, HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(m3uPlaylistUrl, nameof(m3uPlaylistUrl));

            _http = httpClient;

            _m3uPlaylistUrl = m3uPlaylistUrl;
            _streamBaseUrl = new Uri(_m3uPlaylistUrl.GetLeftPart(UriPartial.Authority) +
                                     string.Concat(_m3uPlaylistUrl.Segments[..^1]));
        }

        public async Task<IChunk?> ReadChunkAsync(CancellationToken ct)
        {
            if (_m3uPlaylist is null)
            {
                _m3uPlaylist = await GetM3uPlaylist(ct);
            }

            if (_buffer.Count > 1)
            {
                return _buffer.Dequeue();
            }

            if (ShouldDownload())
            {
                var chunks = (await GetChunklist(ct))
                    .Where(x => x.SequenceNumber > _lastSequence);

                foreach (var chunk in chunks)
                {
                    _buffer.Enqueue(chunk);
                    _lastSequence = chunk.SequenceNumber;
                }
            }

            return _buffer.TryDequeue(out var c) ? c : null;
        }

        private async Task<M3UPlaylist> GetM3uPlaylist(CancellationToken ct)
        {
            return await M3UParser.ParsePlaylistAsync(
                await _http.GetStreamAsync(_m3uPlaylistUrl, ct));
        }

        private async Task<IList<M3UChunk>> GetChunklist(CancellationToken ct)
        {
            var url = _streamBaseUrl + _m3uPlaylist!.ChunklistFilename;
            var chunks = await M3UParser.ParseChunklistAsync(await _http.GetStreamAsync(url));

            return chunks.Select(x =>
            {
                x.ChunkUrl = _streamBaseUrl + x.ChunkUrl;
                return x;
            }).ToList();
        }

        private bool ShouldDownload()
        {
            var now = DateTime.Now;
            var diff = now - _lastDownloadTime;
            if (diff > TimeSpan.FromSeconds(3))
            {
                _lastDownloadTime = now;
                return true;
            }

            return false;
        }
    }
}
