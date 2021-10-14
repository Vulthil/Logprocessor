namespace Automata.Automaton
{
    internal abstract class AutomataPrimitive<T>
    {
        public readonly T Label;

        internal AutomataPrimitive(T label)
        {
            Label = label;
        }
    }
    internal class State<T> : AutomataPrimitive<T>
    {
        internal State(T label) : base(label) { }
    }

    internal sealed class End<T> : State<T>
    {
        internal End(T label) : base(label) { }
    }

    internal class Transition<T> : AutomataPrimitive<T>
    {
        internal Transition(T label) : base(label) { }
    }
}
