using System;
using System.Collections.Generic;
using Services.Shared.Models;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.Tree
{
    public abstract class ContinuationNode : Node
    {
        protected ContinuationNode(int line) : base(line)
        {
        }

        public override ContinuationNode Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            throw new CompileException("");
        }
    }
    public class Offer : Branch
    {

        protected override Direction Direction => Direction.Inbound;
        public Offer(int line, Participant participant, Dictionary<Message, ContinuationNode> continuations) : base(line, participant, continuations)
        {
        }
        
        
    }
}