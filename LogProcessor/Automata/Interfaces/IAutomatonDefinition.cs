using System.Collections.Generic;
using Automata.Automaton;

namespace Automata.Interfaces
{
    public interface IAutomatonDefinition<TState, TTransition>
    {
        /// <summary>
        /// Make a new <see cref="IStatefulAutomaton{TState,TTransition}"/> for the given start state.
        /// </summary>
        /// <param name="startState">The identifier of the starting state.</param>
        /// <returns>A new <see cref="IStatefulAutomaton{TState,TTransition}"/> with the current instance of <see cref="IAutomatonDefinition"/> as basis.</returns>
        IStatefulAutomaton<TState, TTransition> MakeStatefulAutomaton(TState startState);

        /// <summary>
        /// Retrieves the transitions exiting the configured start state.
        /// </summary>
        /// <returns>A list of transition labels</returns>
        public IEnumerable<TTransition> StateTransitions(TState state);

        internal IEnumerable<Transition<TTransition>> TransitionsForState(State<TState> state);

    }

    /// <summary>
    /// An <see cref="IAutomatonDefinition"/> for instantiating <see cref="IAutomatonDefinition{TState,TTransition}"/>
    /// without redundant type definitions.
    /// </summary>
    public interface IAutomatonDefinition
    {
        /// <summary>
        /// Creates an <see cref="IAutomatonDefinition{TState,TTransition}"/> for the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration defining the automaton.</param>
        /// <returns>A new <see cref="IAutomatonDefinition{TState,TTransition}"/>.</returns>
        public static IAutomatonDefinition<TState, TTransition> Create<TState, TTransition>(IAutomatonConfiguration<TState, TTransition> configuration)
        {
            return AutomatonDefinition<TState, TTransition>.Create(configuration.GetConfiguration());
        }

    }
}