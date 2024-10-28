namespace OpenOFM.Core.Streaming.Chunks
{
    public class M3UChunk : IChunk
    {
        public int SequenceNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public string Filename { get; set; }

        public M3UChunk(int sequence, TimeSpan duration, string filename)
        {
            SequenceNumber = sequence;
            Duration = duration;
            Filename = filename;
        }
    }
}
