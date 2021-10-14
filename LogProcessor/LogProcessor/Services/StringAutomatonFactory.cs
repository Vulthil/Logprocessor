using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automata.Automaton;
using LogProcessor.Interfaces;
using Automata.Interfaces;
using LogProcessor.Model.Json;
using MoreLinq;

namespace LogProcessor.Services
{
    public class StringAutomatonFactory : IAutomatonFactory<string, string>
    {
        private readonly Dictionary<string, string> _startStates = new();
        private Dictionary<string, (IAutomatonDefinition<string, string> automatonDefinition, string sessionName)> _automatons { get; set; } = new();
        public IEnumerable<string> TrackedServices => _automatons.Keys;
        private string GetStartState(string serviceId) => _startStates[serviceId];
        
        public void Unset(string sessionName)
        {
            var keys = _automatons.Where(kvp =>
                kvp.Value.sessionName.Equals(sessionName, StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Key);
            keys.ForEach(k =>
            {
                _automatons.Remove(k);
                _startStates.Remove(k);
            });
        }
        public IAutomatonDefinition<string, string> RegisterAutomaton(string sessionName, string serviceId, ServiceDefinition config)
        {
            _startStates[serviceId] = config.StartState;
            var definition = CreateAutomaton(config);
            _automatons[serviceId] = (definition, sessionName);
            return definition;
        }

        public (IStatefulAutomaton<string, string> automaton, string sessionName) GetAutomaton(string serviceId)
        {
            (var automatonDefinition, var sessionName) = _automatons[serviceId];
            return (automatonDefinition.MakeStatefulAutomaton(GetStartState(serviceId)), sessionName);
        }

        public IEnumerable<string> GetStartStateTransitions(string serviceId)
        {
           return _automatons[serviceId].automatonDefinition.StateTransitions(GetStartState(serviceId));
        }

        protected virtual IAutomatonDefinition<string, string> CreateAutomaton(IAutomatonConfiguration<string, string> config)
        {
            return IAutomatonDefinition.Create(config);
        }

        public Task InitializeAutomatonConfigurations(ServiceAutomata definitions)
        {
            definitions.ServicesDefinitions.ForEach(d =>
                RegisterAutomaton(definitions.Name, d.Key, d.Value));
            return Task.CompletedTask;
        }

       
    }
    
}