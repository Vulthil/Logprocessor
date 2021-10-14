using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using SessionTypes.Grammar.Generated;

namespace SessionTypes.Grammar.Visitor
{
    public class BaseVisitor<T> : SessionTypesParserBaseVisitor<T>
    {
        public T2 Visit<T2>(IParseTree context) where T2 : T
        {
            return (T2)Visit(context);
        }
        public List<T2> VisitCollection<T2>(IEnumerable<IParseTree> contexts) where T2 : T
        {
            return contexts.Select(Visit<T2>).ToList();
        }
    }
}