using System;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.Tree
{
    public class EndMessage : ContinuationNode, IEquatable<EndMessage>
    {
        public EndMessage(int line) : base(line)
        {
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            output.MarkEndState();
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            return new EndMessage(Line);
        }

        public bool Equals(EndMessage other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EndMessage) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}