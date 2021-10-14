using System;
using Services.Shared.Models;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar.Tree
{
    public class SendMessage : SendReceiveMessage
    {
        public SendMessage(int line, Participant participant, Message message, ContinuationNode continuation) : base(line, participant, message, continuation)
        {
        }

        public override Direction Direction { get; } = Direction.Outbound;
        
    }
}