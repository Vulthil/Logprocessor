using System;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.Tree
{
    public class Message : Node, IEquatable<Message>
    {
        public string Label { get; }
        public Sort Sort { get; }
        public Message(int line, string label, Sort sort) : base(line)
        {
            Label = label;
            Sort = sort;
        }

        public override Node Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            return new Message(Line, Label, Sort);
        }

        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Label == other.Label && Equals(Sort, other.Sort);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Label, Sort);
        }
    }
}