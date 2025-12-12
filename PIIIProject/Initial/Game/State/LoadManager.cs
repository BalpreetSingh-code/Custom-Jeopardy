using PIIIProject.Initial.Models;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace PIIIProject.Initial.Game.State
{
    public static class LoadManager  // A static class is used because LoadManager does not maintain any instance-specific state, it provides utility functions that are globally accessible and do not require an instance.
    {

        // Path to the file where the game state is saved
        // Dynamically set to ensure portability across environments
        private static readonly string SaveFilePath;

        // Static constructor to initialize the SaveFilePath
        static LoadManager()
        {
            // Dynamically resolve the project root directory
            string projectRoot = PathHelper.GetProjectRootDirectory();

            // Set the save file path to the SaveData folder within the project root
            SaveFilePath = Path.Combine(projectRoot, "SaveData", "gameState.json");
        }

        /// <summary>
        /// Loads the game state and reconstructs the nested structure for questions
        /// </summary>
        /// <param name="questions">Output parameter to hold the reconstructed question structure</param>
        /// <returns>Loaded GameState object</returns>
        /// <exception cref="FileNotFoundException">Thrown if save file does not exist</exception>
        /// <exception cref="InvalidOperationException">Thrown if deserialization or reconstruction fails</exception>
        public static GameState LoadGameState(out Dictionary<string, Dictionary<string, List<Question>>> questions)
        {
            // Initialize the questions dictionary to hold reconstructed data
            questions = new Dictionary<string, Dictionary<string, List<Question>>>();

            // Check if the save file exists
            if (!File.Exists(SaveFilePath))
            {
                // Display an error message and throw an exception if the file is missing
                throw new FileNotFoundException("Save file not found.");
            }

            try
            {
                // Read the JSON file content
                string json = File.ReadAllText(SaveFilePath);

                // Deserialize the JSON content into a GameState object
                var gameState = JsonSerializer.Deserialize<GameState>(json);

                if (gameState == null)
                {
                    // Handle cases where the deserialized object is null
                    MessageBox.Show("Failed to load the game state. Game either does not exist or is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new InvalidOperationException("Failed to deserialize the game state.");
                }

                // Reconstruct the nested question structure from the combined data
                if (gameState.CustomCategories != null)
                {
                    foreach (var entry in gameState.CustomCategories)
                    {
                        // Split the key into category and subcategory
                        string[] splitKey = entry.Key.Split("::");
                        if (splitKey.Length != 2) continue; // Skip invalid keys

                        string category = splitKey[0];
                        string subCategory = splitKey[1];

                        // Initialize the category if it does not exist
                        if (!questions.ContainsKey(category))
                        {
                            questions[category] = new Dictionary<string, List<Question>>();
                        }

                        // Assign questions to the appropriate subcategory
                        questions[category][subCategory] = entry.Value;
                    }
                }

                return gameState; // Return the reconstructed game state
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                MessageBox.Show($"Error parsing the JSON file: {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new InvalidOperationException("Failed to parse the game state JSON.");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors during the load process
                MessageBox.Show($"Unexpected error loading the game state: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new InvalidOperationException("Failed to load the game.");
            }
        }
    }
}