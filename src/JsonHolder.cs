namespace REgenerator
{
    // Represents a container for holding deserialized JSON data.
    public class JsonHolder
    {
        /// <summary>
        /// A list of Namespace objects, each representing a namespace found in the natives JSON file.
        /// </summary>
        /// <remarks>
        /// The Namespaces property is used to store all the namespace data deserialized from
        /// the natives JSON file. Each Namespace object within this list contains a collection of 
        /// functions and their related data, as defined in the original JSON structure.
        /// </remarks>
        public List<Namespace> Namespaces { get; set; } = null!;
    }
}