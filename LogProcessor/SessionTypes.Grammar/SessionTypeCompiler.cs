using System;
using System.IO;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LogProcessor.Model.Json;
using SessionTypes.Grammar.Generated;
using SessionTypes.Grammar.Generator;
using SessionTypes.Grammar.GlobalTree;
using SessionTypes.Grammar.Tree;
using SessionTypes.Grammar.Visitor;

namespace SessionTypes.Grammar
{
    public class SyntaxException : CompileException
    {

        public SyntaxException() : base($"Syntax error occurred in the input.")
        {
        }
    }
    public class SessionTypeCompiler : ISessionTypeCompiler
    {
        public static void Main(string[] args)
        {
            var p = new SessionTypeCompiler();
            p.CompileFile("test.lt", Console.Out);
        }

        public IParseTree ParseFile(string fileName, TextWriter output, TextWriter errorOutput = null)
        {
            var charStream = new AntlrFileStream(fileName);
            return ParseStream(charStream, output, errorOutput);
        }

        public ServiceDefinitionGenerator CompileFile(string fileName, TextWriter output = null, TextWriter errorOutput = null)
        {
            output ??= Console.Out;
            errorOutput ??= output;

            var tree = ParseFile(fileName, output, errorOutput);
            return Compile(tree, errorOutput);
        }
        public ServiceDefinitionGenerator CompileText(string text, TextWriter output = null, TextWriter errorOutput = null)
        {
            output ??= Console.Out;
            errorOutput ??= output;

            var tree = ParseText(text, output, errorOutput);
            return Compile(tree, errorOutput);
        }

        private ServiceDefinitionGenerator Compile(IParseTree tree, TextWriter errorOutput)
        {
            try
            {
                var visitor = new Visitor.Visitor();
                var node = visitor.Visit<SessionBase>(tree);
                Node.Root = node;
                node.Output = errorOutput;
                if (node is GlobalType globalType)
                {

                    node = globalType.ProjectAll();
                }

                if (node is SessionHolder sessionHolder)
                {
                    Node.Root = sessionHolder;
                    var services = new ServiceDefinitionGenerator();
                    

                    sessionHolder.PreAnalyze(services);
                    if (sessionHolder.IsInError)
                    {
                        throw new CompileException($"PreAnalyze failed. Check {nameof(errorOutput)} for details");
                    }

                    sessionHolder.ToServiceDefinition(services);
                    if (sessionHolder.IsInError)
                    {
                        throw new CompileException($"Compilation failed. Check {nameof(errorOutput)} for details");
                    }

                    return services;
                    //return services.GetDefinition();
                }

                throw new CompileException("Unrecognized session.");
            }
            catch (Exception e)
            {
                if (e is CompileException)
                {
                    throw;
                }
                errorOutput.WriteLine(e);
                throw new CompileException(e);
            }

        }

        public IParseTree ParseText(string inputText, TextWriter output, TextWriter errorOutput = null)
        {
            var charStream = CharStreams.fromString(inputText);
            return ParseStream(charStream, output, errorOutput);

        }

        private IParseTree ParseStream(ICharStream charStream, TextWriter output, TextWriter errorOutput)
        {
            output ??= Console.Out;
            errorOutput ??= output;

            var lexer = new SessionTypesLexer(charStream, output, errorOutput);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SessionTypesParser(tokens, output, errorOutput);
            var tree = parser.protocol();
            if (parser.NumberOfSyntaxErrors > 0)
            {
                throw new SyntaxException();
            }

            return tree;
        }
    }

    public class CompileException : Exception
    {
        public CompileException(string message) : base(message)
        {
        }

        public CompileException(Exception innerException) : base(
            "Unknown error occurred. Check InnerException for details.", innerException)
        {

        }
    }
}