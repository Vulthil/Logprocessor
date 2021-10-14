using Services.Shared.Models;

namespace LogProcessor.Services
{
    public interface ILogMessageEnricher
    {
        public void EnrichMessage(LogMessage logMessage);
    }
}