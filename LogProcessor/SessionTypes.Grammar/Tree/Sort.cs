using System;
using System.Collections.Generic;

namespace SessionTypes.Grammar.Tree
{
    public class Sort : Node, IEquatable<Sort>
    {
        public string SortIdentifier { get; }

        public Sort(int line, string sortIdentifier) : base(line)
        {
            SortIdentifier = sortIdentifier;
        }

        public bool Equals(Sort other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SortIdentifier == other.SortIdentifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Sort) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), SortIdentifier);
        }
    }
}