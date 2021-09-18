using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mapper
{
    class ClassDeclarationSyntaxReceiver : ISyntaxReceiver
    {

        public List<ClassDeclarationSyntax> ClassDeclarationCandidates { get; } = new List<ClassDeclarationSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds)
            {
                if (cds.AttributeLists.Any())
                    ClassDeclarationCandidates.Add(cds);

            }
        }
    }
}