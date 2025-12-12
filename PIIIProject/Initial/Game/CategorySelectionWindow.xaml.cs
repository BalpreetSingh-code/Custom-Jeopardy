using PIIIProject.Initial.Auth.Services;
using PIIIProject.Initial.Game.Custom;
using PIIIProject.Initial.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PIIIProject.Initial.Game.State;

namespace PIIIProject.Initial.Game
{
    public partial class CategorySelectionWindow : Window
    {
        // Predefined categories for the game
        // Marked as readonly since these categories should not be changed during runtime
        private readonly string[] predefinedCategories = { "ARTS", "COMP SCI", "SPORTS", "MUSIC" };

        private readonly AuthenticationService _authenticationService; // Service for authentication
        private readonly User _player1; // First player of the game
        private readonly CustomCategoryService _customCategoryService; // Service to handle custom categories

        /// <summary>
        /// Constructor for CategorySelectionWindow, initializes predfefined and custom categories and creates buttons for selection
        /// </summary>
        /// <param name="player1">First player</param>
        /// <param name="authenticationService">Service to manage user authentication</param>
        public CategorySelectionWindow(User player1, AuthenticationService authenticationService)
        {
            InitializeComponent();
            _player1 = player1;
            _authenticationService = authenticationService;

            // Initializing the custom category service with a predefined storage file
            _customCategoryService = new CustomCategoryService();

            // Load custom categories for the current player
            var customCategories = _customCategoryService.LoadCustomCategoriesForUser(_player1.Username);

            // Combine predefined and custom categories for display
            string[] allCategories = predefinedCategories.Concat(customCategories.Select(category => category.Name)).ToArray();
            CreateCategoryButtons(allCategories);
        }

        /// <summary>
        /// Dynamically creates buttons for each category and adds them to the grid
        /// </summary>
        /// <param name="categories">Array of category names</param>
        private void CreateCategoryButtons(string[] categories)
        {
            Console.WriteLine($"Creating buttons for categories: {string.Join(", ", categories)}");

            CategoryGrid.Children.Clear();
            CategoryGrid.RowDefinitions.Clear();
            CategoryGrid.ColumnDefinitions.Clear();

            int numberOfColumns = 2; 
            int numberOfRows = (int)Math.Ceiling((double)categories.Length / numberOfColumns);

            // Create rows and columns dynamically based on the number of categories
            for (int i = 0; i < numberOfRows; i++)
            {
                CategoryGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < numberOfColumns; i++)
            {
                CategoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Create a button for each category and place it in the grid
            for (int i = 0; i < categories.Length; i++)
            {
                string category = categories[i];

                Button button = new Button
                {
                    Content = category,
                    Background = new SolidColorBrush(Color.FromRgb(30, 144, 255)),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(2),
                    FontSize = 24,
                    Padding = new Thickness(20),
                    Tag = category,
                    Margin = new Thickness(10)
                };

                button.Click += CategoryButton_Click; // Attach click event handler

                int row = i / numberOfColumns;
                int column = i % numberOfColumns;

                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);
                CategoryGrid.Children.Add(button);
            }
        }

        /// <summary>
        /// Handles the click event for category button
        /// Determines if the selected category is predefined or custom and navigates accordingly
        /// </summary>
        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string categoryName)
            {
                try
                {
                    if (predefinedCategories.Contains(categoryName))
                    {
                        // Open GameSetupWindow for predefined category
                        GameSetupWindow gameSetupWindow = new GameSetupWindow(categoryName, _authenticationService, _player1);
                        gameSetupWindow.Show();
                    }
                    else
                    {
                        // Load custom category based on the selected category name
                        CustomCategory selectedCustomCategory = _customCategoryService
                            .LoadCustomCategoriesForUser(_player1.Username)
                            .FirstOrDefault(category => category.Name == categoryName);

                        if (selectedCustomCategory != null)
                        {
                            // Opens GameSetup for custom category
                            GameSetupWindow gameSetupWindow = new GameSetupWindow(selectedCustomCategory, _authenticationService, _player1);
                            gameSetupWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show($"Invalid category: '{categoryName}'. Make sure it's correctly defined.",
                                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while processing the category '{categoryName}': {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    this.Close(); // Close current window after selection
                }
            }
        }

        /// <summary>
        /// Opens a window to add a new custom category and updates the displayed categories
        /// </summary>
        private void AddCustomCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            CustomCategoriesWindow customCategoriesWindow = new CustomCategoriesWindow(_player1, _customCategoryService);

            customCategoriesWindow.ShowDialog();

            string[] allCategories = predefinedCategories.Concat(_customCategoryService.LoadCustomCategoriesForUser(_player1.Username).Select(category => category.Name)).ToArray();
            CreateCategoryButtons(allCategories);
        }

        /// <summary>
        /// Loads a saved game state and navigates to the game board
        /// </summary>
        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load game state from the save manager
                Dictionary<string, Dictionary<string, List<Question>>> loadedQuestions;
                GameState gameState = LoadManager.LoadGameState(out loadedQuestions);

                // Restore game state
                User player1 = new User(gameState.Player1Username, "", "", gameState.Player1Score);
                User? player2 = string.IsNullOrWhiteSpace(gameState.Player2Username)
                    ? null
                    : new User(gameState.Player2Username, "", "", gameState.Player2Score);

                // Open GameBoardWindow with the loaded game state
                GameBoardWindow gameBoardWindow = new GameBoardWindow(
                    gameState.SelectedCategory,
                    player2 == null, // Determine single-player mode
                    player1,
                    player2
                );

                gameBoardWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load the game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
