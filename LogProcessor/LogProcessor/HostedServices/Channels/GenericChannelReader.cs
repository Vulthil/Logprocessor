using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LogProcessor.HostedServices.Channels
{
    public class GenericChannelReader<T> : IChannelReader<T>
    {
        private readonly ChannelReader<T> _reader;

        public GenericChannelReader(GenericChannel<T> channel)
        {
            _reader = channel.Reader;
        }

        public Task Completion => _reader.Completion;
        public bool CanCount => _reader.CanCount;
        public int Count => _reader.Count;
        public bool TryRead(out T item) => _reader.TryRead(out item);

        public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default) =>
            _reader.WaitToReadAsync(cancellationToken);

        public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default) => _reader.ReadAsync(cancellationToken);

        public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default) =>
            _reader.ReadAllAsync(cancellationToken);
    }
}