using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    /// <summary>
    /// ProcessCreator creates a new process for each subdirectory containing CDFs.
    /// </summary>
    public class ProcessCreator
    {
        /// <summary>
        /// Boolean to let software know if the logs should be written as separate files or one file.
        /// The writing of the files are handled by external code. (e.g. CDFTester)
        /// </summary>
        public bool WriteAsCollection { get; set; }

        /// <summary>
        /// Run() calls to PathRetriever.GetPaths to get a list of subdirectories containing cdfs.
        /// For each path found, a process is created using ProcessInfo.StartProcess().
        /// A stopwatch is started to track elapsed time.
        /// </summary>
        /// <param name="argPaths">Args passed from Main. A string array of paths to check for CDFs</param>
        public void Run(string[] argPaths, bool writeAsCollection = false)
        {
            WriteAsCollection = writeAsCollection;
            if (argPaths.Length > 0)
            {
                List<string> tempPaths = new List<string>();
                foreach (string arg in argPaths)
                {
                    if (Directory.Exists(arg))
                    {
                        tempPaths.Add(arg);
                    }
                }

                IEnumerable<string> paths = PathRetriever.GetPaths(tempPaths);

                // Start stopwatch to calculate total execution time.
                var watch = Stopwatch.StartNew();
#if DEBUG
                string cdfExecutable = "../../../../CDFTester/bin/x64/Debug/CDFTester.exe";
#else
                string cdfExecutable = "../../../../CDFTester/bin/x64/Release/CDFTester.exe";
#endif
                // Check to make sure CDFTester Executable exists. If not, exit program.
                if (!File.Exists(cdfExecutable))
                {
                    Console.WriteLine("CDFTester.exe not found. Check path in ProcessCreator.cs");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                if (!WriteAsCollection)
                {
                    foreach (string path in paths)
                        ProcessInfo.StartProcess(cdfExecutable, String.Format("-proc {0}", path));
                }
                else
                {
                    foreach (string path in paths)
                        ProcessInfo.StartProcess(cdfExecutable, String.Format("-proc -coll {0}", path));
                }

                int numProcsStarted = ProcessInfo.Processes.Count();

                Console.WriteLine("{0} proccesses have been initiated.\n", numProcsStarted);

                // ConsoleWriter clears lines to make it a little neater,
                // Therefore, we let ConsoleWriter know where the cursor is currently.
                ConsoleWriter.OrigRow = Console.CursorTop;
                ConsoleWriter.OrigCol = Console.CursorLeft;

                // Wait for all processes to exit.
                ProcessInfo.WaitForProcesses();
                watch.Stop();

                // Calculate and print elapsed time.
                SWatch elTime = new SWatch();
                elTime.MS = watch.ElapsedMilliseconds;
                elTime.PrintTime();
            }
            // Get the directory the log files are printed to by the processes and display to user.
            string desktopDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "INVALID_CDFs");
            Console.WriteLine("\nCheck {0} for the logs.\n", desktopDir);
        }
    }
}
