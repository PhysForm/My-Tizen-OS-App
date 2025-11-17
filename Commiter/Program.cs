
using System;
using System.Diagnostics;

namespace Commiter
{
    public class Program
    {
        static void text(string text)
        {
            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {
            string commitMessage;
            bool useLfs = false;
            bool addAll = false;
            string? lfsTrackFile = null;

            // Parse arguments
            if (args.Length > 0)
            {
                // With args mode
                commitMessage = string.Empty;
                
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-lfs" || args[i] == "--lfs")
                    {
                        useLfs = true;
                    }
                    else if (args[i] == "-a" || args[i] == "--all")
                    {
                        addAll = true;
                    }
                    else if (args[i] == "-m" || args[i] == "--message")
                    {
                        if (i + 1 < args.Length)
                        {
                            commitMessage = args[i + 1];
                            i++;
                        }
                    }
                    else if (args[i] == "-t" || args[i] == "--track")
                    {
                        if (i + 1 < args.Length)
                        {
                            lfsTrackFile = args[i + 1];
                            i++;
                        }
                    }
                    else if (string.IsNullOrEmpty(commitMessage))
                    {
                        // If no -m flag, treat first non-flag arg as message
                        commitMessage = args[i];
                    }
                }

                if (string.IsNullOrEmpty(commitMessage))
                {
                    text("Error: Commit message required when using arguments.");
                    text("Usage: Commiter [-m] <message> [-a|--all] [-lfs|--lfs] [-t|--track <file>]");
                    return;
                }
            }
            else
            {
                // Without args mode - interactive
                text("Commit Message: ");
                commitMessage = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(commitMessage))
                {
                    text("Error: Commit message cannot be empty.");
                    return;
                }

                text("Add all files? (y/n): ");
                string? addAllInput = Console.ReadLine();
                addAll = addAllInput?.ToLower() == "y";

                text("Use git LFS? (y/n): ");
                string? lfsInput = Console.ReadLine();
                useLfs = lfsInput?.ToLower() == "y";

                text("Track specific file with git LFS? (leave empty to skip): ");
                lfsTrackFile = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lfsTrackFile))
                {
                    lfsTrackFile = null;
                }
            }

            // Execute git commands
            try
            {
                // Track specific file with LFS if requested
                if (!string.IsNullOrEmpty(lfsTrackFile))
                {
                    text($"Tracking '{lfsTrackFile}' with git LFS...");
                    RunGitCommand($"lfs track \"{lfsTrackFile}\"");
                    RunGitCommand("add .gitattributes");
                }

                if (addAll)
                {
                    text("Adding all files...");
                    RunGitCommand("add -A");
                }
                else if (useLfs)
                {
                    text("Adding files via git LFS...");
                    RunGitCommand("lfs add .");
                    RunGitCommand("add .gitattributes");
                }

                text($"Committing with message: '{commitMessage}'...");
                RunGitCommand($"commit -m \"{commitMessage}\"");
                text("Commit successful!");
            }
            catch (Exception ex)
            {
                text($"Error during commit: {ex.Message}");
            }
        }

        static void RunGitCommand(string arguments)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                throw new Exception("Failed to start git process.");
            }

            process.WaitForExit();
            
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }

            if (process.ExitCode != 0)
            {
                throw new Exception($"Git command failed: {error}");
            }
        }
    }
}
