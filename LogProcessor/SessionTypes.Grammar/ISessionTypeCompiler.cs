using System.IO;
using Antlr4.Runtime.Tree;
using LogProcessor.Model.Json;
using SessionTypes.Grammar.Generator;

namespace SessionTypes.Grammar
{
    public interface ISessionTypeCompiler
    {
        IParseTree ParseFile(string fileName, TextWriter output, TextWriter errorOutput = null);
        ServiceDefinitionGenerator CompileFile(string fileName, TextWriter output = null, TextWriter errorOutput = null);
        ServiceDefinitionGenerator CompileText(string text, TextWriter output = null, TextWriter errorOutput = null);
        IParseTree ParseText(string inputText, TextWriter output, TextWriter errorOutput = null);
    }
}