using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mpt
{
    class Program
    {
        static List<string> availablePackages = new List<string>() {
            "tpruvot/cpuminer-multi",
            "xmrig/xmrig",
            "minerpool/aeon-cpu-miner",
            "minecoins/minergate-cli",
            "robostlund/miner-trex-cuda",
            "odelucca/chia-plotter",
            "dockminer/gminer",
            "tacobit/bminer",
            "tacobit/bminer-amd",
            "mrazu/teamredminer_amdgpu-pro",
            "marsmensch/cgminer"
        };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command specified. Use 'mpt --help' for a list of commands.");
                return;
            }

            string command = args[0];

            if (command == "--help" || command == "-h")
            {
                ShowHelp();
                return;
            }

            if (command == "search")
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Package name not specified.");
                    return;
                }

                string packageName = args[1];
                List<string> matches = Search(packageName);

                if (matches.Count == 0)
                {
                    Console.WriteLine($"No packages matching '{packageName}' found.");
                }
            }
            else if (command == "--list" || command == "-l")
            {
                ListPackages();
            }
            else if (command == "--version")
            {
                ShowVersion();
            }
            else if (args.Length < 2)
            {
                Console.WriteLine("Package name not specified.");
                return;
            }
            else
            {
                string packageName = args[1];

                if (!availablePackages.Contains(packageName))
                {
                    Console.WriteLine($"Package {packageName} not available.");
                    return;
                }

                switch (command)
                {
                    case "install":
                        Install(packageName);
                        break;
                    case "purge":
                    case "remove":
                        Remove(packageName);
                        break;
                    default:
                        Console.WriteLine("Command not recognized. Use 'mpt --help' for a list of commands.");
                        break;
                }
            }
        }

        static void Install(string packageName)
        {
            string command = $"docker pull {packageName}";
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
        }

        static void Remove(string packageName)
        {
            string command = $"docker rmi {packageName}";
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
        }

        static void ListPackages()
        {
            Console.WriteLine("Installed packages:");
            string command = "docker images";
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
        }

        static List<string> Search(string packageName)
        {
            List<string> results = availablePackages.FindAll(name => name.Contains(packageName));

            if (results.Count == 0)
            {
                Console.WriteLine($"No packages found matching '{packageName}'.");
            }
            else
            {
                Console.WriteLine($"Packages found matching '{packageName}':");
                foreach (string result in results)
                {
                    Console.WriteLine(result);
                }
            }

            return results;
        }

        static void ShowVersion()
        {
            Console.WriteLine("MPT version 1.0");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: mpt [options] [package_name]");
            Console.WriteLine("Options:");
            Console.WriteLine("  install\tInstall a package");
            Console.WriteLine("  purge, remove\tRemove a package");
            Console.WriteLine("  -l, --list\tList installed packages");
            Console.WriteLine("  search\tSearch a specific Package");
            Console.WriteLine("  --version\tShow MPT version");
            Console.WriteLine("  -h, --help\tShow this help message");
        }
    }
}
