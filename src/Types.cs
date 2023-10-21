using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativesGen
{
    internal class Types
    {
        public static readonly Dictionary<string, string> Mapper = new()
        {
            { "void", "Void" },
            { "int", "Int" },
            { "float", "Float" },
            { "bool", "Bool" },
            { "BOOL", "Bool" },
            { "const char*", "String" }
        };
    }
}