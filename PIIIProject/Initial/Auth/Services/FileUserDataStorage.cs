using PIIIProject.Initial.Auth.Interface;
using PIIIProject.Initial.Models;
using System.IO;

namespace PIIIProject.Initial.Auth.Services
{
    public class FileUserDataStorage : IUserDataStorage
    {
        private readonly string _filePath; // Path to the file used for storing user data

        /// <summary>
        /// Initializes the FileUserDataStorage with a default file path
        /// </summary>
        public FileUserDataStorage()
        {
            // Look for the consistent part of the path
            string currentDirectory = Directory.GetCurrentDirectory();
            string targetSubPath = @"course-project-runtime-terrors\PIIIProject";

            // Find the base directory containing the target subpath
            string projectRootDirectory = FindDirectoryPath(currentDirectory, targetSubPath);
            if (string.IsNullOrEmpty(projectRootDirectory))
            {
                throw new DirectoryNotFoundException($"The project root directory containing '{targetSubPath}' was not found.");
            }

            // Create the Users folder path
            string userFolder = Path.Combine(projectRootDirectory, "Users");
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder); // Ensure the directory exists
            }

            _filePath = Path.Combine(userFolder, "registrations.txt");
        }

        /// <summary>
        /// Searches for the root directory containing the specified subpath.
        /// </summary>
        /// <param name="startDirectory">The starting directory for the search.</param>
        /// <param name="targetSubPath">The subpath to look for.</param>
        /// <returns>The full path to the directory containing the subpath, or null if not found.</returns>
        private string FindDirectoryPath(string startDirectory, string targetSubPath)
        {
            string directory = startDirectory;

            while (directory != null)
            {
                if (directory.EndsWith(targetSubPath, StringComparison.OrdinalIgnoreCase))
                {
                    return directory;
                }

                directory = Directory.GetParent(directory)?.FullName;
            }

            return null;
        }

        /// <summary>
        /// Saves a user's data to the storage file
        /// </summary>
        /// <param name="user">User object containing the data to save</param>
        /// <returns>True if the user was successfully saved otherwise false</returns>
        public bool SaveUserAsync(User user)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_filePath)) // Opens the file for appending
                {
                    writer.WriteLine($"{user.Username},{user.Password},{user.Email},{user.Score}"); // Formats the user data
                }
                return true; // Successfully saved
            }
            catch
            {
                return false; // Failed to save
            }
        }

        /// <summary>
        /// Loads all user data from the storage file and parses file and creates a collection of user objects
        /// </summary>
        /// <returns>A collection of User objects, empty if the file does not exist</returns>
        public IEnumerable<User> LoadUsersAsync()
        {
            if (!File.Exists(_filePath))
            {
                return Enumerable.Empty<User>();
            }

            List<User> users = new List<User>();
            string[] lines = File.ReadAllLines(_filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    User user = new(parts[0], parts[1], parts[2], int.Parse(parts[3]));
                    users.Add(user);
                }
            }
            return users;
        }

        /// <summary>
        /// Retrieves a user by their email address, then searches loaded user data for a match
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>The User object if found otherwise, null</returns>
        public User GetUserByEmail(string email)
        {
            IEnumerable<User> users = LoadUsersAsync();
            foreach (User user in users)
            {
                if (user.Email == email)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
