using System.Collections.Generic;

namespace LogProcessor.Models.Infrastructure
{
    public class ViolatingMessage
    {
        private ViolatingMessage() { }
        public ViolatingMessage(LogMessageEntity logMessageEntity)
        {
            LogMessage = logMessageEntity;
        }

        public int Id { get; set; }

        public int LogMessageId { get; set; }
        public LogMessageEntity LogMessage { get; set; }

        public ICollection<PoisonedMessage> PoisonedMessages { get; set; }
    }
}