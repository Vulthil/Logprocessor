using System.Threading.Channels;

namespace LogProcessor.HostedServices.Channels
{
    public class GenericChannel<T>
    {
        private readonly Channel<T> _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
            {SingleWriter = true, SingleReader = true});

        public ChannelReader<T> Reader => _channel.Reader;
        public ChannelWriter<T> Writer => _channel.Writer;
    }
}