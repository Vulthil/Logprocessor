using System.Collections.Generic;
using SessionTypes.Grammar.Tree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.GlobalTree
{
    public class GlobalSend : ContinuationNode
    {
        public Participant From { get; }
        public Participant To { get; }
        public Message Message { get; }
        public ContinuationNode Continuation { get; }

        public GlobalSend(int line, Participant from, Participant to, Message message, ContinuationNode continuation) : base(line)
        {
            From = from;
            To = to;
            Message = message;
            Continuation = continuation;
        }

        public override HashSet<Participant> GetParticipants()
        {
            var set = new HashSet<Participant>
            {
                From,
                To
            };
            set.UnionWith(Continuation.GetParticipants());
            return set;
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            if (participant.Equals(To))
            {
                return new ReceiveMessage(Line,From, Message, Continuation.Project(participant, projectionBuilder));
            }
            else if (participant.Equals(From))
            {
                return new SendMessage(Line,To, Message, Continuation.Project(participant, projectionBuilder));
            }
            else
            {
                return Continuation.Project(participant, projectionBuilder);
            }
        }

    }
}