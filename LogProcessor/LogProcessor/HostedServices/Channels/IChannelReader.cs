using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace LogProcessor.HostedServices.Channels
{
    public interface IChannelReader<T>
    {
        public Task Completion { get; }
        
        public bool CanCount { get; }
        
        public int Count { get; }
        public bool TryRead([MaybeNullWhen(false)] out T item);
        public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);
        public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default);

        public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}