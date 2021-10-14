using System;
using System.Collections.Generic;
using System.IO;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.GlobalTree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar.Tree
{
    public abstract class Node
    {
        public int Line { get; set; }
        public static SessionBase Root { get; set; }
        protected Node(int line)
        {
            Line = line;
        }

        public virtual void PreAnalyze(ServiceDefinitionGenerator output)
        {

        }
        public virtual void Analyze(ServiceDefinitionGenerator output)
        {

        }
        public virtual void ToServiceDefinition(ServiceDefinitionGenerator output)
        {

        }

       

        public virtual HashSet<Participant> GetParticipants()
        {
            return new HashSet<Participant>();
        }

        public virtual Node Project(Participant participant, ProjectionBuilder projectionBuilder)
        {
            throw new CompileException("What are you doing here? Don't try to project a token that doesn't belong to a global type.");
        }

    }
}