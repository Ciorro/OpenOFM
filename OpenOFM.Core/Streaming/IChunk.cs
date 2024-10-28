namespace OpenOFM.Core.Streaming
{
    public interface IChunk
    {
        int SequenceNumber { get; }
        TimeSpan Duration { get; }
    }
}
