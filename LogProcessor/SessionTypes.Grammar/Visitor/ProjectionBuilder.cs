using System.Collections.Generic;
using SessionTypes.Grammar.Tree;

namespace SessionTypes.Grammar.Visitor
{
    public class ProjectionBuilder
    {
        public HashSet<Participant> Participants { get; set; } = new ();

        public void AddParticipant(Participant participant)
        {
            Participants.Add(participant);
        }
    }
}