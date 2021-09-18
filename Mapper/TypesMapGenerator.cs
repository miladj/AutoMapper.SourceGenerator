using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Mapper
{
    public class TypesMapGenerator
    {
        private readonly ITypeSymbol _fromType;
        private readonly ITypeSymbol _toType;
        private readonly SourceGenerator _sourceGenerator;
        private readonly CompilerPreparation _compilerPreparation;
        private readonly HashSet<(ITypeSymbol, ITypeSymbol)> _typePairList;
        private readonly ISymbol _symbolInfo;

        public TypesMapGenerator(ISymbol symbol, ITypeSymbol fromType, ITypeSymbol toType,
            SourceGenerator sourceGenerator, CompilerPreparation compilerPreparation,
            HashSet<(ITypeSymbol, ITypeSymbol)> typePairList)
        {
            _symbolInfo = symbol;
            this._fromType = fromType;
            this._toType = toType;
            _sourceGenerator = sourceGenerator;
            _compilerPreparation = compilerPreparation;
            _typePairList = typePairList;
        }

        public void GenerateSource()
        {
            var valueTuple = (fromType: _fromType, toType: _toType);
            if (_typePairList.Contains(valueTuple))
            {
                // _compilerPreparation.Context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(id: "Convertor004",
                //     title: $"duplicate {Constants.AttributeName} attribute",
                //     messageFormat: $"duplicate {Constants.AttributeName} attribute {{0}} --> {{1}}.",
                //     category: "MapMethodGenerator",
                //     DiagnosticSeverity.Warning,
                //     isEnabledByDefault: true), symbolInfo.Locations[0], fromType.Name, toType.Name));
                return;
            }

            _typePairList.Add(valueTuple);
            string fromTypeName = _fromType.GetTypeName();
            string destinationTypeName = _toType.GetTypeName();
            _sourceGenerator.AppendLine($"namespace {_symbolInfo.ContainingNamespace.ToDisplayString()}");
            _sourceGenerator.AppendLine("{", 1);
            _sourceGenerator.AppendLine(
                $"public {(_symbolInfo.IsStatic ? "static" : "")} partial class {_symbolInfo.Name}");
            _sourceGenerator.AppendLine("{", 1);
            _sourceGenerator.AppendLine(
                $"public {(_symbolInfo.IsStatic ? "static" : "")} void {Constants.MapMethodName}({fromTypeName} {Constants.InputParamName},out {destinationTypeName} {Constants.OutputParamName})");
            _sourceGenerator.AppendLine("{", 1);
            _sourceGenerator.AppendLine($"{Constants.OutputParamName}=new {destinationTypeName}();");

            foreach (var s in _fromType.GetMembers())
            {
                var memberName = s.Name;
                ImmutableArray<ISymbol> fromTypeMembers = _fromType.GetMembers(memberName);
                ImmutableArray<ISymbol> toTypeMembers = _toType.GetMembers(memberName);
                IPropertySymbol fromProperty = fromTypeMembers.OfType<IPropertySymbol>().FirstOrDefault(x =>
                    !x.IsWriteOnly && x.GetMethod?.DeclaredAccessibility == Accessibility.Public);
                IPropertySymbol toProperty = toTypeMembers.OfType<IPropertySymbol>().FirstOrDefault(x =>
                    !x.IsReadOnly && x.SetMethod?.DeclaredAccessibility == Accessibility.Public);
                if (fromProperty == null || toProperty == null)
                    continue;
                bool canTypeBeConverted =
                    this._compilerPreparation.Compilation.CanTypeBeConverted(fromProperty.Type, toProperty.Type);
                if (canTypeBeConverted)
                    _sourceGenerator.AppendLine(
                        $"{Constants.OutputParamName}.{memberName}={Constants.InputParamName}.{memberName};");
                else if (SymbolEqualityComparer.Default.Equals(fromProperty.Type.BaseType,
                    _compilerPreparation.Compilation.GetTypeByMetadataName("System.Enum")))
                {
                    _sourceGenerator.AppendLine(
                        $"{Constants.OutputParamName}.{memberName}=System.Enum.Parse<{toProperty.Type.GetTypeName()}>({Constants.InputParamName}.{memberName}.ToString());");

                }
                else
                {
                    new TypesMapGenerator(_symbolInfo, fromProperty.Type, toProperty.Type, _sourceGenerator.CreateSubSource(),
                        _compilerPreparation,_typePairList).GenerateSource();
                    string newGuid = "T" + Guid.NewGuid().ToString().Replace("-", "_");
                    _sourceGenerator.AppendLine($"{Constants.MapMethodName}({Constants.InputParamName}.{memberName},out var {newGuid});");
                    _sourceGenerator.AppendLine($"{Constants.OutputParamName}.{memberName}={newGuid};");
                }

            }

            _sourceGenerator.AppendLine("", -1);
            _sourceGenerator.AppendLine("}", -1);
            _sourceGenerator.AppendLine("}", -1);
            _sourceGenerator.AppendLine("}", -1);
            
        }
    }
}