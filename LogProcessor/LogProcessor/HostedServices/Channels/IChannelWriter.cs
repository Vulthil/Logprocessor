using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogProcessor.HostedServices.Channels
{
    public interface IChannelWriter<T>
    {
        public bool TryComplete(Exception error = null);
        public bool TryWrite(T item);
        public ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default);
        public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default);
        public void Complete(Exception error = null);
    }
}