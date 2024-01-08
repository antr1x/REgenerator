using Newtonsoft.Json;

namespace REgenerator
{
    // Represents a namespace containing a list of functions.
    public class Namespace
    {
        /// <summary>
        /// The name of the namespace. This property is ignored during natives JSON deserialization
        /// as the namespace name is typically used as a key in the natives JSON structure rather than a property.
        /// </summary>
        [JsonIgnore]
        public string Name { get; set; } = null!;

        /// <summary>
        /// A list of Function objects. Each Function object represents a function contained within this namespace.
        /// </summary>
        /// <remarks>
        /// The Functions property stores all the function data deserialized from the natives JSON file for this specific namespace.
        /// Each Function object contains detailed information about a function, such as its name, parameters, return type, etc.
        /// </remarks>
        public List<Function> Functions { get; set; } = null!;
    }
}