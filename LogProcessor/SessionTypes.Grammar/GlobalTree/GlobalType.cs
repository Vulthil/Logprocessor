using System.IO;
using System.Linq;
using SessionTypes.Grammar.Tree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.GlobalTree
{
    public abstract class SessionBase : Node
    {

        public bool IsInError { get; private set; }
        public TextWriter Output { get; set; }
        protected SessionBase(int line) : base(line)
        {
        }
        public void ReportSemanticError(int line, string error)
        {
            IsInError = true;
            Output.WriteLine($"Line: {line}, Error: {error}");
        }

    }
    public class GlobalType : SessionBase
    {
        public ContinuationNode GlobalMessage { get; }

        public GlobalType(int line, ContinuationNode globalMessage) : base(line)
        {
            GlobalMessage = globalMessage;
        }

        public SessionBase ProjectAll()
        {
            var projectionBuilder = new ProjectionBuilder();
            var participants = GlobalMessage.GetParticipants();
            var sessions = participants.Select(p=> Project(p, projectionBuilder));
            return new SessionHolder(Line, sessions.ToList()){Output = Output};
        }

        public override Session Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            var node = GlobalMessage.Project(participant, projectionBuilder);
            return new Session(Line,participant, node);
        }

    }
}