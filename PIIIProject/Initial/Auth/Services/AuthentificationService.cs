using PIIIProject.Initial.Models;
using System.Text.RegularExpressions;

namespace PIIIProject.Initial.Auth.Services
{
    public class AuthenticationService
    {
        private readonly FileUserDataStorage _userDataStorage; // Handles user data storage operations

        /// <summary>
        /// Initializes the AuthenticationService with a user data storage dependency
        /// </summary>
        public AuthenticationService()
        {
            _userDataStorage = new FileUserDataStorage();
        }

        /// <summary>
        /// Validates if the provided email meets the expected format
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if the email is valid, otherwise false</returns>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            email = email.Trim(); // Removes leading/trailing whitespace
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z]+\.(com|net|ca)$"; // Email must end with specific domains
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Validates if the provided password meets security requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>Validation message indicating success or the reason for failure</returns>
        public string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return "Password cannot be empty.";
            if (password.Length < 8) return "Password must be at least 8 characters long.";
            if (!Regex.IsMatch(password, "[0-9]")) return "Password must contain at least one digit.";
            if (!Regex.IsMatch(password, "[!@#$%^&*()_+{}\\[\\]:;<>,.?~\\/-]")) return "Password must contain at least one special character.";
            if (!Regex.IsMatch(password, "[A-Z]")) return "Password must contain at least one uppercase letter.";
            if (!Regex.IsMatch(password, "[a-z]")) return "Password must contain at least one lowercase letter.";

            return "Valid"; // Password meets all requirements
        }

        /// <summary>
        /// Validates if the provided username is non-empty
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <returns>True if the username is valid, otherwise false</returns>
        public bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username); 
        }

        /// <summary>
        /// Verifies if the provided email and password match a stored user
        /// </summary>
        /// <param name="email">Email of the user attempting to log in</param>
        /// <param name="password">Password of the user attempting to log in</param>
        /// <returns>True if the email and password match a user, otherwise false</returns>
        public bool VerifyLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return false;

            User user = GetUserByEmail(email); // Retrieve user by email
            return user != null && user.Password == password; // Check if user exists and password matches
        }

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User object if found, otherwise null</returns>
        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return _userDataStorage.GetUserByEmail(email); 
        }
    }
}
