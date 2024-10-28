namespace OpenOFM.Core.Streaming
{
    public interface IChunkSource
    {
        Task<IChunk?> ReadChunkAsync(CancellationToken ct = default);
    }
}
