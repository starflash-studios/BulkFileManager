using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;

namespace BulkFileManager {
    class Program {
        static bool recursion = false;
        static bool lazy = false;
        static string activeFolder = "";
        static string command = "";
        static string wildcard = "*.*";

        static void Main(string[] args) {
            if (args == null || args.Length < 1 || args.Contains("/?")) { Help(); return; }
            for (int a = 0; a < args.Length; a++) {  args[a] = args[a].ToLower(); }
            ProcessArgs(args);
        }

        static void Help() {
            Console.WriteLine("Bulk File Manager (by Starflash Studios)");
            Console.WriteLine("Usage: BFM.exe [directory] [/r, /l, and /w + pattern] [/c +command]");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("/r = Recursive Searching Mode (Scans both directory AND subdirectories)");
            Console.WriteLine("/l = Local Mode (Replaces ./ with current directory -- Saves time, but can break on some instances");
            Console.WriteLine("/w [x] = Wildcard Mode, where [x] is your wildcard (ie *.txt)");
            Console.WriteLine("/c [x] = Command Mode, where [x] is your command (Add any of the below replacements if desired)");
            Console.WriteLine("");
            Console.WriteLine("Command Replacements:");
            Console.WriteLine(@"`p = Path (ie c:\test\example.png)");
            Console.WriteLine("`f = File (ie example.png)");
            Console.WriteLine("`e = Extension (ie .png)");
            Console.WriteLine(@"`d = Directory (ie c:\test)");
            Console.WriteLine("`n = Filename (ie example)");
        }

        static void ProcessArgs(string[] args) {
            if (!args[0].Contains(":"[0])) {
                activeFolder = GetFolder("./");
            } else {
                activeFolder = GetFolder(args[0]);
            }
            
            if (args.Contains("/r")) { recursion = true; }
            if (args.Contains("/l")) { lazy = true; }
            for (int a = 0; a < args.Length; a++) {
                if (args[a] == "/w") {
                    wildcard = args[a + 1];
                } else if (args[a] == "/c") {
                    for (int b = a+1; b < args.Length; b++) {
                        command += "\"" + args[b] + "\"" + " ";
                    }
                }
            }


            string[] gotFiles = new string[0];
            if (recursion) {
                gotFiles = Directory.GetFiles(activeFolder, wildcard, SearchOption.AllDirectories);
            } else {
                gotFiles = Directory.GetFiles(activeFolder, wildcard);
            }

            foreach(string f in gotFiles) {
                ProcessFile(f);
            }
        }

        static void ProcessFile(string file) {
            Console.WriteLine("Processing: " + file);
            if (string.IsNullOrEmpty(command)) {
                OpenWithDefaultProgram(file);
            } else {
                string cmd = command.Replace("`d", Path.GetDirectoryName(file));
                cmd = cmd.Replace("`p", file);
                cmd = cmd.Replace("`n", Path.GetFileNameWithoutExtension(file));
                cmd = cmd.Replace("`e", Path.GetExtension(file));
                cmd = cmd.Replace("`f", Path.GetFileName(file));
                if (lazy) {
                    cmd = cmd.Replace("./", GetFolder("./"));
                    cmd = cmd.Replace(@".\", GetFolder("./"));
                }
                OpenWithCommandPrompt(cmd);
            }
        }

        public static void OpenWithDefaultProgram(string path) {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        static void OpenWithCommandPrompt(string command) {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        static string GetFolder(string input) {
            if (input[0] == "."[0] && (input[1] == @"\"[0] || input[1] == "/"[0])) {
                input = AppDomain.CurrentDomain.BaseDirectory + @"\" + input.TrimStart("."[0]);
            }
            return Path.GetDirectoryName(input) + @"\";
        }
    }
}