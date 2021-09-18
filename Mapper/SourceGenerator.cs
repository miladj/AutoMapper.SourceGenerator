using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mapper
{
    public class SourceGenerator
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private List<SourceGenerator> _subSourceGenerators = new List<SourceGenerator>();
        private int _identCount = 0;
        
        public void AppendLine(string str,int indentChange=0)
        {
            _stringBuilder.Append('\t', _identCount);
            _stringBuilder.AppendLine(str);
            _identCount += indentChange;
        }

        public SourceGenerator CreateSubSource()
        {
            SourceGenerator sourceGenerator = new SourceGenerator();
            _subSourceGenerators.Add(sourceGenerator);
            return sourceGenerator;
        }
        public override string ToString()=>_stringBuilder.ToString()+string.Join("\r\n",_subSourceGenerators.Select(x=>x.ToString()));
        
    }
}