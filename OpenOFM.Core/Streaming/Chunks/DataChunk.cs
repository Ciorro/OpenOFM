namespace OpenOFM.Core.Streaming.Chunks
{
    public class DataChunk : IChunk
    {
        public int SequenceNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public byte[] Data { get; set; }

        public DataChunk(int sequence, TimeSpan duration, byte[] data)
        {
            SequenceNumber = sequence;
            Duration = duration;
            Data = data;
        }
    }
}
