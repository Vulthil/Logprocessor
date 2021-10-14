using System.Collections.Generic;
using System.Linq;
using Automata.Helpers;
using Automata.Interfaces;

namespace Automata.Automaton
{
    /// <summary>
    /// A <see cref="StatefulAutomaton{TState,TTransition}"/> for
    /// </summary>
    /// <typeparam name="TState">The identifier type for states.</typeparam>
    /// <typeparam name="TTransition">The identifier type for transitions.</typeparam>
    internal class StatefulAutomaton<TState, TTransition> : IStatefulAutomaton<TState, TTransition>
    {
        private State<TState> State { get; set; }
        internal AutomatonDefinition<TState, TTransition> AutomatonDefinition { get; set; }
        IAutomatonDefinition<TState, TTransition> IStatefulAutomaton<TState, TTransition>.AutomatonDefinition
        {
            get => AutomatonDefinition;
        }

        internal StatefulAutomaton(AutomatonDefinition<TState, TTransition> definition, TState startState)
        {
            AutomatonDefinition = definition;
            State = AutomatonDefinition.States[startState];
        }

        /// <summary>
        /// Takes the given <paramref name="transitionId"/> in the current <see cref="State"/> the <see cref="StatefulAutomaton{TState,TTransition}"/> is in.
        /// </summary>
        /// <param name="transitionId">The identifier of the transition to take.</param>
        public MoveResult<TState> Move(TTransition transitionId)
        {
            if (AutomatonDefinition.Transitions.TryGetValue(transitionId, out var transition))
            {
                var newState = AutomatonDefinition.Move(State, transition);

                if (newState != null)
                {
                    State = newState;
                    return (State is End<TState>)
                        ? new EndStateResult<TState>(State.Label)
                        : new NewStateResult<TState>(State.Label);
                }

                return new ErrorResult<TState>(TransitionErrorTextHelper.InvalidTransition(State, transition));
            }
            
            return new ErrorResult<TState>(TransitionErrorTextHelper.NoSuchTransition(transitionId));

        }

        public IEnumerable<TTransition> GetTransitionsFromCurrentState()
        {
            return ((IAutomatonDefinition<TState,TTransition>)AutomatonDefinition).TransitionsForState(State).Select(x => x.Label);
        }
    }
}