using System.Collections.Generic;
using System.Threading.Tasks;
using Automata.Automaton;
using Automata.Interfaces;
using LogProcessor.Model.Json;

namespace LogProcessor.Interfaces
{
    public interface IAutomatonFactory<TState, TTransition>
    {
        public (IStatefulAutomaton<string, string> automaton, string sessionName) GetAutomaton(string serviceId);

        IEnumerable<TTransition> GetStartStateTransitions(string serviceId);
        public IEnumerable<string> TrackedServices { get; }
        Task InitializeAutomatonConfigurations(ServiceAutomata definitions);
        void Unset(string sessionName);
    }
}