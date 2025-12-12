using PIIIProject.Initial.Auth.Services;
using PIIIProject.Initial.Models;

public class RegistrationService
{
    private readonly FileUserDataStorage _userDataStorage; // Handles user data storage
    private readonly AuthenticationService _authenticationService; // Handles authentication and validation

    /// <summary>
    /// Initializes RegistrationService with dependencies for user data storage and authentication
    /// </summary>
    public RegistrationService()
    {
        _userDataStorage = new FileUserDataStorage();
        _authenticationService = new AuthenticationService();
    }

    /// <summary>
    /// Registers a new user after validating their details
    /// </summary>
    /// <param name="user">User to register</param>
    /// <returns>Message indicating the result of the registration attempt</returns>
    public string RegisterUser(User user)
    {
        if (user == null) return "User cannot be null."; // Ensure user object is not null

        if (!_authenticationService.IsValidUsername(user.Username))
            return "Invalid username. Please provide a valid username."; // Check username format

        if (!_authenticationService.IsValidEmail(user.Email))
            return "Invalid email format."; // Check email format

        string passwordValidationResult = _authenticationService.ValidatePassword(user.Password);
        if (passwordValidationResult != "Valid")
            return passwordValidationResult; // Return password validation result if invalid

        if (IsDuplicateUser(user.Username, user.Email))
            return "A user with the same username or email already exists."; // Ensure no duplicate users

        if (!_userDataStorage.SaveUserAsync(user))
            return "An error occurred while saving the user."; // Check if saving the user failed

        return "Success"; // Return success if all checks and saving succeeded
    }

    /// <summary>
    /// Checks if a user with the same username or email already exists
    /// </summary>
    /// <param name="username">Username to check for duplicates</param>
    /// <param name="email">Email to check for duplicates</param>
    /// <returns>True if a duplicate user is found otherwise false</returns>
    private bool IsDuplicateUser(string username, string email)
    {
        IEnumerable<User> users = _userDataStorage.LoadUsersAsync(); // Load existing users
        foreach (User existingUser in users)
        {
            if (existingUser.Username.ToLower() == username.ToLower() || existingUser.Email.ToLower() == email.ToLower())
            {
                return true; 
            }
        }
        return false; 
    }
}
