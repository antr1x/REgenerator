using System.Text;
using System.Text.RegularExpressions;

namespace REgenerator
{
    // Represents the main program for processing native functions and converting them to Lua format.
    public class Program
    {
        public static void Main(string[] args)
        {
            // Check if any command-line argument is provided.
            // If not, display a message asking for the user to drag and drop a natives file onto the executable.
            if (args.Length == 0)
            {
                Console.WriteLine("Please drag and drop the natives.hpp file onto the exe.");
                return;
            }

            // Read the content of the file provided as the command-line argument.
            var inputFilePath = args[0];
            var input = File.ReadAllText(inputFilePath);

            // Define a Regex pattern to match C++ namespaces and their content.
            // This pattern looks for the keyword "namespace", followed by a name (all uppercase), and then captures everything between curly braces.
            var patternNamespace = @"namespace\s+([A-Z]+)\s*\{([\s\S]*?)^\}";
            var namespaceMatches = Regex.Matches(input, patternNamespace, RegexOptions.Multiline);

            var namespaceInfos = new List<NamespaceInfo>();

            // For each matched namespace, extract its name and content.
            foreach (Match namespaceMatch in namespaceMatches)
            {
                var namespaceName = namespaceMatch.Groups[1].Value;
                var contentBetweenBraces = namespaceMatch.Groups[2].Value.Replace("const char*", "String");

                // Extract function details within the captured namespace content.
                NamespaceInfo namespaceInfo = new NamespaceInfo
                {
                    NamespaceName = namespaceName,
                    Functions = ExtractFunctions(contentBetweenBraces)
                };

                namespaceInfos.Add(namespaceInfo);
            }

            // For each extracted namespace, generate its corresponding Lua file.
            foreach (var namespaceInfo in namespaceInfos)
            {
                GenerateLuaFile(namespaceInfo);
            }
        }

        // This function is responsible for generating a Lua file based on the provided namespace and its associated functions.
        private static void GenerateLuaFile(NamespaceInfo namespaceInfo)
        {
            // Define a constant for the directory path where the Lua files will be saved.
            const string path = "natives";

            // Construct the complete path to the Lua file based on the namespace name, like "natives\NAMESPACE_NAME.lua"
            var filePath = $"{path}\\{namespaceInfo.NamespaceName}.lua";

            // Ensure the directory exists. If it doesn't, it will create it.
            Directory.CreateDirectory(path);

            // Open a StreamWriter to write to the file.
            using var writer = new StreamWriter(filePath);

            // Initialize the Lua table with the name of the namespace.
            // For instance, if the namespace is "ENTITY", it will write "ENTITY = {}" to the file.
            writer.WriteLine($"{namespaceInfo.NamespaceName} = {{}}\n");

            // For each function within the namespace, convert it to its Lua format using the GenerateLuaFunction method.
            // Then, write the resulting Lua function to the file.
            foreach (var functionInfo in namespaceInfo.Functions)
            {
                string luaFunction = GenerateLuaFunction(namespaceInfo.NamespaceName, functionInfo);
                writer.WriteLine(luaFunction);
            }

            // Conclude the file by returning the Lua table (which now contains all the functions).
            // This makes it possible to require this Lua file from other scripts and use its functions.
            writer.WriteLine($"return {namespaceInfo.NamespaceName}");
        }

        // This function is designed to convert a native function (in C++ format or similar) into its Lua equivalent.
        private static string GenerateLuaFunction(string namespaceName, FunctionInfo functionInfo)
        {
            var luaFunction = new StringBuilder();

            // This line generates a comment line containing the original signature of the function in C++ format.
            // The comment will look like: 
            // -- int SOME_FUNCTION(int param1, const char* param2) // 0xHASH
            var originalSignature = $"{functionInfo.ReturnType} {functionInfo.FunctionName}({string.Join(", ", functionInfo.Parameters.Select(p => (p.Type == "String" ? "const char*" : p.Type) + " " + p.ParamName))}) // {functionInfo.Hash}";
            luaFunction.AppendLine($"-- {originalSignature}");

            // Determines if the function has a return value or not.
            // If it's a 'void' function, then there is no 'return' keyword, otherwise it will prepend 'return' to the function call.
            var returnType = functionInfo.ReturnType == "void" ? "" : "return ";
            var luaReturnType = GetFormattedType(functionInfo.ReturnType);

            // Start constructing the Lua function.
            // The Lua function will have a name that is a combination of the namespace and the original function name.
            // For instance, if the namespace is "NAMESPACE" and function is "SOME_FUNCTION", the Lua function will be called "NAMESPACE.SOME_FUNCTION".
            luaFunction.AppendLine($"function {namespaceName}.{functionInfo.FunctionName}({string.Join(", ", functionInfo.Parameters.Select(p => p.ParamName))})");
            luaFunction.AppendLine($"  {returnType}native.invoke(");
            luaFunction.AppendLine($"    {luaReturnType},");
            luaFunction.AppendLine($"    {functionInfo.Hash}" + (functionInfo.Parameters.Count > 0 ? "," : ""));

            // Here, the arguments for the 'native.invoke' Lua function are being constructed.
            // For each parameter in the original function, it determines the type and whether the parameter is passed by reference or value.
            for (var i = 0; i < functionInfo.Parameters.Count; i++)
            {
                var param = functionInfo.Parameters[i];
                var paramType = GetFormattedType(param.Type);
                var refOrArg = param.IsReference ? "ref" : "arg"; // If the parameter is a reference type, it's passed as "ref", otherwise as "arg".

                // Constructing the argument. The last argument doesn't need a comma at the end.
                if (i == functionInfo.Parameters.Count - 1)
                    luaFunction.AppendLine($"    {refOrArg}({paramType}, {param.ParamName})");
                else
                    luaFunction.AppendLine($"    {refOrArg}({paramType}, {param.ParamName}),");
            }

            // Closing function body
            luaFunction.AppendLine("  )");
            luaFunction.AppendLine("end");

            return luaFunction.ToString();
        }

        // Extracts all native function details from the given input.
        private static List<FunctionInfo> ExtractFunctions(string input)
        {
            // This regular expression pattern is designed to match native function declarations. 
            // It captures the return type, function name, its parameters, and a hash.
            // Here's a breakdown of each capture:
            // - returnType: captures the data type of the function's return value.
            // - functionName: captures the name of the function.
            // - parameters: captures everything inside the parentheses, essentially the parameters of the function.
            // - hash: captures the hash value associated with the function, which appears as a comment after the function declaration.
            var patternFunction = @"NATIVE_DECL\s+(?<returnType>\w+)\s+(?<functionName>\w+)\s*\((?<parameters>[^)]*)\)\s*{.*?}\s*\/\/\s*(?<hash>0x[0-9A-Fa-f]+)\s*";
            var functionMatches = Regex.Matches(input, patternFunction, RegexOptions.Multiline);

            var functionList = new List<FunctionInfo>();

            // For each matched function, extract the details using the named capture groups in the regex pattern.
            foreach (Match match in functionMatches)
            {
                // Extracting the return type of the function.
                var returnType = match.Groups["returnType"].Value;

                // Extracting the name of the function.
                var functionName = match.Groups["functionName"].Value;

                // Extracting the parameters of the function as a string and splitting it into individual parameters.
                var parameters = match.Groups["parameters"].Value;
                var parameterArray = parameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // For each parameter, split it to capture its type and name.
                // Also, determine if the parameter is a reference type (contains an asterisk '*').
                var parameterInfoList = new List<ParameterInfo>();

                foreach (string param in parameterArray)
                {
                    var paramParts = param.Trim().Split(new char[] { ' ' }, 2);
                    var parameterInfo = new ParameterInfo
                    {
                        Type = paramParts[0],
                        ParamName = paramParts[1],
                        IsReference = paramParts[0].Contains("*")
                    };
                    parameterInfoList.Add(parameterInfo);
                }

                // Extracting the hash value associated with the function.
                var hash = match.Groups["hash"].Value;

                // Constructing a FunctionInfo object with the extracted details.
                var functionInfo = new FunctionInfo
                {
                    ReturnType = returnType,
                    FunctionName = functionName,
                    Parameters = parameterInfoList,
                    Hash = hash
                };

                functionList.Add(functionInfo);
            }

            return functionList;
        }

        // Converts C++ types into a more readable format for Lua.
        static string GetFormattedType(string type)
        {
            // Simple function to remove * from the function type and return it with Type. prefix
            type = type.Replace("*", "");
            return "Type." + char.ToUpper(type[0]) + type.Substring(1).ToLower();
        }
    }
}