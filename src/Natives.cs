using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativesGen
{
    public class Natives
    {
        public class Function
        {
            public string ReturnType { get; set; } = null!;
            public string Name { get; set; } = null!;
            public List<string> Parameters { get; set; } = null!;
            public string Hash { get; set; } = null!;
        }

        public class Namespace
        {
            public string Name { get; set; } = null!;
            public List<Function> Functions { get; set; } = null!;
        }
    }
}