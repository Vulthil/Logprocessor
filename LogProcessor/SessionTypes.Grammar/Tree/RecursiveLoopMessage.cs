using System;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.Tree
{
    public class RecursiveLoopMessage : ContinuationNode, IEquatable<RecursiveLoopMessage>
    {
        public string RecIdentifier { get; }

        public RecursiveLoopMessage(int line, string recIdentifier) : base(line)
        {
            RecIdentifier = recIdentifier;
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            if (!output.AddRecursionState(RecIdentifier))
            {
                Root.ReportSemanticError(Line, $"Recursion identifier '{RecIdentifier}' was not found.");
            }
        }

        public bool Equals(RecursiveLoopMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RecIdentifier == other.RecIdentifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecursiveLoopMessage) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), RecIdentifier);
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            return new RecursiveLoopMessage(Line,RecIdentifier);
        }
    }
}