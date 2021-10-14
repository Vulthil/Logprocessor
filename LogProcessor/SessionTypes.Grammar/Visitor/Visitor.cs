using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MoreLinq;
using SessionTypes.Grammar.Generated;
using SessionTypes.Grammar.GlobalTree;
using SessionTypes.Grammar.Tree;

namespace SessionTypes.Grammar.Visitor
{
    public static class ContextExtensions
    {
        public static int Line(this ParserRuleContext context)
        {
            return context.Start.Line;
        }
    }
    public class Visitor : BaseVisitor<Node>
    {
        
        public override Node VisitLocalSessionType(SessionTypesParser.LocalSessionTypeContext context)
        {
            return new SessionHolder(context.Line(), VisitCollection<Session>(context.localSession()));
        }

        

        public override Node VisitLocalSession(SessionTypesParser.LocalSessionContext context)
        {
            var participant = Visit<Participant>(context.participantName());
            var message = Visit<ContinuationNode>(context.tStart());
            return new Session(context.Line(), participant, message);
        }

        public override Node VisitGlobalType(SessionTypesParser.GlobalTypeContext context)
        {
            return new GlobalType(context.Line(), Visit<ContinuationNode>(context.gtStart()));
        }

        public override Node VisitGlobalSend(SessionTypesParser.GlobalSendContext context)
        {
            var from = Visit<Participant>(context.from);
            var to = Visit<Participant>(context.to);
            var message = Visit<Message>(context.messageLabel());
            var continuation = Visit<ContinuationNode>(context.globalContinuation());
            return new GlobalSend(context.Line(), from, to, message, continuation);
        }

        public override Node VisitGlobalBranch(SessionTypesParser.GlobalBranchContext context)
        {
            var from = Visit<Participant>(context.from);
            var to = Visit<Participant>(context.to);
            var messages = VisitCollection<Message>(context.messageLabel());
            var continuations = VisitCollection<ContinuationNode>(context.globalContinuation());
            return new GlobalBranch(context.Line(), from, to, messages.Zip(continuations).ToDictionary((x ) =>x.First, x => x.Second));
        }

        public override Node VisitGlobalRecursiveStart(SessionTypesParser.GlobalRecursiveStartContext context)
        {
            var recIdentifier = context.identifier().GetText();
            var contContext = (ParserRuleContext) context.gt() ?? context.globalContinuation();
            return new GlobalRecursiveStart(context.Line(), recIdentifier, Visit<ContinuationNode>(contContext));
        }

        public override Node VisitParticipantName(SessionTypesParser.ParticipantNameContext context)
        {
            return new Participant(context.Line(), context.GetText());
        }

        public override Node VisitChoiceMessage(SessionTypesParser.ChoiceMessageContext context)
        {
            var participant = Visit<Participant>(context.participantName());
            var messages = VisitCollection<Message>(context.messageLabel());
            var options = VisitCollection<ContinuationNode>(context.continuation());
            return new Choice(context.Line(), participant, messages.Zip(options).ToDictionary((x) => x.First, x => x.Second));
        }
        public override Node VisitGlobalContinuation(SessionTypesParser.GlobalContinuationContext context)
        {
            return Visit<Node>(context.gt());
        }
        public override Node VisitContinuation(SessionTypesParser.ContinuationContext context)
        {
            return Visit<Node>(context.t());
        }

        public override Node VisitOfferMessage(SessionTypesParser.OfferMessageContext context)
        {
            var participant = Visit<Participant>(context.participantName());
            var messages = VisitCollection<Message>(context.messageLabel());
            var options = VisitCollection<ContinuationNode>(context.continuation());
            return new Offer(context.Line(), participant, messages.Zip(options).ToDictionary((x) => x.First, x => x.Second));
        }
        

        public override Node VisitEndMessage(SessionTypesParser.EndMessageContext context)
        {
            return new EndMessage(context.Line());
        }

        public override Node VisitRecursiveLoop(SessionTypesParser.RecursiveLoopContext context)
        {
            var recIdentifier = context.identifier().GetText();
            return new RecursiveLoopMessage(context.Line(), recIdentifier);
        }

        public override Node VisitRecursiveStart(SessionTypesParser.RecursiveStartContext context)
        {
            var recIdentifier = context.identifier().GetText();
            return new RecursiveStartMessage(context.Line(), recIdentifier, Visit<ContinuationNode>(context.continuation()));
        }
        
        public override Node VisitSendReceive(SessionTypesParser.SendReceiveContext context)
        {
            var participant = Visit<Participant>(context.participantName());
            var message = Visit<Message>(context.messageLabel());
            var continuation = Visit<ContinuationNode>(context.continuation());
            switch (context.op.Type)
            {
                case SessionTypesParser.INTERR:
                    return new ReceiveMessage(context.Line(), participant,message,continuation);
                case SessionTypesParser.BANG:
                    return new SendMessage(context.Line(), participant,message, continuation);
                default:
                    throw new ArgumentOutOfRangeException(nameof(context));
            }
        }

        public override Node VisitMessageLabel(SessionTypesParser.MessageLabelContext context)
        {
            var message = context.identifier().GetText();
            Sort sort = null;
            if (context.sort() != null)
            {
                sort = Visit<Sort>(context.sort());
            }
            return new Message(context.Line(), message, sort);
        }

        public override Node VisitSort(SessionTypesParser.SortContext context)
        {
            return new Sort(context.Line(), context.identifier().GetText());
        }
        
    }
}