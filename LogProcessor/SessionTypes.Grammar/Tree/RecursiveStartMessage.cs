using System;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar.Tree
{
    public class RecursiveStartMessage : ContinuationNode, IEquatable<RecursiveStartMessage>
    {
        public string RecIdentifier { get; }
        public ContinuationNode Continuation { get; }

        public RecursiveStartMessage(int line, string recIdentifier, ContinuationNode continuation) : base(line)
        {
            RecIdentifier = recIdentifier;
            Continuation = continuation;
        }
        public override void PreAnalyze(ServiceDefinitionGenerator output)
        {
            Continuation.PreAnalyze(output);
        }
        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            output.MarkRecursiveStart(RecIdentifier);
            Continuation.ToServiceDefinition(output);
            output.EndRecursion(RecIdentifier);
        }

        public bool Equals(RecursiveStartMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RecIdentifier == other.RecIdentifier && Equals(Continuation, other.Continuation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecursiveStartMessage) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), RecIdentifier, Continuation);
        }
    }
}