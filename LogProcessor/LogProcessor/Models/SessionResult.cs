using Services.Shared.Models;

namespace LogProcessor.Models
{
    public abstract class SessionResult
    {
        public LogMessage Message { get; set; }

        protected SessionResult(LogMessage message)
        {
            Message = message;
        }
    }

    public class SuccessResult : SessionResult
    {
        public SuccessResult(LogMessage message) : base(message) { }
    }

    public sealed class EndResult : SuccessResult
    {
        public EndResult(LogMessage message) : base(message) { }
    }

    public sealed class SkippedMessageResult : SuccessResult
    {
        public SkippedMessageResult(LogMessage message) : base(message) { }
    }


    public abstract class ErrorResult : SessionResult
    {
        public string ErrorMessage { get; set; }

        protected ErrorResult(LogMessage message, string e) : base(message)
        {
            ErrorMessage = e;
        }

    }
    public sealed class ExceptionResult : ErrorResult
    {
        public ExceptionResult(LogMessage message, string e) : base(message, e) { }

        public static ExceptionResult PoisonedMessage(LogMessage message)
        {
            return new ExceptionResult(message, "Cannot advance local session because global session is faulted.");
        }

    }
    public sealed class TimeoutResult : ErrorResult
    {
        public TimeoutResult(LogMessage message, string e) : base(message, e) { }
        public TimeoutResult(string sessionId, string e) : this(new LogMessage() { SessionId = sessionId }, e) { }

    }

    public sealed class IllegalStartResult : ErrorResult
    {
        public IllegalStartResult(LogMessage message, string e) : base(message, e) { }

    }

    public sealed class EvictionResult : ErrorResult
    {
        private EvictionResult(string sessionId, string message) : base(new LogMessage() { SessionId = sessionId }, message) { }

        public static EvictionResult ForcedEvictionResult(string sessionId)
        {
            return new(sessionId, "Session was forcibly evicted from tracking.");
        }

        public static EvictionResult FailedSafeEvictionResult(string sessionId)
        {
            return new(sessionId, "Session tried to safely terminate, but has hanging messages");
        }
    }

}