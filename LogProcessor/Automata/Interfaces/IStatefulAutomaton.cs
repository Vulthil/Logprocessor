using System.Collections.Generic;
using Automata.Automaton;

namespace Automata.Interfaces
{
    public interface IStatefulAutomaton<TState, TTransition>
    {
        IAutomatonDefinition<TState, TTransition> AutomatonDefinition { get; }

        /// <summary>
        /// Takes the given <paramref name="transitionId"/> in the current <see cref="StatefulAutomaton{TState,TTransition}.State"/> the <see cref="StatefulAutomaton{TState,TTransition}"/> is in.
        /// </summary>
        /// <param name="transitionId">The identifier of the transition to take.</param>
        /// <returns>The identifier of the new state.</returns>
        MoveResult<TState> Move(TTransition transitionId);

        public IEnumerable<TTransition> GetTransitionsFromCurrentState();
    }

    public abstract class MoveResult { }
    public abstract class MoveResult<T> : MoveResult
    {
        public abstract bool WasValid { get; }
    }

    public class NewStateResult<T> : MoveResult<T>
    {
        public T NewState { get; }

        public NewStateResult(T newState)
        {
            NewState = newState;
        }

        public override bool WasValid => true;
    }

    public class EndStateResult<T> : NewStateResult<T>
    {
        public EndStateResult(T newState) : base(newState)
        {
        }
    }

    public class ErrorResult<T> : MoveResult<T>
    {
        public string ErrorMsg { get; }

        public ErrorResult(string errorMsg)
        {
            ErrorMsg = errorMsg;
        }

        public override bool WasValid => false;
        
    }

}