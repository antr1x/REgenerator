namespace REgenerator
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Responsible for deserializing natives JSON content into a structured C# object.
    /// </summary>
    public class NativesDeserializer
    {
        /// <summary>
        /// Deserializes the natives JSON string into a structured JsonHolder object.
        /// </summary>
        /// <param name="jsonContent">The natives JSON content as a string.</param>
        /// <returns>A JsonHolder object representing the structured data.</returns>
        /// <remarks>
        /// This method converts the raw natives JSON data into a structured format that includes
        /// namespaces and functions, making it easier to process and generate the Lua API.
        /// </remarks>
        public static JsonHolder Deserialize(string jsonContent)
        {
            // Convert the raw JSON into a nested dictionary structure
            var rawHolder = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Function>>>(jsonContent);
            var holder = new JsonHolder
            {
                Namespaces = new List<Namespace>()
            };

            var functionId = 0; // Counter for assigning unique IDs to each function

            // Process each namespace and its functions in the JSON data
            foreach (var ns in rawHolder!)
            {
                var namespaceItem = new Namespace
                {
                    Name = ns.Key, // The name of the namespace
                    Functions = new List<Function>() // Preparing to store functions
                };

                // Process each function within the namespace
                foreach (var fn in ns.Value)
                {
                    fn.Value.Hash = fn.Key; // Assign the function's hash value
                    fn.Value.Id = functionId++.ToString(); // Assign and increment the function ID

                    // Add the function to the current namespace
                    namespaceItem.Functions.Add(fn.Value);
                }

                // Add the processed namespace to the holder
                holder.Namespaces.Add(namespaceItem);
            }

            return holder; // Return the fully structured data
        }
    }
}