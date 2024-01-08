namespace REgenerator
{
    /// <summary>
    /// Handles the display and interaction of the settings menu in the console.
    /// </summary>
    internal class Menu
    {
        /// <summary>
        /// Displays the settings menu and handles user input for selection and toggling options.
        /// </summary>
        public static void ShowSettingsMenu()
        {
            var exitMenu = false;
            var currentSelection = 0;

            while (!exitMenu)
            {
                Console.Clear();
                PrintColoredAsciiArt();
                Console.WriteLine();

                // Display menu options and current settings
                DisplayMenuOption(0, currentSelection, "Include comments", Settings.GenerateComments);
                DisplayMenuOption(1, currentSelection, "Include C++ signature", Settings.GenerateCppSignature);
                DisplayMenuOption(2, currentSelection, "Generate single file", Settings.GenerateSingleFile);

                // Display "Generate" in green
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{GetMenuOptionString(3, currentSelection)} Generate");
                Console.ResetColor();

                // Display "Exit" in red
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{GetMenuOptionString(4, currentSelection)} Exit");
                Console.ResetColor();

                // Handle key input for menu navigation and selection
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        currentSelection = (currentSelection + 4) % 5;
                        break;
                    case ConsoleKey.DownArrow:
                        currentSelection = (currentSelection + 1) % 5;
                        break;
                    case ConsoleKey.Enter:
                        HandleEnterPress(currentSelection);
                        exitMenu = currentSelection == 4;
                        break;
                }
            }
        }

        /// <summary>
        /// Displays a single menu option with its current state (Yes/No).
        /// </summary>
        /// <param name="index">Index of the menu option.</param>
        /// <param name="currentSelection">Index of the currently selected menu option.</param>
        /// <param name="text">Text label of the menu option.</param>
        /// <param name="toggleValue">Current boolean state of the option (Yes/No).</param>
        public static void DisplayMenuOption(int index, int currentSelection, string text, bool toggleValue)
        {
            Console.WriteLine($"{GetMenuOptionString(index, currentSelection)} {text}: {(toggleValue ? "Yes" : "No")}");
        }

        /// <summary>
        /// Handles the actions to perform when the Enter key is pressed based on the current menu selection.
        /// </summary>
        /// <param name="currentSelection">Index of the currently selected menu option.</param>
        public static void HandleEnterPress(int currentSelection)
        {
            switch (currentSelection)
            {
                case 0:
                    Settings.GenerateComments = !Settings.GenerateComments;
                    break;
                case 1:
                    Settings.GenerateCppSignature = !Settings.GenerateCppSignature;
                    break;
                case 2:
                    Settings.GenerateSingleFile = !Settings.GenerateSingleFile;
                    break;
                case 3:
                    Api.Generate();
                    break;
            }
        }

        /// <summary>
        /// Prints ASCII art for the program header with "RE" in "REgenerator" colored differently.
        /// </summary>
        public static void PrintColoredAsciiArt()
        {
            string[] asciiArtLines = {
                @"██████╗ ███████╗ ██████╗ ███████╗███╗   ██╗███████╗██████╗  █████╗ ████████╗ ██████╗ ██████╗ ",
                @"██╔══██╗██╔════╝██╔════╝ ██╔════╝████╗  ██║██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗",
                @"██████╔╝█████╗  ██║  ███╗█████╗  ██╔██╗ ██║█████╗  ██████╔╝███████║   ██║   ██║   ██║██████╔╝",
                @"██╔══██╗██╔══╝  ██║   ██║██╔══╝  ██║╚██╗██║██╔══╝  ██╔══██╗██╔══██║   ██║   ██║   ██║██╔══██╗",
                @"██║  ██║███████╗╚██████╔╝███████╗██║ ╚████║███████╗██║  ██║██║  ██║   ██║   ╚██████╔╝██║  ██║",
                @"╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝",
            };

            foreach (string line in asciiArtLines)
            {
                // Coloring specific characters for visual effect
                for (int i = 0; i < line.Length; i++)
                {
                    Console.ForegroundColor = i < 16 ? ConsoleColor.Red : ConsoleColor.White;
                    Console.Write(line[i]);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Notifies the user if the 'natives.json' file is missing in the program directory.
        /// </summary>
        public static void NotifyFileMissing()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File 'natives.json' not found in the REgenerator directory.");
            Console.ResetColor();
        }

        /// <summary>
        /// Offers the user an option to download 'natives.json' via Git if it's not found.
        /// </summary>
        /// <param name="gitRepoUrl">URL of the Git repository to download from.</param>
        /// <returns>True if the download was initiated and successful, false otherwise.</returns>
        public static bool OfferGitDownload(string gitRepoUrl)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Git is installed.");
            Console.WriteLine("Would you like to download 'natives.json' from the GitHub repository? (y/n)");
            Console.ResetColor();

            HighlightGitAuthor();

            // Loop until valid input (Y or N) is received
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Y:
                        return Git.CloneGitRepository(gitRepoUrl);
                    case ConsoleKey.N:
                        return false;
                }
            }
        }

        /// <summary>
        /// Notifies the user if Git is not installed, which is required for downloading the JSON file.
        /// </summary>
        /// <param name="gitRepoUrl">URL of the Git repository where 'natives.json' can be downloaded.</param>
        public static void NotifyGitNotInstalled(string gitRepoUrl)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Git is not installed. Please install Git or manually download 'natives.json' from:");
            Console.WriteLine(gitRepoUrl);
            Console.ResetColor();

            HighlightGitAuthor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Highlights the name of the main author and contributors of the 'gta5-nativedb-data' project.
        /// </summary>
        public static void HighlightGitAuthor()
        {
            // Detailed credit to the repository's main author and contributors
            Console.WriteLine("\nREgenerator uses data from the 'gta5-nativedb-data' repository.\n");

            Console.Write("Special thanks to ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("alloc8or");
            Console.ResetColor();
            Console.WriteLine(" for the original creation and maintenance of the repository.");

            Console.WriteLine("Additionally, heartfelt appreciation goes out to all the ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("contributors");
            Console.ResetColor();
            Console.WriteLine(" who have made the project possible through their hard work and dedication.");

            Console.WriteLine("For more information and to contribute, visit the GitHub repository:\nhttps://github.com/alloc8or/gta5-nativedb-data\n");
        }

        /// <summary>
        /// Returns a formatted string for each menu option, marking the current selection.
        /// </summary>
        public static string GetMenuOptionString(int optionIndex, int currentSelection)
        {
            return optionIndex == currentSelection ? "> " : "  ";
        }
    }
}