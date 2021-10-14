using System;
using System.Collections.Generic;
using Services.Shared.Models;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar.Tree
{
    public abstract class Branch : ContinuationNode, IEquatable<Branch>
    {
        protected Participant Participant { get; }
        protected Dictionary<Message, ContinuationNode> Continuations { get; }
        protected abstract Direction Direction { get; }
        protected Branch(int line, Participant participant, Dictionary<Message, ContinuationNode> continuations) : base(line)
        {
            Participant = participant;
            Continuations = continuations;
        }

        public override void PreAnalyze(ServiceDefinitionGenerator output)
        {
            output.MarkParticipant(Participant.Name);
            foreach (var (_,continuationNode) in Continuations)
            {
                continuationNode.PreAnalyze(output);
            }
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            var state = output.GetLatestState();
            foreach (var (message, continuation) in Continuations)
            {
                output.AddSendReceiveTransition(Direction, Participant.Name, message.Label, state);
                continuation.ToServiceDefinition(output);
            }

        }

        public bool Equals(Branch other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Participant, other.Participant) && Equals(Continuations, other.Continuations);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Branch)obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Participant, Continuations);
        }
    }
}