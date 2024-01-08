namespace REgenerator
{
    // Holds configuration settings for the Lua API generation process.
    public class Settings
    {
        /// <summary>
        /// Indicates whether to generate comments in the Lua API.
        /// If true, comments are included in the generated Lua code.
        /// </summary>
        public static bool GenerateComments;

        /// <summary>
        /// Determines if the API should be generated in a single Lua file.
        /// If true, all namespaces and functions are combined into one file.
        /// If false, each namespace is generated in a separate Lua file.
        /// </summary>
        public static bool GenerateSingleFile;

        /// <summary>
        /// Indicates whether to include C++ signatures as comments in the Lua API.
        /// If true, the original C++ function signatures are included as comments.
        /// </summary>
        public static bool GenerateCppSignature;

        /// <summary>
        /// Path to the 'natives.json' file used for generating the Lua API.
        /// This path is set based on the location of the 'natives.json' file,
        /// whether it's a local file or cloned from a Git repository.
        /// </summary>
        public static string NativeJsonPath = null!;
    }
}