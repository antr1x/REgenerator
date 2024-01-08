namespace REgenerator
{
    public class Program
    {
        public static void Main()
        {
            // Hide the console blinking cursor
            Console.CursorVisible = false;

            // Define the GitHub repository URL and local file temp paths
            var gitRepoUrl = "https://github.com/alloc8or/gta5-nativedb-data";
            var gitRepoPath = gitRepoUrl.Split('/').Last();
            var filePathRepo = Path.Combine(gitRepoPath, "natives.json");
            var filePathLocal = "natives.json";

            // Check if the local natives.json file exists
            if (File.Exists(filePathLocal))
            {
                // If local file exists, set the path in settings
                Settings.NativeJsonPath = filePathLocal;
            }
            else
            {
                // Check if the file exists in the Git cloned directory
                if (File.Exists(filePathRepo))
                {
                    // If file exists in the Git cloned directory, set the path in settings
                    Settings.NativeJsonPath = filePathRepo;
                }
                else if (Git.IsGitInstalled())
                {
                    // If Git is installed, offer to download the file
                    if (Menu.OfferGitDownload(gitRepoUrl))
                    {
                        // Update the file path in settings after downloading
                        Settings.NativeJsonPath = filePathRepo;
                    }
                    else
                    {
                        // Notify user if they choose not to download the file
                        Menu.NotifyFileMissing();
                        return;
                    }
                }
                else
                {
                    // Notify user if Git is not installed
                    Menu.NotifyFileMissing();
                    Menu.NotifyGitNotInstalled(gitRepoUrl);
                    return;
                }
            }

            // Show the settings menu with the appropriate file path
            Menu.ShowSettingsMenu();
        }
    }
}
