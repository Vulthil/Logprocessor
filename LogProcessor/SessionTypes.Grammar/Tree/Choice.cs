using System.Collections.Generic;
using MoreLinq.Extensions;
using Services.Shared.Models;

namespace SessionTypes.Grammar.Tree
{
    public class Choice : Branch
    {
        protected override Direction Direction => Direction.Outbound;

        public Choice(int line, Participant participant, Dictionary<Message, ContinuationNode> continuations) : base(line, participant, continuations)
        {
        }


    }
}