using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Mapper
{
    public class CompilerPreparation
    {
        public INamedTypeSymbol AttributeType { get; private set; }
        public GeneratorExecutionContext Context { get; }
        public Compilation Compilation { get; private set; }

        public CompilerPreparation(GeneratorExecutionContext context)
        {
            Context = context;
            Prepare();
        }

        public void Prepare()
        {
            this.Context.AddSource(Constants.AttributeSourceName, Constants.AttributeSource);

            var compilation = Context.Compilation;

            var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            var contextCompilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(Constants.AttributeSource, Encoding.UTF8), options));
            Compilation = contextCompilation;
            AttributeType = contextCompilation.GetTypeByMetadataName($"{Constants.Namespace}.{Constants.AttributeName}Attribute");
        }
    }
}