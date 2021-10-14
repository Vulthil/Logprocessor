using System.Collections.Generic;

namespace Automata.Interfaces
{
    public interface IAutomatonConfiguration<TState, TTransition>
    {
        public IEnumerable<(TState state, TTransition input, TState next)> GetConfiguration();
    }
}