namespace PIIIProject.Initial.Models
{
    public class GameState
    {
        /// <summary>
        /// Gets or sets the username of Player 1
        /// </summary>
        public string Player1Username { get; set; }

        /// <summary>
        /// Gets or sets the username of Player 2
        /// Null if a second player is not set
        /// </summary>
        public string? Player2Username { get; set; }

        /// <summary>
        /// Gets or sets the score of Player 1
        /// </summary>
        public int Player1Score { get; set; }

        /// <summary>
        /// Gets or sets the score of Player 2
        /// </summary>
        public int Player2Score { get; set; }

        /// <summary>
        /// Gets or sets the currently selected category for the game
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// Gets or sets a collection of custom categories 
        /// Null if no custom categories are defined
        /// </summary>
        public Dictionary<string, List<Question>>? CustomCategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is Player 1's turn
        /// </summary>
        public bool IsPlayer1Turn { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the game state was last saved
        /// </summary>
        public DateTime SaveTimestamp { get; set; }
    }
}
