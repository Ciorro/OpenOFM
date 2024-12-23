using OpenOFM.Core.Streaming.Chunks;
using System.Globalization;

namespace OpenOFM.Core.Streaming.M3U
{
    internal class M3UParser
    {
        public static async Task<M3UPlaylist> ParsePlaylistAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var m3uPlaylist = new M3UPlaylist();

            string? line;
            while (!string.IsNullOrEmpty((line = await reader.ReadLineAsync())))
            {
                if (!line.StartsWith("#"))
                {
                    m3uPlaylist.ChunklistFilename = line;
                    continue;
                }

                if (line.StartsWith("#EXT-X-STREAM-INF:"))
                {
                    string[] properties = line[("#EXT-X-STREAM-INF:".Length)..].Split(',');

                    foreach (string property in properties)
                    {
                        int splitIndex = property.IndexOf('=');
                        string propertyName = property[..splitIndex];
                        string propertyValue = property[(splitIndex + 1)..];

                        switch (propertyName)
                        {
                            case "BANDWIDTH":
                                m3uPlaylist.Bandwidth = int.Parse(propertyValue);
                                break;
                            case "CODECS":
                                m3uPlaylist.Codec = propertyValue;
                                break;
                        }
                    }
                }
            }

            return m3uPlaylist;
        }

        public static async Task<IList<M3UChunk>> ParseChunklistAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var chunks = new List<M3UChunk>();

            int sequence = 0;
            string? line;
            while (!string.IsNullOrEmpty((line = await reader.ReadLineAsync())))
            {
                if (line.StartsWith("#EXT-X-MEDIA-SEQUENCE:"))
                {
                    sequence = int.Parse(line.Split(':')[1]);
                }

                if (line.StartsWith("#EXTINF:"))
                {
                    var duration = TimeSpan.FromSeconds(
                        float.Parse(line["#EXTINF:".Length..^1], CultureInfo.InvariantCulture));
                    var filename = (await reader.ReadLineAsync())!;

                    chunks.Add(new M3UChunk(sequence, duration, filename));

                    sequence++;
                }
            }

            return chunks;
        }
    }
}
