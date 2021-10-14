using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Interfaces;
using MoreLinq.Extensions;

namespace Automata.Automaton
{
    /// <summary>
    /// An <see cref="AutomatonDefinition{TState,TTransition}"/> for
    /// </summary>
    /// <typeparam name="TState">The identifier type for states.</typeparam>
    /// <typeparam name="TTransition">The identifier type for transitions.</typeparam>
    internal class AutomatonDefinition<TState, TTransition> : IAutomatonDefinition, IAutomatonDefinition<TState, TTransition>
    {
        internal readonly IDictionary<TState, State<TState>> States;
        internal readonly IDictionary<TTransition, Transition<TTransition>> Transitions;
        private Dictionary<State<TState>, IEnumerable<Transition<TTransition>>> TransitionsForStates { get; set; }

        private Dictionary<(State<TState> state, Transition<TTransition> transition), State<TState>> Definition { get; set; }

        internal AutomatonDefinition(IEnumerable<(State<TState> start, Transition<TTransition> transition, State<TState> end)> definition, IDictionary<TState, State<TState>> states, IDictionary<TTransition, Transition<TTransition>> transitions)
        {
            States = states;
            Transitions = transitions;
            Definition = definition.ToDictionary(x => (x.start, x.transition), x => x.end);
            TransitionsForStates = Definition.Keys.GroupBy(d => d.state)
                .ToDictionary(x => x.Key, y => y.Select(z => z.transition));
        }

        /// <summary>
        /// Creates an <see cref="AutomatonDefinition{TState,TTransition}"/> given a list of valid transitions.
        /// </summary>
        /// <param name="definition">List of <see cref="ValueTuple{TState, TTransition, TState}"/> which are valid in the automaton.</param>
        /// <returns>A new <see cref="AutomatonDefinition{TState,TTransition}"/> for the given <paramref name="definition"/>.</returns>
        internal static AutomatonDefinition<TState, TTransition> Create(IEnumerable<(TState state, TTransition input, TState end)> definition)
        {
            var endDefinitions = definition.Where(s => !definition.Select(d => d.state).Contains(s.end));
            var stateDefinitions = definition.Count() == 1 ? definition : definition.Except(endDefinitions);

            var endStates = endDefinitions.Select(s => s.end).Distinct().Select(s => new End<TState>(s));
            var states = stateDefinitions.SelectMany(s => new []{s.end, s.state}).Distinct().Select(s => new State<TState>(s));
            states = states.Concat(endDefinitions.Select(s => s.end).Distinct().Select(s => new State<TState>(s)));
            var transitions = definition.Select(s => s.input).Distinct().Select(s => new Transition<TTransition>(s));

            var stateDict = endStates.Concat(states).DistinctBy(x => x.Label).ToDictionary(x => x.Label);
            var transitionDict = transitions.ToDictionary(x => x.Label);

            return new AutomatonDefinition<TState, TTransition>(definition.Select(d => (stateDict[d.state], transitionDict[d.input], stateDict[d.end])), stateDict, transitionDict);
        }

        internal State<TState> Move(State<TState> state, Transition<TTransition> transition)
        {
            if (Definition.TryGetValue((state, transition), out var newState))
            {
                return newState;
            }

            return null;
        }

        /// <summary>
        /// Make a new <see cref="StatefulAutomaton{TState,TTransition}"/> for the given start state.
        /// </summary>
        /// <param name="startState">The identifier of the starting state.</param>
        /// <returns>A new <see cref="StatefulAutomaton{TState,TTransition}"/> with the current instance of <see cref="IAutomatonDefinition{TState,TTransition}"/> as basis.</returns>
        public IStatefulAutomaton<TState, TTransition> MakeStatefulAutomaton(TState startState)
        {
            return new StatefulAutomaton<TState, TTransition>(this, startState);
        }

        public IEnumerable<TTransition> StateTransitions(TState state)
        {
            return Definition.Keys.Where(k => k.state.Label.Equals(state)).Select(k => k.transition.Label);
        }

        IEnumerable<Transition<TTransition>> IAutomatonDefinition<TState, TTransition>.TransitionsForState(State<TState> state)
        {
            return TransitionsForStates[state];
        }
    }


}
