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
    /// An abstraction of CDFTester process information.
    /// Includes information about elapsed time, path name, and eventhandler for process exit.
    /// </summary>
    public class ProcessInfo
    {
        /// <summary>
        /// The process started by ProcessCreator class.
        /// </summary>
        public Process Process { get; set; }

        /// <summary>
        /// The CDF path being tested by the process.
        /// </summary>
        public string CDFPath { get; set; }

        /// <summary>
        /// Tracks the elapsed time of the process.
        /// </summary>
        public Stopwatch Clock { get; set; }

        private bool _eventHandled;

        /// <summary>
        /// True if the process-exit eventhandler has been triggered.
        /// </summary>
        public bool EventHandled { get { return _eventHandled; } }

        /// <summary>
        /// A list of processes currently running.
        /// Processes are added before start and removed upon exit.
        /// </summary>
        public static List<ProcessInfo> Processes = new List<ProcessInfo>();

        /// <summary>
        /// List of all stopwatches of exited processes.
        /// Used for estimating the time left for program to run.
        /// </summary>
        public static List<Stopwatch> Watches = new List<Stopwatch>();

        /// <summary>
        /// EventHandler triggered by an exited process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExited(object sender, System.EventArgs e)
        {
            _eventHandled = true;
            Clock.Stop();
            Watches.Add(Clock);
            Processes.Remove(this);
#if !DEBUG
            ConsoleWriter.ProcessExited(this, Processes.Count());
            ConsoleWriter.Working(procNotExited: false);
#endif
        }

        /// <summary>
        /// Initiates a process given a CDFTester executable and a path to check for corrupt CDFs.
        /// </summary>
        /// <param name="_cdfExecutable">File path of CDFTester executable.</param>
        /// <param name="path">The path to check for corrupted CDF files.</param>
        public static void StartProcess(string _cdfExecutable, string path)
        {
            Process p = new Process();
            ProcessInfo pInfo = new ProcessInfo();
            p.StartInfo.FileName = _cdfExecutable;
            p.StartInfo.Arguments = String.Format(@"-dummy {0}", path);
            p.StartInfo.UseShellExecute = false;
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(pInfo.ProcessExited);
            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.WriteLine("Filename: {0}", p.StartInfo.FileName);
                Console.WriteLine("CMD Argument: {0}", p.StartInfo.Arguments);
                Console.WriteLine();
            }

            string[] pathsplit = path.Split('\\');
            string product = pathsplit[pathsplit.Length - 2];
            string year = pathsplit[pathsplit.Length - 1];
            pInfo.Process = p;
            pInfo.CDFPath = product + year;
            pInfo.Clock = Stopwatch.StartNew();
            Console.WriteLine("Starting process # {0} for : {1}\n", pInfo.Process.Id, pInfo.CDFPath);
            Processes.Add(pInfo);
        }

        /// <summary>
        /// A loop that waits for all processes to end. 
        /// Processes are removed from ProcessInfo.Processes by event handler.
        /// Initiates printing of "Working . . ." to console if not in DEBUG mode.
        /// </summary>
        public static void WaitForProcesses()
        {
            while (Processes.Count() > 0)
            {
#if !DEBUG
                ConsoleWriter.Working(true);
#endif
            }
            ConsoleWriter.Working(false);
        }
    }
}
