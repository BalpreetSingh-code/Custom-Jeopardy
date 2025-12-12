namespace PIIIProject.Initial.Models{
    public class User
    {
        /// <summary>
        /// Gets or sets the username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the current score of the user
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the list of custom categories created by the user
        /// </summary>
        public List<CustomCategory> CustomCategories { get; set; } = []; // Initializes default to empty

        /// <summary>
        /// Initializes a new instance of the User class with the specified details
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <param name="email">Email address of the user</param>
        /// <param name="score">Initial score of the user</param>
        public User(string username, string password, string email, int score)
        {
            Username = username;
            Password = password;
            Email = email;
            Score = score;
        }
    }
}
