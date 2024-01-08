using System.Diagnostics;
using System.IO;

namespace REgenerator
{
    internal class Git
    {
        /// <summary>
        /// Checks if Git is installed on the system by attempting to execute 'git --version'.
        /// </summary>
        /// <returns>True if Git is installed and can be executed, otherwise false.</returns>
        public static bool IsGitInstalled()
        {
            try
            {
                // Set up the process start information for 'git --version'
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true // Avoid creating a new window for the process
                };

                // Start the process and read the output
                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        // Read the output to determine if the 'git' command is successful
                        var result = reader.ReadToEnd();
                        // Check if the output contains 'git version'
                        return !string.IsNullOrEmpty(result) && result.Contains("git version");
                    }
                }
            }
            catch
            {
                // An exception is likely to occur if 'git' is not a recognized command
                return false;
            }
        }

        /// <summary>
        /// Clones the specified Git repository to the local machine.
        /// </summary>
        /// <param name="gitRepoUrl">The URL of the Git repository to clone.</param>
        /// <returns>True if the repository was successfully cloned, otherwise false.</returns>
        public static bool CloneGitRepository(string gitRepoUrl)
        {
            try
            {
                // Set up the process start information for 'git clone'
                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone {gitRepoUrl}",
                    UseShellExecute = false,
                    CreateNoWindow = true // Run the process without opening a new window
                };

                // Start the cloning process
                using (var process = Process.Start(psi))
                {
                    Console.Write("Cloning repository");
                    // Loop until the cloning process has completed
                    while (!process!.HasExited)
                    {
                        Console.Write(".");
                        Thread.Sleep(500); // Provide a visual indication of ongoing process
                    }
                }

                Console.WriteLine("\nDone.");
                return true; // Indicate successful cloning
            }
            catch
            {
                // Handle any exceptions that occur during the cloning process
                Console.WriteLine("\nFailed to clone the repository.");
                return false; // Indicate unsuccessful cloning
            }
        }
    }
}