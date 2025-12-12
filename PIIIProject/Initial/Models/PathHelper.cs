using System.IO;

namespace PIIIProject.Initial.Models
{
    public static class PathHelper // Static class since it provides utility functions and does not need instance
    {
        /// <summary>
        /// Gets the root directory of the project by searching for a specific target subpath in the directory structure
        /// </summary>
        /// <returns>The full path to the project root directory.</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified target subpath is not found in the directory structure
        /// </exception>
        public static string GetProjectRootDirectory()
        {
            string targetSubPath = @"course-project-runtime-terrors\PIIIProject"; 
            string currentDirectory = Directory.GetCurrentDirectory();
            string rootDirectory = FindDirectoryPath(currentDirectory, targetSubPath);

            if (string.IsNullOrEmpty(rootDirectory)) 
            {
                throw new DirectoryNotFoundException($"The project root directory containing '{targetSubPath}' was not found.");
            }

            return rootDirectory; 
        }

        /// <summary>
        /// Recursively searches parent directories to find the directory that matches the specified target subpath
        /// </summary>
        /// <param name="startDirectory">Starting directory for the search</param>
        /// <param name="targetSubPath">Subpath to search for</param>
        /// <returns>Full path of the directory containing the target subpath, or null if not found</returns>
        private static string FindDirectoryPath(string startDirectory, string targetSubPath)
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
    }
}
