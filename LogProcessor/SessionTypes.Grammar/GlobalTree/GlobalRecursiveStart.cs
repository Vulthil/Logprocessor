using System.Collections.Generic;
using SessionTypes.Grammar.Tree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.GlobalTree
{
    public class GlobalRecursiveStart : ContinuationNode
    {
        public string RecIdentifier { get; }
        public ContinuationNode GlobalMessage { get; }

        public GlobalRecursiveStart(int line, string recIdentifier, ContinuationNode globalMessage) : base(line)
        {
            RecIdentifier = recIdentifier;
            GlobalMessage = globalMessage;
        }
        public override HashSet<Participant> GetParticipants()
        {
            return GlobalMessage.GetParticipants();
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            var participants = GlobalMessage.GetParticipants();
            if (participants.Contains(participant))
            {
                return new RecursiveStartMessage(Line,RecIdentifier,
                    GlobalMessage.Project(participant, projectionBuilder));
            }
            else
            {
                return new EndMessage(Line);
            }
        }

    }
}