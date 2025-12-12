using System.Windows;
using PIIIProject.Initial.Auth;
using PIIIProject.Initial.Auth.Services;
using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Game
{
    /// <summary>
    /// Interaction logic for the GameSetupWindow.xaml
    /// </summary>
    public partial class GameSetupWindow : Window
    {
        // Private fields to store the selected category, custom category, and authentication service instance
        private readonly string? _selectedCategory; // Stores the name of a predefined category
        private readonly CustomCategory? _selectedCustomCategory; // Stores the details of a custom category
        private readonly AuthenticationService _authenticationService; // Handles user authentication

        // Private fields to store player information
        private User _player1; // Stores the details of Player 1
        private User? _player2; // Stores the details of Player 2 (optional for single-player mode)

        /// <summary>
        /// Constructor for predefined category setup.
        /// </summary>
        /// <param name="category">The selected predefined category.</param>
        /// <param name="authenticationService">An instance of the AuthenticationService.</param>
        /// <param name="player1">Details of Player 1.</param>
        /// <param name="player2">Details of Player 2 (optional).</param>
        public GameSetupWindow(string category, AuthenticationService authenticationService, User player1, User player2 = null)
        {
            InitializeComponent();
            _selectedCategory = category;
            _authenticationService = authenticationService;
            _player1 = player1;
            _player2 = player2;

            if (string.IsNullOrEmpty(_selectedCategory))
            {
                MessageBox.Show("Predefined category is null or empty.", "Debug", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Constructor for custom category setup.
        /// </summary>
        /// <param name="customCategory">The selected custom category.</param>
        /// <param name="authenticationService">An instance of the AuthenticationService.</param>
        /// <param name="player1">Details of Player 1.</param>
        /// <param name="player2">Details of Player 2 (optional).</param>
        public GameSetupWindow(CustomCategory customCategory, AuthenticationService authenticationService, User player1, User player2 = null)
        {
            InitializeComponent();
            _selectedCustomCategory = customCategory;
            _authenticationService = authenticationService;
            _player1 = player1;
            _player2 = player2;
        }

        /// <summary>
        /// Event handler for the "Start Game" button click.
        /// Determines the game mode and proceeds to start the game.
        /// </summary>
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the Single Player mode is selected
            bool isSinglePlayer = SinglePlayerRadioButton.IsChecked == true;

            if (isSinglePlayer)
            {
                // Start the game in single-player mode
                StartGameBoard(isSinglePlayer);
            }
            else if (TwoPlayersRadioButton.IsChecked == true)
            {
                // Open the login window to authenticate the second player
                LoginWindow loginWindow = new LoginWindow(_authenticationService, LoginContext.AddSecondPlayer);
                loginWindow.ShowDialog(); // Show the login window as a dialog

                if (loginWindow.IsLoginSuccessful && loginWindow.LoggedInUser != null)
                {
                    // If login is successful, set Player 2 and start the game
                    _player2 = loginWindow.LoggedInUser;
                    StartGameBoard(isSinglePlayer);
                }
                else
                {
                    // Show an error message if login fails
                    MessageBox.Show(
                        "Second player login failed. Game cannot be started.",
                        "Login Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        /// <summary>
        /// Starts the game by opening the GameBoardWindow.
        /// </summary>
        /// <param name="isSinglePlayer">Indicates if the game is in single-player mode.</param>
        private void StartGameBoard(bool isSinglePlayer)
        {
            if (_selectedCustomCategory != null)
            {
                GameBoardWindow gameBoardWindow = new GameBoardWindow(_selectedCustomCategory, isSinglePlayer, _player1, _player2);
                gameBoardWindow.Show();
            }
            else if (!string.IsNullOrEmpty(_selectedCategory))
            {
                GameBoardWindow gameBoardWindow = new GameBoardWindow(_selectedCategory, isSinglePlayer, _player1, _player2);
                gameBoardWindow.Show();
            }
            else
            {
                return;
            }

            this.Close();
        }

    }
}