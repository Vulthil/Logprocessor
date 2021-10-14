using System;
using System.Reflection;

namespace HttpEnricher.Config
{
    public class ConsumerConfig
    {
        public string ReceiveEndpoint { get; set; }
        public string TypeFullName { get; set; }
        public Type Type { get; private set; }

        public Type GetConsumerType()
        {
            var type = Assembly.GetEntryAssembly().GetType(TypeFullName);
            Type = type;
            return Type;
        }
    }
}