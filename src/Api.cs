using System.Text;
using System.IO;
using System.Linq;

namespace REgenerator
{
    /// <summary>
    /// Responsible for generating Lua API scripts from deserialized natives JSON data.
    /// </summary>
    public class Api
    {
        /// <summary>
        /// Generates Lua API scripts based on the natives JSON data.
        /// </summary>
        /// <remarks>
        /// This method determines whether to create a single Lua file for all namespaces
        /// or separate Lua files for each namespace based on the user settings.
        /// </remarks>
        public static void Generate()
        {
            // Read JSON content from the file specified in settings
            var jsonContent = File.ReadAllText(Settings.NativeJsonPath);
            var jsonHolder = NativesDeserializer.Deserialize(jsonContent);

            // Create a directory for storing Lua files
            var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "natives");
            Directory.CreateDirectory(baseDirectory);

            // Check if the API should be generated in a single file
            if (Settings.GenerateSingleFile)
            {
                // Generate all namespaces in a single Lua file
                var filePath = Path.Combine(baseDirectory, "natives.lua");
                using var writer = new StreamWriter(filePath);

                foreach (var namespaceInfo in jsonHolder.Namespaces)
                {
                    var luaCode = GenerateNamespace(namespaceInfo);
                    writer.WriteLine(luaCode);
                }
            }
            else
            {
                // Generate each namespace in a separate Lua file
                foreach (var namespaceInfo in jsonHolder.Namespaces)
                {
                    var filePath = Path.Combine(baseDirectory, $"{namespaceInfo.Name}.lua");
                    using (var writer = new StreamWriter(filePath))
                    {
                        var luaCode = GenerateNamespace(namespaceInfo);
                        writer.WriteLine(luaCode);
                    }
                }
            }
        }

        /// <summary>
        /// Generates Lua code for a given namespace.
        /// </summary>
        /// <param name="namespaceInfo">Namespace information.</param>
        /// <returns>String containing Lua code for the namespace.</returns>
        private static string GenerateNamespace(Namespace namespaceInfo)
        {
            var luaCode = new StringBuilder();

            // Start the namespace declaration
            luaCode.AppendLine($"{namespaceInfo.Name} = {{}}\n");

            // Generate Lua functions for each function within the namespace
            foreach (var function in namespaceInfo.Functions)
            {
                var luaFunctionCode = GenerateLuaFunction(namespaceInfo.Name, function);
                luaCode.AppendLine(luaFunctionCode);
            }

            return luaCode.ToString();
        }

        /// <summary>
        /// Generates Lua function code for a given native function.
        /// This function takes a native function's information and converts it into a Lua function declaration.
        /// It handles parameter sanitization, type formatting, and special function marking.
        /// </summary>
        /// <param name="namespaceName">The namespace under which the function will be generated.</param>
        /// <param name="function">Object containing information about the native function.</param>
        /// <returns>A string representation of the Lua function.</returns>
        private static string GenerateLuaFunction(string namespaceName, Function function)
        {
            var luaFunction = new StringBuilder();

            // Determines if the function requires special handling (e.g., vector fixes).
            var isFunctionSpecial = Globals.FixVectorsFunctions.TryGetValue(function.Name, out int expectedId) && expectedId.ToString() == function.Id;

            // Adds the original C++ function signature as a comment for reference.
            if (Settings.GenerateCppSignature)
            {
                luaFunction.AppendLine($"-- {GetCppSignature(function)}");
            }

            // Adds any additional comments provided for the function.
            if (Settings.GenerateComments && !string.IsNullOrEmpty(function.Comment))
            {
                luaFunction.AppendLine($"--[[\n{function.Comment}\n--]]");
            }

            // Constructs the Lua function declaration with sanitized parameter names to avoid conflicts with Lua keywords.
            var paramList = string.Join(", ", function.Params.Select(p => SanitizeParamName(p.Name)));
            luaFunction.AppendLine($"function {namespaceName}.{function.Name}({paramList})");

            // Prepares the function's return statement based on its return type.
            var returnType = function.ReturnType == "void" ? "" : "return ";
            luaFunction.AppendLine($"  {returnType}native.invoke(");
            luaFunction.AppendLine($"    {GetFormattedType(function.ReturnType)}, {function.Id}, {isFunctionSpecial.ToString().ToLowerInvariant()}" + (function.Params.Any() ? "," : ""));

            // Adds each parameter to the function call, formatting and sanitizing as necessary.
            foreach (var param in function.Params)
            {
                var formattedType = GetFormattedType(param.Type.Contains("const char*") ? "string" : param.Type);
                var paramName = SanitizeParamName(param.Name);
                luaFunction.AppendLine($"    {(param.Type.Contains("*") ? "ref" : "arg")}({formattedType}, {paramName}),");
            }

            // Closes the function call and declaration.
            if (function.Params.Any())
                luaFunction.Remove(luaFunction.Length - 3, 1);
            luaFunction.AppendLine("  )");
            luaFunction.AppendLine("end");

            return luaFunction.ToString();
        }

        /// <summary>
        /// Generates the C++ signature of a function and formats it as a comment.
        /// This function converts the return type, function name, and parameters of a native function
        /// into a C++ function declaration, providing clarity on the original native function signature.
        /// Includes the native hash for easy reference.
        /// </summary>
        private static string GetCppSignature(Function function)
        {
            return $"{function.ReturnType} {function.Name}({string.Join(", ", function.Params.Select(p => $"{p.Type} {p.Name}"))}) // {function.Hash}";
        }

        /// <summary>
        /// Formats a C++ type into a Lua-compatible type string.
        /// This function takes a C++ type and converts it to the corresponding type used in Lua API.
        /// Removes pointer asterisks and adjusts the case to match Lua conventions.
        /// </summary>
        private static string GetFormattedType(string type)
        {
            type = type.Replace("*", "");
            return "Type." + char.ToUpper(type[0]) + type.Substring(1).ToLower();
        }

        /// <summary>
        /// Sanitizes parameter names to avoid conflicts with Lua reserved keywords.
        /// If a parameter name matches a Lua keyword, it appends an underscore to make it valid.
        /// Ensures that generated Lua functions do not contain syntax errors due to keyword misuse.
        /// </summary>
        private static string SanitizeParamName(string paramName)
        {
            return Globals.LuaKeywords.Contains(paramName) ? $"{paramName}_" : paramName;
        }
    }
}
