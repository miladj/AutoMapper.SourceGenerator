using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Mapper
{
    public static class Extensions
    {
        public static string GetTypeName(this ITypeSymbol type)
        {
            return $"{type.ToDisplayString()}";
        }

        public static bool CanTypeBeConverted(this Compilation compilation,ITypeSymbol source,ITypeSymbol destination)
        {
            return SymbolEqualityComparer.Default.Equals(source, destination) || 
            compilation.HasImplicitConversion(source, destination);

        }
    }
}
