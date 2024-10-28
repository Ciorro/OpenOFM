namespace OpenOFM.Core.Streaming.M3U
{
    public class M3UPlaylist
    {
        public int Bandwidth { get; set; } = 192000;
        public string Codec { get; set; } = "mp4a.40.2";
        public string ChunklistFilename { get; set; } = "";

        public M3UPlaylist() { }

        public M3UPlaylist(string chunklistFilename)
            : this(chunklistFilename, "", 192000)
        { }

        public M3UPlaylist(string chunklistFilename, string codec, int bandwidth)
        {
            ChunklistFilename = chunklistFilename;
            Codec = codec;
            Bandwidth = bandwidth;
        }
    }
}
