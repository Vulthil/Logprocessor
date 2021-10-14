using Services.Shared.Models;

namespace LogProcessor.Models.Infrastructure
{
    public class LogMessageEntity
    {
        private LogMessageEntity() { }
        public LogMessageEntity(LogMessage message, string errorMessage)
        {
            Message = message;
            ErrorMessage = errorMessage;
        }

        public int Id { get; set; }
        public string ErrorMessage { get; set; }
        public LogMessage Message { get; set; }
    }
}