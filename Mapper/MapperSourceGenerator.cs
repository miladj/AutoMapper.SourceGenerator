using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Mapper
{
    [Generator]
    public class MapperSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // if (!Debugger.IsAttached)
            // {
            //     Debugger.Launch();
            // }

            context.RegisterForSyntaxNotifications(() => new ClassDeclarationSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilerPreparation = new CompilerPreparation(context);

            if (context.SyntaxReceiver is ClassDeclarationSyntaxReceiver m)
            {
                string newClass = string.Empty;
                foreach (var mClassDeclarationSyntax in m.ClassDeclarationCandidates)
                {
                    string source = new ClassDeclarationHandler(compilerPreparation, mClassDeclarationSyntax).CreateSource();
                    if(!string.IsNullOrEmpty(source))
                        newClass+=source;
                }
                
                if (!string.IsNullOrEmpty(newClass))
                {
                    context.AddSource(Constants.MapperSourceName, newClass);
                }
            }

        }

        
    }
}