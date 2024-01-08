namespace REgenerator
{
    /// <summary>
    /// Represents a single parameter of a function in the API.
    /// This class is used to store information about each parameter
    /// of a function as defined in the JSON file.
    /// </summary>
    public class FunctionParam
    {
        /// <summary>
        /// The data type of the parameter.
        /// This property holds the type as specified in the JSON file,
        /// such as 'int', 'string', etc.
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// The name of the parameter.
        /// This is the identifier used in the function's signature
        /// and is directly derived from the JSON file.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}