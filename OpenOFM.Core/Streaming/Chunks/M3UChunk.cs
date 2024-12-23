namespace OpenOFM.Core.Streaming.Chunks
{
    public class M3UChunk : IChunk
    {
        public int SequenceNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public string ChunkUrl { get; set; }

        public M3UChunk(int sequence, TimeSpan duration, string chunkUrl)
        {
            SequenceNumber = sequence;
            Duration = duration;
            ChunkUrl = chunkUrl;
        }
    }
}
