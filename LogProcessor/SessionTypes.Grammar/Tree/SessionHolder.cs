using System.Collections.Generic;
using System.IO;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.GlobalTree;

namespace SessionTypes.Grammar.Tree
{
    public class SessionHolder : SessionBase
    {
        public List<Session> Sessions { get; }
        
        public SessionHolder(int line, List<Session> sessions) : base(line)
        {
            Sessions = sessions;
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            Sessions.ForEach(s => s.ToServiceDefinition(output));
        }

        public override void PreAnalyze(ServiceDefinitionGenerator output)
        {
            Sessions.ForEach(s => s.PreAnalyze(output));
        }

        
    }
}