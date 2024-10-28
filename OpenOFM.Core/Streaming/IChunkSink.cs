namespace OpenOFM.Core.Streaming
{
    public interface IChunkSink
    {
        Task WriteChunkAsync(IChunk chunk, CancellationToken ct = default);
    }
}
