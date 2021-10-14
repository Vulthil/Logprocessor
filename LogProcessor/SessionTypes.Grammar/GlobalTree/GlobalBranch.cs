using System;
using System.Collections.Generic;
using System.Linq;
using Domemtech.Globbing;
using SessionTypes.Grammar.Tree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.GlobalTree
{
    public class GlobalBranch : ContinuationNode, IEquatable<GlobalBranch>
    {
        public Participant From { get; }
        public Participant To { get; }
        public Dictionary<Message, ContinuationNode> Continuations { get; }

        public GlobalBranch(int line, Participant from, Participant to, Dictionary<Message, ContinuationNode> continuations) : base(line)
        {
            From = from;
            To = to;
            Continuations = continuations;
        }
        public override HashSet<Participant> GetParticipants()
        {
            var set = new HashSet<Participant>
            {
                From,
                To
            };
            set.UnionWith(Continuations.Values.SelectMany(p => p.GetParticipants()));
            return set;
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            var conts = Continuations
                .ToDictionary(c => c.Key,
                    c => c.Value.Project(participant, projectionBuilder));
            if (participant.Equals(To))
            {
                
                
                return new Offer(Line,From, conts);
            }
            else if (participant.Equals(From))
            {
                return new Choice(Line,To, conts);
            }
            else
            {
                if (conts.Values.Distinct().Count() != 1)
                {
                    Root.ReportSemanticError(Line, "Not projectable, undefined, fix");
                    throw new CompileException("Not projectable, undefined, fix");
                }

                return conts.Values.First();
            }
        }

        public bool Equals(GlobalBranch other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(From, other.From) && Equals(To, other.To) && Equals(Continuations, other.Continuations);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GlobalBranch) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), From, To, Continuations);
        }
    }
}