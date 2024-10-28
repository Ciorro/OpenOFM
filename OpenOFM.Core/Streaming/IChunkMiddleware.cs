namespace OpenOFM.Core.Streaming
{
    public interface IChunkMiddleware : IChunkSource, IChunkSink
    {
        TimeSpan BufferedDuration { get; }
    }
}
