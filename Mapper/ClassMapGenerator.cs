using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Mapper
{
    public class ClassMapGenerator
    {
        private readonly ISymbol _symbolInfo;
        private readonly CompilerPreparation _compilerPreparation;
        private readonly HashSet<(ITypeSymbol, ITypeSymbol)> _typePairList = new HashSet<(ITypeSymbol, ITypeSymbol)>();
        private readonly SourceGenerator _sourceGenerator= new SourceGenerator();
        public ClassMapGenerator(ISymbol symbol,CompilerPreparation compilerPreparation)
        {
            _symbolInfo = symbol;
            _compilerPreparation = compilerPreparation;
            
        }
        public void CreateConvertMethod(ITypeSymbol fromType, ITypeSymbol toType)
        {

            new TypesMapGenerator(_symbolInfo, fromType, toType, _sourceGenerator, _compilerPreparation, _typePairList).GenerateSource();

        }

        public override string ToString() => _sourceGenerator.ToString();
    }
}