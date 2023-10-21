using System.Text.RegularExpressions;

namespace NativesGen
{
    internal class Program
    {
        private static readonly Regex RegexNamespace = new(@"namespace\s(\w+)(.*?)(?=\s*namespace|$)", RegexOptions.Singleline);
        private static readonly Regex RegexFunction = new(@"NATIVE_DECL\s+(.*?)\s+(\w+)\((.*?)\)\s*\{.*?invoke<.*?>\((0x\w+),", RegexOptions.Singleline);

        public static void Main(string[] args)
        {
            // Check if the program was executed without any command-line arguments.
            if (args.Length == 0)
            {
                // Prompt the user to drag and drop the natives file onto our super duper natives generator.
                Console.WriteLine("Please drag and drop the natives.hpp file onto the exe.");
                return;
            }

            // Take the first command-line argument as the path to the input natives file.
            var inputFilePath = args[0];

            // Construct the path for the output directory where the generated lua files will be saved.
            // This directory will be named "generated" and will be located in the same directory as the input file.
            var outputDirectoryPath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, "generated");

            // Create the "generated" directory if it doesn't exist.
            Directory.CreateDirectory(outputDirectoryPath);

            // Read the contents of the input natives file.
            var nativesData = File.ReadAllText(inputFilePath);

            // Parse the input data to extract information about namespaces and functions.
            var nativesNamespaces = ParseNatives(nativesData);

            // Iterate over each parsed natives file namespace.
            foreach (var ns in nativesNamespaces)
            {
                // Initialize the content for the Lua file with the namespace declaration.
                var luaContent = new List<string> { $"{ns.Name} = {{}}\n" };

                // Convert each function within the namespace into its Lua representation and add it to the content.
                luaContent.AddRange(ns.Functions.SelectMany(fn => ConvertFunctionToLua(fn, ns.Name)));

                // Add a return statement for the namespace at the end of the content.
                luaContent.Add($"return {ns.Name}");

                // Save the constructed Lua content to a file. The file is named after the namespace and saved in the "generated" directory.
                File.WriteAllLines($"{outputDirectoryPath}\\{ns.Name}.lua", luaContent);
            }
        }

        private static List<Natives.Namespace> ParseNatives(string data)
        {
            // Use the RegexNamespace pattern to find all namespace definitions in the data.
            return RegexNamespace.Matches(data)
                .Select(match =>
                {
                    // For each matched namespace, extract its name.
                    var namespaceName = match.Groups[1].Value;

                    // Within the namespace block, find all the function definitions using the RegexFunction pattern.
                    var functions = RegexFunction.Matches(match.Groups[2].Value)
                        .Cast<Match>()
                        .Select(m =>
                        {
                            // For each matched function, extract its return type, name, parameters, and hash.

                            // Return type of the function, e.g., "void".
                            var returnType = m.Groups[1].Value;

                            // Name of the function, e.g., "DoSomething".
                            var functionName = m.Groups[2].Value;

                            // Parameters of the function, split by comma and trimmed, e.g., ["int x", "float y"].
                            var functionParameters = m.Groups[3].Value.Split(',').Select(p => p.Trim()).ToList();

                            // Hash value associated with the function, e.g., "0x12345678".
                            var functionHash = m.Groups[4].Value;

                            return new Natives.Function
                            {
                                ReturnType = returnType,
                                Name = functionName,
                                Parameters = functionParameters,
                                Hash = functionHash
                            };
                        })
                        .ToList();

                    // Return the namespace along with its associated functions.
                    return new Natives.Namespace
                    {
                        Name = namespaceName,
                        Functions = functions
                    };
                })
                .ToList();
        }

        private static IEnumerable<string> ConvertFunctionToLua(Natives.Function fn, string namespaceName)
        {
            // Extract key data about the function for use in the Lua definition.
            var functionName = fn.Name;
            var functionHash = fn.Hash;
            var returnTypeOriginal = fn.ReturnType;

            // Convert C++ data type to the corresponding Lua data type.
            var returnTypeLua = ConvertType(returnTypeOriginal);

            // Parse and extract individual parameter types and names.
            var parameterTypesAndNames = fn.Parameters.Select(ParseParameter).ToList();

            // Extract only the names of the parameters for our Lua function definition.
            var parameterNamesList = parameterTypesAndNames.Select(p => p.Name).ToList();

            // Convert the parameter names into a comma-separated string.
            var parametersString = string.Join(", ", parameterNamesList);

            // Check if the function has any parameters.
            var hasParameters = parameterNamesList.Any(p => !string.IsNullOrWhiteSpace(p));

            // If the function's return type in Lua is "Void", we won't add a "return" statement in Lua.
            // Otherwise, we prefix the function call with "return".
            var returnStatementPrefix = returnTypeLua == "Void" ? "" : "return ";

            // Depending on whether the function has parameters, we format our invocation arguments accordingly.
            var invokeArgs = hasParameters
                ? $"{returnTypeLua}, {functionHash}, {parametersString}"
                : $"{returnTypeLua}, {functionHash}";

            // Generate Lua function definition lines.
            // Comment line showing the original C++ function signature and its hash.
            yield return $"-- {returnTypeOriginal} {functionName}({parametersString}) // {functionHash}";
            // Lua function declaration line.
            yield return $"function {namespaceName}.{functionName}({parametersString})";
            // Lua function body, where the native function is invoked.
            yield return $"  {returnStatementPrefix}native.invoke(ValueType.{invokeArgs})";
            // End of Lua function.
            yield return "end\n";
        }

        private static (string Type, string Name) ParseParameter(string parameter)
        {
            // Split the parameter string by space. 
            // In most cases, parameters will be in the format "type name", e.g., "int number".
            var split = parameter.Split(' ');

            // Depending on the number of words in the split:
            // 1 word: It's just the type, e.g., "int".
            // 2 words: It's a simple type followed by a name, e.g., "int number".
            // 3 words: It's a composite type followed by a name, e.g., "unsigned int number".
            var type = split.Length >= 2 ? $"{split[0]} {split[1]}" : split[0];
            var name = split.Length == 3 ? split[2] : (split.Length >= 2 ? split[1] : "");

            // Convert the C++ type to the corresponding Lua type and return along with the parameter name.
            return (ConvertType(type), name);
        }

        private static string ConvertType(string type) => Types.Mapper.TryGetValue(type, out var mappedType) ? mappedType : type;
    }
}