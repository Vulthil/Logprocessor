using System;
using Services.Shared.Models;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar.Tree
{
    public abstract class SendReceiveMessage : ContinuationNode, IEquatable<SendReceiveMessage>
    {
        protected Participant Participant { get; }
        protected Message Message { get; }
        protected ContinuationNode Continuation { get; }
        public abstract Direction Direction { get; }
        protected SendReceiveMessage(int line, Participant participant, Message message, ContinuationNode continuation) : base(line)
        {
            Participant = participant;
            Message = message;
            Continuation = continuation;
        }

        public override void PreAnalyze(ServiceDefinitionGenerator output)
        {
            output.MarkParticipant(Participant.Name);
            Continuation.PreAnalyze(output);
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            output.AddSendReceiveTransition(Direction, Participant.Name, Message.Label);
            Continuation.ToServiceDefinition(output);
        }
        public bool Equals(SendReceiveMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Participant, other.Participant) && Equals(Message, other.Message) && Equals(Continuation, other.Continuation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReceiveMessage)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Participant, Message, Continuation);
        }
    }
}