using PIIIProject.Initial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PIIIProject.Initial.Game
{
    /// <summary>
    /// Interaction logic for LeaderboardWindow.xaml
    /// </summary>
    public partial class LeaderboardWindow : Window
    {
        private bool isSinglePlayer; // To determine if it is single-player mode
        private int player1Score; // Player 1's final score
        private int player2Score; // Player 2's final score
        private User _player1;
        private User? _player2;


        /// <summary>
        /// Constructor for LeaderboardWindow.
        /// </summary>
        /// <param name="isSinglePlayer">True if the game was played in single-player mode.</param>
        /// <param name="player1Score">Final score of Player 1.</param>
        /// <param name="player2Score">Final score of Player 2 (0 if single-player).</param>
        public LeaderboardWindow(bool isSinglePlayer, int player1Score, int player2Score, User player1, User player2)
        {
            InitializeComponent();
            this.isSinglePlayer = isSinglePlayer;
            this.player1Score = player1Score;
            this.player2Score = player2Score;
            _player1 = player1;
            _player2 = player2;

            DisplayLeaderboard(); // Display the leaderboard
        }

        /// <summary>
        /// Displays the leaderboard based on the game mode and scores.
        /// </summary>
        private void DisplayLeaderboard()
        {
            if (isSinglePlayer)
            {
                // Display single-player score
                AddTextBlock($"{_player1.Username} Score: {player1Score}");
            }
            else
            {
                // Determine the winner and loser in multiplayer mode
                string winner = player1Score > player2Score ? $"{_player1.Username}" : $"{_player2.Username}";
                string loser = player1Score > player2Score ? $"{_player2.Username}" : $"{_player1.Username}";
                int winnerScore = Math.Max(player1Score, player2Score);
                int loserScore = Math.Min(player1Score, player2Score);

                // Display the winner and loser
                AddTextBlock($"Winner: {winner} - Score: {winnerScore}");
                AddTextBlock($"Loser: {loser} - Score: {loserScore}");
            }
        }

        /// <summary>
        /// Adds a TextBlock to the LeaderboardPanel.
        /// </summary>
        /// <param name="text">The text to display.</param>
        private void AddTextBlock(string text)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            LeaderboardPanel.Children.Add(textBlock);
        }

        /// <summary>
        /// Handles the click event for the Close button.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the Leaderboard window
        }
    }
}