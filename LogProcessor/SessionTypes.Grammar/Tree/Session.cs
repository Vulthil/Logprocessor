using System;
using System.Collections.Generic;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar.Tree
{
    public class Session : ContinuationNode
    {
        public Participant Participant { get; }
        public ContinuationNode Start { get; }
        
        public Session(int line, Participant participant, ContinuationNode start) : base(line)
        {
            Participant = participant;
            Start = start;
        }

        public override void PreAnalyze(ServiceDefinitionGenerator output)
        {
            if (!output.MarkInternalParticipant(Participant.Name))
            {
                Root.ReportSemanticError(Line, $"Participant with name '{Participant.Name}' has already been defined.");
            }
            Start.PreAnalyze(output);
            //output.StopInternal();
        }

        public override void ToServiceDefinition(ServiceDefinitionGenerator output)
        {
            output.AddServiceDefinition(Participant.Name);

            Start.ToServiceDefinition(output);

            output.EndServiceDefinition();
        }

    }
}