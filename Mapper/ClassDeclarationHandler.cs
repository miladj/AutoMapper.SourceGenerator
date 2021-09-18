using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mapper
{
    public class ClassDeclarationHandler
    {
        private readonly CompilerPreparation _compilerPreparation;
        private readonly ClassDeclarationSyntax _mClassDeclarationSyntax;
        private readonly ISymbol? _classSymbolInfo;
        private readonly ClassMapGenerator _classMapGenerator;

        public ClassDeclarationHandler(CompilerPreparation compilerPreparation, ClassDeclarationSyntax mClassDeclarationSyntax)
        {
            _compilerPreparation = compilerPreparation;
            _mClassDeclarationSyntax = mClassDeclarationSyntax;
            SemanticModel semanticModel = _compilerPreparation.Compilation.GetSemanticModel(_mClassDeclarationSyntax.SyntaxTree);
            _classSymbolInfo = semanticModel.GetDeclaredSymbol(_mClassDeclarationSyntax);
            _classMapGenerator = new ClassMapGenerator(_classSymbolInfo,_compilerPreparation);
        }
        public string CreateSource()
        {

            if (_classSymbolInfo.ContainingType != null)
            {
                _compilerPreparation.Context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(id: "Convertor001",
                    title: "Class cannot be nested class",
                    messageFormat: "Class cannot be nested class '{0}'.",
                    category: "MapMethodGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true), _classSymbolInfo.Locations[0], _classSymbolInfo.ToDisplayString()));
                return null;
            }
            ImmutableArray<AttributeData> attributeDatas = _classSymbolInfo.GetAttributes();

            foreach (AttributeData attributeData in attributeDatas)
            {
                if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _compilerPreparation.AttributeType))
                    continue;
                // if (!symbol.IsStatic)
                // {
                //     context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(id: "Convertor003",
                //         title: "Class must be static",
                //         messageFormat: "Class must be static '{0}'.",
                //         category: "MapMethodGenerator",
                //         DiagnosticSeverity.Error,
                //         isEnabledByDefault: true), symbol.Locations[0], mClassDeclarationSyntax.Identifier.Text));
                //     // throw new NotSupportedException("Class must be partial");
                //     break;
                //     
                // }
                if (_mClassDeclarationSyntax.Modifiers.All(x => x.Text != "partial"))
                {
                    _compilerPreparation.Context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(id: "Convertor001",
                        title: "Class must be partial",
                        messageFormat: "Class must be partial '{0}'.",
                        category: "MapMethodGenerator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true), _classSymbolInfo.Locations[0], _mClassDeclarationSyntax.Identifier.Text));
                    // throw new NotSupportedException("Class must be partial");
                    break;
                }
                ImmutableArray<TypedConstant> attributeDataConstructorArguments = attributeData.ConstructorArguments;
                if (attributeDataConstructorArguments.Length != 2)
                {
                    _compilerPreparation.Context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(id: "Convertor002",
                        title: "Please provide two type",
                        messageFormat: "two types must be provided.",
                        category: "MapMethodGenerator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true), _classSymbolInfo.Locations[0], _mClassDeclarationSyntax.Identifier.Text));
                    // throw new NotSupportedException("Class must be partial");
                    break;
                }
                var fromType = attributeDataConstructorArguments[0].Value as INamedTypeSymbol;
                var toType = (attributeDataConstructorArguments[1].Value as INamedTypeSymbol);
                
                _classMapGenerator.CreateConvertMethod(fromType, toType);
            }

            return _classMapGenerator.ToString();
        }

        
    }
}