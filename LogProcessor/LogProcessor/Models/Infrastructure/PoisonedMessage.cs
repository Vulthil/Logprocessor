namespace LogProcessor.Models.Infrastructure
{
    public class PoisonedMessage
    {
        private PoisonedMessage() { }
        public PoisonedMessage(LogMessageEntity msgEntity, ViolatingMessage violatingMessage)
        {
            LogMessage = msgEntity;
            ViolatingMessage = violatingMessage;
        }
        public PoisonedMessage(LogMessageEntity msgEntity, int violatingMessageId)
        {
            LogMessage = msgEntity;
            ViolatingMessageId = violatingMessageId;
        }

        public int Id { get; set; }

        public int LogMessageId { get; set; }
        public LogMessageEntity LogMessage { get; set; }

        public int ViolatingMessageId { get; set; }

        public ViolatingMessage ViolatingMessage { get; set; }
        
    }
}