namespace OpenOFM.Core.Streaming
{
    public class StreamingPipeline
    {
        private readonly List<IChunkMiddleware> _middlewares = new();

        private readonly IChunkSource _source;
        private readonly IChunkSink _sink;

        public StreamingPipeline(IChunkSource source, IChunkSink sink)
        {
            _source = source;
            _sink = sink;
        }

        public IReadOnlyCollection<object> Middlewares
        {
            get => (_middlewares as IEnumerable<object>).Prepend(_source).Append(_sink).ToList();
        }

        public StreamingPipeline AddMiddleware(IChunkMiddleware middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }

        public StreamingPipeline AddMiddleware(params IChunkMiddleware[] middlewares)
        {
            _middlewares.AddRange(middlewares);
            return this;
        }

        public async Task Process(CancellationToken ct)
        {
            var steps = (_middlewares as IEnumerable<object>)
                .Prepend(_source).Append(_sink).ToList();

            for (int i = 0; i < steps.Count - 1; i++)
            {
                var source = steps[i] as IChunkSource;
                var sink = steps[i + 1] as IChunkSink;

                if (sink is not null && source is not null)
                {
                    await WriteAllChunks(sink, await ReadAllChunks(source, ct), ct);
                }
            }
        }

        private async Task<ICollection<IChunk>> ReadAllChunks(IChunkSource source, CancellationToken ct = default)
        {
            var chunks = new List<IChunk>();

            IChunk? chunk;
            while ((chunk = await source.ReadChunkAsync(ct)) is not null)
            {
                chunks.Add(chunk);
            }

            return chunks;
        }

        private async Task WriteAllChunks(IChunkSink sink, ICollection<IChunk> chunks, CancellationToken ct = default)
        {
            foreach (var chunk in chunks)
            {
                await sink.WriteChunkAsync(chunk, ct);
            }
        }
    }
}
