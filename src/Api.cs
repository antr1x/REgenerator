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
        /// Converts a native function into its Lua equivalent.
        /// </summary>
        /// <param name="namespaceName">The namespace of the function.</param>
        /// <param name="function">The function to convert.</param>
        /// <returns>String containing the Lua function.</returns>
        private static string GenerateLuaFunction(string namespaceName, Function function)
        {
            var luaFunction = new StringBuilder();
            var isFunctionSpecial = Globals.FixVectorsFunctions.TryGetValue(function.Name, out int expectedId) && expectedId.ToString() == function.Id;

            // Include C++ signature and comments if enabled in settings
            if (Settings.GenerateCppSignature)
            {
                luaFunction.AppendLine($"-- {GetCppSignature(function)}");
            }
            if (Settings.GenerateComments && !string.IsNullOrEmpty(function.Comment))
            {
                luaFunction.AppendLine($"--[[\n{function.Comment}\n--]]");
            }

            // Construct the Lua function
            luaFunction.AppendLine($"function {namespaceName}.{function.Name}({string.Join(", ", function.Params.Select(p => p.Name))})");
            luaFunction.AppendLine($"  {(function.ReturnType == "void" ? "" : "return ")}native.invoke(");
            luaFunction.AppendLine($"    {GetFormattedType(function.ReturnType)}, {function.Id}, {isFunctionSpecial.ToString().ToLowerInvariant()}" + (function.Params.Any() ? "," : ""));

            // Add function parameters
            foreach (var param in function.Params)
            {
                var formattedType = GetFormattedType(param.Type.Contains("const char*") ? "string" : param.Type);
                luaFunction.AppendLine($"    {(param.Type.Contains("*") ? "ref" : "arg")}({formattedType}, {param.Name}),");
            }

            // Close the function body
            if (function.Params.Any())
                luaFunction.Remove(luaFunction.Length - 3, 1);
            luaFunction.AppendLine("  )");
            luaFunction.AppendLine("end");

            return luaFunction.ToString();
        }

        // Gets the C++ signature of the function as a comment string.
        private static string GetCppSignature(Function function)
        {
            return $"{function.ReturnType} {function.Name}({string.Join(", ", function.Params.Select(p => $"{p.Type} {p.Name}"))}) // {function.Hash}";
        }

        // Formats the type string for use in Lua function parameters or return type.
        private static string GetFormattedType(string type)
        {
            type = type.Replace("*", "");
            return "Type." + char.ToUpper(type[0]) + type.Substring(1).ToLower();
        }
    }
}
