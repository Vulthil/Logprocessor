using Services.Shared.Models;

namespace SessionTypes.Grammar.Tree
{
    public class ReceiveMessage : SendReceiveMessage//, IEquatable<ReceiveMessage>
    {
        public override Direction Direction { get; } = Direction.Inbound;

        public ReceiveMessage(int line, Participant participant, Message message, ContinuationNode continuation) : base(line, participant, message, continuation)
        {
        }

    }
}