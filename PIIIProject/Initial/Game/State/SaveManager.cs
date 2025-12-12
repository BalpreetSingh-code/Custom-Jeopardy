using PIIIProject.Initial.Models;
using System.IO;
using System.Text.Json;
using System.Windows;


namespace PIIIProject.Initial.Game.State
{
    public static class SaveManager  // A static class is used because SaveManager does not need to store any instance-specific state, it provides utility methods that can be accessed globally without creating an object.
    {

        // Path to the file where the game state is saved
        // Dynamically set to ensure portability across environments
        private static readonly string SaveFilePath;

        // Static constructor to initialize the SaveFilePath
        static SaveManager()
        {
            // Dynamically resolve the project root directory
            string projectRoot = PathHelper.GetProjectRootDirectory();

            // Set the save file path to the SaveData folder within the project root
            SaveFilePath = Path.Combine(projectRoot, "SaveData", "gameState.json");
        }

        /// <summary>
        /// Saves current game state and associated questions to a JSON file
        /// </summary>
        /// <param name="gameState">Current game state to save</param>
        /// <param name="questions">Dictionary containing game questions categorized</param>
        public static void SaveGameState(GameState gameState, Dictionary<string, Dictionary<string, List<Question>>> questions)
        {
            try
            {
                // Combine categories and subcategories into a flat dictionary
                var combinedQuestions = new Dictionary<string, List<Question>>();
                foreach (var category in questions)
                {
                    foreach (var subCategory in category.Value)
                    {
                        // Create a key by combining category and subcategory names
                        string key = $"{category.Key}::{subCategory.Key}";
                        combinedQuestions[key] = subCategory.Value;
                    }
                }

                // Assign the combined questions to the game state
                gameState.CustomCategories = combinedQuestions;

                // Configure JSON serialization options
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Format JSON for readability
                };

                // Serialize the game state to JSON
                string json = JsonSerializer.Serialize(gameState, options);

                // Ensure the SaveData directory exists before saving
                string directory = Path.GetDirectoryName(SaveFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory); // Create the directory if it doesn't exist
                }

                // Write the JSON string to the save file
                File.WriteAllText(SaveFilePath, json);

                // Notify the user of successful save
                MessageBox.Show("Game saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors during the save process
                MessageBox.Show($"Error saving game state: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new InvalidOperationException("Failed to save the game state.");
            }
        }
    }
}
