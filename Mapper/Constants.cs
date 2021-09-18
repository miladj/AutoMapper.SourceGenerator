using System;
using System.Collections.Generic;
using System.Text;

namespace Mapper
{
    public class Constants
    {
        public const string Namespace = nameof(Mapper);
        public const string AttributeName = "Map";
        public const string InputParamName = "inputObj";
        public const string OutputParamName = "outputObj";
        public const string AttributeSourceName = "Mapper.Attribute.src";
        public const string MapperSourceName = "Mapper.Convertor.src";
        public const string MapMethodName = "Convert";
        public static string AttributeSource = $@"
using System;

namespace {Namespace}
{{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class {AttributeName}Attribute:Attribute
    {{
        private readonly Type _from;
        private readonly Type _to;

        public {AttributeName}Attribute(Type from,Type to)
        {{
            _from = @from;
            _to = to;
        }}
    }}    
}}
";
    }
}
