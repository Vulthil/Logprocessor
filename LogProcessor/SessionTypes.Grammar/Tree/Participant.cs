using System;

namespace SessionTypes.Grammar.Tree
{
    public class Participant : Node, IEquatable<Participant>
    {
        public string Name { get; }

        public Participant(int line, string name) : base(line)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Participant other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Participant) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}