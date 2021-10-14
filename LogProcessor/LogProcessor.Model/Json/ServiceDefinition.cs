using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Interfaces;
using LogProcessor.Model.Utilities;

namespace LogProcessor.Model.Json
{
    public class ServiceAutomata
    {
        public string Name { get; set; }
        public Dictionary<string, ServiceDefinition> ServicesDefinitions { get; set; }
    }

    public class ServiceDefinition : IAutomatonConfiguration<string, string>
    {
        public string ServiceId { get; set; }
        public string StartState { get; set; }
        public List<ServiceTransition> Transitions { get; set; }
        private IEnumerable<(string state, string input, string next)> _definition {get; set;}
        public string SMCatString => _definition.Aggregate(string.Empty, (prev, next) => string.Join(prev, SMCatSingleLine(next), "\n")).Replace("Awaiting","initial").Replace("Completed","final");
        private Func<(string, string, string), string> SMCatSingleLine = ((string state, string input, string next) tuple) => $"{tuple.state} => {tuple.next} : {tuple.input};";

        public IEnumerable<(string state, string input, string next)> GetConfiguration()
        {
            return _definition ??= Transitions.Select(t => (state: t.State, input: t.ToTransitionLabel(), next: t.NewState));
        }
    }
}