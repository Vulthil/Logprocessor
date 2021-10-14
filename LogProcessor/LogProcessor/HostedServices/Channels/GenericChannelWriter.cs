using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LogProcessor.HostedServices.Channels
{
    public class GenericChannelWriter<T> : IChannelWriter<T>
    {
        private readonly ChannelWriter<T> _writer;

        public GenericChannelWriter(GenericChannel<T> channel)
        {
            _writer = channel.Writer;
        }

        public bool TryComplete(Exception error = null) => _writer.TryComplete(error);


        public bool TryWrite(T item) => _writer.TryWrite(item);

        public ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default) =>
            _writer.WaitToWriteAsync(cancellationToken);

        public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default) =>
            _writer.WriteAsync(item, cancellationToken);

        public void Complete(Exception error = null) => _writer.Complete(error);
    }
}