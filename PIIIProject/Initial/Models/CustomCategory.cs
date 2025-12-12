
namespace PIIIProject.Initial.Models
{
    public class CustomCategory
    {
        /// <summary>
        /// Gets or sets the name of the custom category
        /// </summary>
        public string Name { get; set; } // Category name

        /// <summary>
        /// Gets or sets the username of the creator of the custom category
        /// Null if no username is associated
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the types of questions in the category
        /// Each type maps to a list of questions
        /// </summary>
        public Dictionary<string, List<Question>> Types { get; set; } = new Dictionary<string, List<Question>>();

        /// <summary>
        /// Initializes a new instance of the CustomCategory class with the specified name
        /// </summary>
        /// <param name="name">The name of the custom category</param>
        public CustomCategory(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Validates the custom category to ensure that:
        /// - No two questions in the same type have the same point value
        /// - Each type contains exactly 4 unique point values (e.g., 200, 400, 600, 800)
        /// </summary>
        /// <returns>True if the category is valid otherwise false</returns>
        public bool IsValid()
        {
            foreach (var type in Types.Values)
            {
                var pointValues = new HashSet<int>(); // Tracks unique point values in the current type
                foreach (var question in type)
                {
                    if (!pointValues.Add(question.PointValue)) return false; // Duplicate point value found
                }

                if (pointValues.Count != 4) return false; // Ensure exactly 4 unique point values
            }
            return true;
        }
    }
}
