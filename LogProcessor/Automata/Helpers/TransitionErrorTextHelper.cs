using Automata.Automaton;

namespace Automata.Helpers
{
    internal static class TransitionErrorTextHelper
    {
        internal static string NoSuchTransition<T>(T transitionId)
        {
            return $"No transition with value {transitionId} defined.";
        }


        internal static string InvalidTransition<TState, TTransition>(State<TState> state, Transition<TTransition> transition)
        {
            return $"Transition {transition.Label} is not valid from state {state.Label}.";
        }
    }
}