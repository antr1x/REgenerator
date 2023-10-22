using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REgenerator
{
    // Represents a namespace containing a list of functions.
    internal class NamespaceInfo
    {
        public string NamespaceName { get; set; } = null!;
        public List<FunctionInfo> Functions { get; set; } = new();
    }

    // Represents a native function with its return type, function name, parameters, and hash.
    internal class FunctionInfo
    {
        public string ReturnType { get; set; } = null!;
        public string FunctionName { get; set; } = null!;
        public List<ParameterInfo> Parameters { get; set; } = null!;
        public string Hash { get; set; } = null!;
    }

    // Represents a function parameter with its type, name, and reference details.
    internal class ParameterInfo
    {
        public string Type { get; set; } = null!;
        public string ParamName { get; set; } = null!;
        public bool IsReference { get; set; }
    }
}
