using Services.Shared.Models;

namespace LogProcessor.Model.Json
{
    public class ServiceTransition
    {
        public string State { get; set; }
        public string NewState { get; set; }
        public string Message { get; set; }
        public string OpposingService { get; set; }
        public Direction Direction { get; set; }
    }
}