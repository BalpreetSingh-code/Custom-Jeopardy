using System.IO;
using System.Text.Json;
using PIIIProject.Initial.Models;
using System.Text.Json.Serialization;
using System.Windows;

namespace PIIIProject.Initial.Game.Custom
{
    public class CustomCategoryService
    {
        private readonly string _dataPath; // Path to the base file for storing custom categories

        /// <summary>
        /// Initializes the CustomCategoryService with a dynamic data path for storing categories
        /// </summary>
        public CustomCategoryService()
        {
            // Resolve the project root dynamically
            string projectRoot = PathHelper.GetProjectRootDirectory();

            // Set the base data path within the project directory
            string dataFolder = Path.Combine(projectRoot, "CustomData");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder); // Ensure the directory exists
            }

            _dataPath = Path.Combine(dataFolder, "CustomCategories.json"); // Default file path
        }

        /// <summary>
        /// Loads custom categories for a specific user
        /// </summary>
        /// <param name="username">Username to load categories for</param>
        /// <returns>List of valid custom categories</returns>
        public List<CustomCategory> LoadCustomCategoriesForUser(string username)
        {
            string userCategoryPath = _dataPath.Replace(".json", $"_{username}.json"); // Adjust file path for the user

            if (!File.Exists(userCategoryPath))
            {
                return new List<CustomCategory>();
            }

            try
            {
                string jsonString = File.ReadAllText(userCategoryPath);

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    MessageBox.Show($"Custom category file for user '{username}' is empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<CustomCategory>();
                }

                var categories = JsonSerializer.Deserialize<List<CustomCategory>>(jsonString, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                });

                if (categories == null || !categories.Any())
                {
                    MessageBox.Show($"No valid custom categories found for user '{username}'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<CustomCategory>();
                }

                List<CustomCategory> validCategories = new List<CustomCategory>();
                foreach (CustomCategory category in categories)
                {
                    if (string.IsNullOrWhiteSpace(category.Name))
                    {
                        MessageBox.Show("A custom category has an invalid or empty name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    if (category.Types == null || !category.Types.Any())
                    {
                        MessageBox.Show($"Custom category '{category.Name}' contains no subcategories.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    foreach (var subcategory in category.Types)
                    {
                        if (string.IsNullOrWhiteSpace(subcategory.Key))
                        {
                            MessageBox.Show($"Subcategory in '{category.Name}' has an invalid or empty name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            continue;
                        }

                        if (subcategory.Value == null || !subcategory.Value.Any())
                        {
                            MessageBox.Show($"Subcategory '{subcategory.Key}' in category '{category.Name}' contains no questions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    validCategories.Add(category);
                }

                return validCategories; // Returns only valid categories
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Error parsing custom category file for user '{username}': {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CustomCategory>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error loading custom categories for user '{username}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CustomCategory>();
            }
        }

        /// <summary>
        /// Saves custom categories for a specific user
        /// </summary>
        /// <param name="username">Username to save categories for</param>
        /// <param name="customCategories">List of custom categories to save</param>
        public void SaveCustomCategoriesForUser(string username, List<CustomCategory> customCategories)
        {
            string userCategoryPath = _dataPath.Replace(".json", $"_{username}.json"); // Adjust file path for the user

            foreach (CustomCategory category in customCategories)
            {
                category.Username = username; // Assign the username to each category
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Formats JSON for readability
            };

            try
            {
                string jsonString = JsonSerializer.Serialize(customCategories, options);

                // Save to the JSON file
                File.WriteAllText(userCategoryPath, jsonString);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving custom categories: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
