using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PIIIProject.Initial.Game.State;
using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Game
{
    /// <summary>
    /// Interaction logic for GameBoardWindow.xaml
    /// </summary>
    public partial class GameBoardWindow : Window
    {
        // Player scores and turn tracking
        private int player1Score = 0; // Tracks Player 1's score
        private int player2Score = 0; // Tracks Player 2's score
        private bool player1Turn = true; // Keeps track of whose turn it is (Player 1 starts first)

        // Game mode and progress tracking
        private bool isSinglePlayer; // Determines if the game is in single-player mode
        private int totalQuestionsAnswered = 0; // Tracks the number of answered questions
        private int totalQuestions; // Total number of questions in the selected category

        // Player information
        private User _player1; // Represents Player 1
        private User? _player2; // Represents Player 2 (optional for single-player mode)
        private string _selectedCategory; // Stores the currently selected category
        private CustomCategory _customCategory;

        // Headers for game board columns, based on the selected category
        private readonly Dictionary<string, string[]> categoryHeaders = new()
        {
            { "ARTS", new[] { "Modern Art", "Famous Painters", "Art History", "Famous Sculptures" } },
            { "COMP SCI", new[] { "Prog Languages", "Algorithms", "OOP", "Computer History" } },
            { "MUSIC", new[] { "Famous Artists", "Famous Songs", "Rock Music", "Instruments" } },
            { "SPORTS", new[] { "Olympic Sports", "Team Sports", "Famous Athletes", "Sports Trivia" } }
        };

        // Dictionary to store questions organized by category and column
        private readonly Dictionary<string, Dictionary<string, List<Question>>> questions;

        /// <summary>
        /// Constructor for the GameBoardWindow.
        /// Initializes the game board based on the selected category and game mode.
        /// </summary>
        /// <param name="selectedCategory">The category selected by the user.</param>
        /// <param name="isSinglePlayer">Indicates if the game is single-player.</param>
        /// <param name="player1">Player 1's details.</param>
        /// <param name="player2">Player 2's details (if applicable).</param>
        public GameBoardWindow(string selectedCategory, bool isSinglePlayer, User player1, User player2)
        {
            InitializeComponent(); // Initialize UI components
            this.isSinglePlayer = isSinglePlayer; // Set the game mode
            _player1 = player1; // Assign Player 1
            _player2 = player2; // Assign Player 2
            _customCategory = null; // Initialize custom category as null

            try
            {
                if (string.IsNullOrEmpty(selectedCategory))
                {
                    MessageBox.Show("Selected category is null or empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentNullException(nameof(selectedCategory), "Selected category cannot be null or empty.");
                }

                if (categoryHeaders.ContainsKey(selectedCategory))
                {
                    _selectedCategory = selectedCategory;
                    questions = InitializeQuestions(selectedCategory);
                    CreateGameBoard(selectedCategory);
                }
                else
                {

                    GameState loadedGameState = LoadManager.LoadGameState(out var loadedQuestions);
                    _selectedCategory = selectedCategory;
                    questions = loadedQuestions;

                    if (!questions.ContainsKey(selectedCategory))
                    {
                        MessageBox.Show($"Custom category '{selectedCategory}' not found in saved game data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw new KeyNotFoundException($"Custom category '{selectedCategory}' does not exist in the saved game data.");
                    }

                    // Set scores from the loaded game state
                    player1Score = loadedGameState.Player1Score;
                    player2Score = loadedGameState.Player2Score;
                    UpdatePlayerLabels();

                    // Loop through all questions in the saved file, checking if `IsAnswered` is true, and incrementing `totalQuestionsAnswered` accordingly
                    foreach (var category in questions)
                    {
                        foreach (var column in category.Value)
                        {
                            totalQuestionsAnswered += column.Value.Count(questions => questions.IsAnswered);
                        }
                    }

                    totalQuestions = questions[_selectedCategory].Sum(column => column.Value.Count);

                    CreateGameBoard(selectedCategory);

                    if (totalQuestionsAnswered == totalQuestions)
                    {
                        ShowLeaderboard();
                    }

                    CreateGameBoard(selectedCategory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during game initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (isSinglePlayer)
            {
                CenterPlayer1Score();
            }

            UpdatePlayerLabels(); // Initialize player score labels
        }



        public GameBoardWindow(CustomCategory customCategory, bool isSinglePlayer, User player1, User player2)
        {
            InitializeComponent();
            this.isSinglePlayer = isSinglePlayer;
            _customCategory = customCategory;
            questions = InitializeQuestions();

            _player1 = player1;
            _player2 = player2;

            if (isSinglePlayer)
            {
                CenterPlayer1Score(); // Adjust UI layout for single-player mode
            }

            CreateGameBoard();
            _selectedCategory = customCategory.Name;
            totalQuestions = questions[_selectedCategory].Sum(column => column.Value.Count);


            UpdatePlayerLabels();
        }


        /// <summary>
        /// Updates the player score labels displayed on the game board.
        /// </summary>
        private void UpdatePlayerLabels()
        {
            // Set Player 1's score label
            Player1Score.Text = $"{_player1.Username}: ${player1Score}";

            // Set Player 2's score label (only for multiplayer mode)
            if (!isSinglePlayer && _player2 != null)
            {
                Player2Score.Text = $"{_player2.Username}: ${player2Score}";
            }
        }

        /// <summary>
        /// Adjusts the scoreboard layout for single-player mode.
        /// </summary>
        private void CenterPlayer1Score()
        {
            Player2Border.Visibility = Visibility.Collapsed; // Hide Player 2's scoreboard
            Grid.SetColumnSpan(Player1Border, 2); // Extend Player 1's scoreboard across two columns
            Player1Score.TextAlignment = TextAlignment.Center; // Center-align Player 1's score text
        }

        private void CreateGameBoard(string selectedCategory)
        {
            var categoryQuestions = questions[selectedCategory]; // Retrieve questions for the selected category
            int col = 0; // Column index for the grid

            // Loop through each column in the category
            foreach (var column in categoryQuestions)
            {
                // Create and add a column header
                TextBlock header = new TextBlock
                {
                    Text = column.Key, // Header text (e.g., "Modern Art")
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Background = Brushes.DarkBlue,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                };

                // Position the header in the grid
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, col);
                GameBoardGrid.Children.Add(header);

                // Add buttons for each question in the column
                for (int row = 0; row < column.Value.Count; row++)
                {
                    Question question = column.Value[row]; // Get the question
                    Button button = new Button
                    {
                        Content = $"${question.PointValue}", // Button text showing point value
                        Tag = new { Category = selectedCategory, Column = column.Key, Row = row }, // Attach metadata for event handling
                        FontSize = 30,
                        FontWeight = FontWeights.Bold,
                        Background = question.IsAnswered ? Brushes.White : Brushes.DarkBlue, // White if answered
                        Foreground = question.IsAnswered ? Brushes.LimeGreen : Brushes.LimeGreen, // Gray text if answered
                        Margin = new Thickness(5),
                        IsEnabled = !question.IsAnswered // Disable the button if the question is already answered
                    };

                    // Attach the click event handler
                    button.Click += QuestionButton_Click;

                    // Position the button in the grid
                    Grid.SetRow(button, row + 1);
                    Grid.SetColumn(button, col);
                    GameBoardGrid.Children.Add(button);
                }

                col++; // Move to the next column
            }
        }


        private void CreateGameBoard()
        {
            if (_customCategory == null)
            {
                MessageBox.Show("Custom category is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (!questions.ContainsKey(_customCategory.Name))
            {
                MessageBox.Show($"Questions for custom category '{_customCategory.Name}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            var categoryQuestions = questions[_customCategory.Name]; // Retrieve questions for the custom category
            int col = 0;

            foreach (var column in categoryQuestions)
            {
                // Create column headers and question buttons
                TextBlock header = new TextBlock
                {
                    Text = column.Key,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Background = Brushes.DarkBlue,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                };

                Grid.SetRow(header, 0);
                Grid.SetColumn(header, col);
                GameBoardGrid.Children.Add(header);

                for (int row = 0; row < column.Value.Count; row++)
                {
                    Button button = new Button
                    {
                        Content = $"${column.Value[row].PointValue}",
                        Tag = new { Category = _customCategory.Name, Column = column.Key, Row = row },
                        FontSize = 30,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.DarkBlue,
                        Foreground = Brushes.LimeGreen,
                        Margin = new Thickness(5)
                    };

                    button.Click += QuestionButton_Click;

                    Grid.SetRow(button, row + 1);
                    Grid.SetColumn(button, col);
                    GameBoardGrid.Children.Add(button);
                }

                col++;
            }
        }


        /// <summary>
        /// Event handler for question button clicks.
        /// Opens the question window and processes the result.
        /// </summary>
        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false; // Disable the button to prevent repeated clicks

                // Retrieve metadata from the button's Tag
                dynamic tag = button.Tag;
                string category = tag.Category;
                string column = tag.Column;
                int row = tag.Row;

                // Retrieve the selected question
                var selectedQuestion = questions[category][column][row];

                // Check if the question is already answered
                if (selectedQuestion.IsAnswered)
                {
                    MessageBox.Show("This question has already been answered.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Open the question window and pass the question for display
                QuestionWindow questionWindow = new QuestionWindow(selectedQuestion, HandleQuestionResult);
                questionWindow.ShowDialog();

                // Mark the question as answered
                selectedQuestion.IsAnswered = true;

                // Increment the count of answered questions
                totalQuestionsAnswered++;

                // Check if all questions are answered and show the leaderboard if so
                if (totalQuestionsAnswered == totalQuestions)
                {
                    ShowLeaderboard();
                }
            }
        }

        /// <summary>
        /// Processes the result of a question and updates scores.
        /// </summary>
        /// <param name="isCorrect">Indicates if the player's answer was correct.</param>
        /// <param name="points">The points earned or lost for the question.</param>
        private void HandleQuestionResult(bool isCorrect, int points)
        {
            if (isCorrect)
            {
                UpdateScore(points); // Add points for a correct answer
            }
        }

        /// <summary>
        /// Updates the player's score and switches turns in multiplayer mode.
        /// </summary>
        /// <param name="pointsEarned">The number of points earned by the player.</param>
        private void UpdateScore(int pointsEarned)
        {
            if (isSinglePlayer || player1Turn)
            {
                player1Score += pointsEarned; // Update Player 1's score
                Player1Score.Text = $"{_player1.Username}: ${player1Score}"; // Refresh Player 1's score label
            }
            else
            {
                player2Score += pointsEarned; // Update Player 2's score
                Player2Score.Text = $"{_player2.Username}: ${player2Score}"; // Refresh Player 2's score label
            }

            player1Turn = !player1Turn; // Switch turn for multiplayer mode
        }

        /// <summary>
        /// Displays the leaderboard at the end of the game.
        /// </summary>
        private void ShowLeaderboard()
        {
            // Open the leaderboard window and pass the final scores
            LeaderboardWindow leaderboardWindow = new LeaderboardWindow(isSinglePlayer, player1Score, player2Score, _player1, _player2);
            leaderboardWindow.ShowDialog();
            Close(); // Close the game board window after showing the leaderboard
        }
        private Dictionary<string, Dictionary<string, List<Question>>> InitializeQuestions(string selectedCategory)
        {
            var questions = new Dictionary<string, Dictionary<string, List<Question>>>();

            if (string.IsNullOrEmpty(selectedCategory))
            {
                throw new ArgumentNullException(nameof(selectedCategory), "Selected category cannot be null or empty.");
            }

            if (!categoryHeaders.ContainsKey(selectedCategory))
            {
                MessageBox.Show($"Category '{selectedCategory}' not found in categoryHeaders.", "Debug", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new KeyNotFoundException($"Category '{selectedCategory}' does not exist in predefined categories.");
            }

            // Handle predefined categories
            if (categoryHeaders.ContainsKey(selectedCategory))
            {
                questions[selectedCategory] = new Dictionary<string, List<Question>>
                {
                    [categoryHeaders[selectedCategory][0]] = new List<Question>
            {
                new Question("What movement did Pablo Picasso co-found?", new[] { "Cubism", "Impressionism", "Futurism", "Surrealism" }, 0, 200),
                new Question("Who is known for the 'Campbell's Soup Cans' artwork?", new[] { "Andy Warhol", "Jackson Pollock", "Mark Rothko", "Pablo Picasso" }, 0, 400),
                new Question("Which artist is famous for 'The Persistence of Memory'?", new[] { "Salvador Dalí", "Claude Monet", "Edvard Munch", "Henri Matisse" }, 0, 600),
                new Question("What is the primary style of Piet Mondrian's paintings?", new[] { "De Stijl", "Baroque", "Expressionism", "Pop Art" }, 0, 800),
            },
                    [categoryHeaders[selectedCategory][1]] = new List<Question>
            {
                new Question("Who painted the 'Mona Lisa'?", new[] { "Leonardo da Vinci", "Michelangelo", "Van Gogh", "Rembrandt" }, 0, 200),
                new Question("Which artist painted the 'Starry Night'?", new[] { "Vincent van Gogh", "Claude Monet", "Paul Cézanne", "Paul Gauguin" }, 0, 400),
                new Question("What painter is known for the 'Girl with a Pearl Earring'?", new[] { "Johannes Vermeer", "Caravaggio", "Francisco Goya", "Titian" }, 0, 600),
                new Question("Which artist painted 'The School of Athens'?", new[] { "Raphael", "Michelangelo", "Leonardo da Vinci", "Titian" }, 0, 800),
            },
                    [categoryHeaders[selectedCategory][2]] = new List<Question>
            {
                new Question("What ancient civilization is known for the pyramids?", new[] { "Egypt", "Greece", "Rome", "China" }, 0, 200),
                new Question("Which era is known for its dramatic use of light and shadow?", new[] { "Baroque", "Renaissance", "Impressionism", "Modernism" }, 0, 400),
                new Question("Who was the primary architect of the Parthenon?", new[] { "Phidias", "Ictinus", "Callicrates", "Vitruvius" }, 1, 600),
                new Question("What style of art is characterized by intricate designs and gold leaf?", new[] { "Byzantine", "Gothic", "Romanesque", "Neoclassical" }, 0, 800),
            },
                    [categoryHeaders[selectedCategory][3]] = new List<Question>
            {
                new Question("Who sculpted 'David'?", new[] { "Michelangelo", "Donatello", "Bernini", "Rodin" }, 0, 200),
                new Question("The 'Thinker' was sculpted by whom?", new[] { "Auguste Rodin", "Henry Moore", "Gian Lorenzo Bernini", "Donatello" }, 0, 400),
                new Question("What civilization created the Terracotta Army?", new[] { "China", "Japan", "Egypt", "India" }, 0, 600),
                new Question("Who sculpted the 'Pieta'?", new[] { "Michelangelo", "Bernini", "Canova", "Donatello" }, 0, 800),
            }
                };
            }
            else if (_customCategory != null && _customCategory.Name == selectedCategory)
            {
                questions[selectedCategory] = new Dictionary<string, List<Question>>();

                foreach (var subcategory in _customCategory.Types)
                {
                    if (string.IsNullOrEmpty(subcategory.Key))
                    {
                        MessageBox.Show($"Subcategory name in custom category '{_customCategory.Name}' is null or empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    if (subcategory.Value == null || !subcategory.Value.Any())
                    {
                        MessageBox.Show($"Subcategory '{subcategory.Key}' in custom category '{_customCategory.Name}' contains no questions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    questions[selectedCategory][subcategory.Key] = subcategory.Value;
                }
            }
            else
            {
                // If the category is not found in both predefined and custom categories
                MessageBox.Show($"Category '{selectedCategory}' does not exist in predefined or custom categories.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new ArgumentException($"Invalid category: {selectedCategory}");
            }

            return questions;
        }

        /// <summary>
        /// Initializes the dictionary containing all questions and their details.
        /// </summary>
        /// <returns>A dictionary of questions categorized by topics and columns.</returns>
        private Dictionary<string, Dictionary<string, List<Question>>> InitializeQuestions()
        {
            var questions = new Dictionary<string, Dictionary<string, List<Question>>>
            {
                [_customCategory.Name] = new Dictionary<string, List<Question>>(),
            };

            foreach (var type in _customCategory.Types)
            {
                questions[_customCategory.Name].Add(type.Key, type.Value);
            }
            return questions;
        }
        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GameState gameState = new GameState
                {
                    Player1Username = _player1.Username,
                    Player2Username = _player2?.Username,
                    Player1Score = player1Score,
                    Player2Score = player2Score,
                    SelectedCategory = _selectedCategory,
                    CustomCategories = null,
                    IsPlayer1Turn = player1Turn,
                    SaveTimestamp = DateTime.Now
                };

                SaveManager.SaveGameState(gameState, questions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save the game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}