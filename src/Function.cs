using Newtonsoft.Json;
using System.Collections.Generic;

namespace REgenerator
{
    /// <summary>
    /// Represents a single function as defined in the natives JSON file.
    /// </summary>
    public class Function
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The Jenkins hash of the function, used as an identifier.
        /// </summary>
        [JsonProperty("jhash")]
        public string Jhash { get; set; } = null!;

        /// <summary>
        /// Any comments associated with the function, often used for documentation or explanatory notes.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; } = null!;

        /// <summary>
        /// Parameters that the function accepts. Stored as a list of FunctionParam objects.
        /// Each FunctionParam object represents a parameter with its type and name.
        /// </summary>
        [JsonProperty("params")]
        public List<FunctionParam> Params { get; set; } = null!;

        /// <summary>
        /// The return type of the function, such as 'void', 'int', etc.
        /// </summary>
        [JsonProperty("return_type")]
        public string ReturnType { get; set; } = null!;

        /// <summary>
        /// The build version or other relevant information associated with the function.
        /// </summary>
        [JsonProperty("build")]
        public string Build { get; set; } = null!;

        /// <summary>
        /// A unique identifier for the function, assigned during processing and not part of the original JSON.
        /// This ID is used internally for API generation.
        /// </summary>
        [JsonIgnore]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The hash key for the function, serving as an identifier in the original JSON data.
        /// This property is populated based on the key in the JSON file.
        /// </summary>
        [JsonIgnore]
        public string Hash { get; set; } = null!;
    }
}