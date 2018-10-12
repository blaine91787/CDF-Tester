using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    /// <summary>
    /// A class to handle the pretty printing of the console during CDFTester execution.
    /// </summary>
    public static class ConsoleWriter
    {
        /// <summary>
        /// The original row of the cursor position.
        /// ConsoleWriter manipulates the console, so this keeps track of position.
        /// </summary>
        public static int OrigRow { get; set; }

        /// <summary>
        /// The original column of the cursor position.
        /// ConsoleWriter manipulates the console, so this keeps track of position.
        /// </summary>
        public static int OrigCol { get; set; }
        
        /// <summary>
        /// Resets row and column to the beginning of console.
        /// To be used after a Console.Clear()
        /// </summary>
        public static void ResetColRow()
        {
            OrigRow = 0;
            OrigCol = 0;
        }

        /// <summary>
        /// Writes to console given cartesian-like coordinate system based on OrigRow and OrigCol as the origin.
        /// </summary>
        /// <param name="str">The string to be written to console.</param>
        /// <param name="x">The column to begin writing the string.</param>
        /// <param name="y">The row to write the string to.</param>
        /// <param name="clear">Whether the line should be cleared before writing.</param>
        public static void WriteAtPosition(string str, int x, int y, bool clear = false)
        {
            try
            {
                Console.SetCursorPosition(OrigCol + x, OrigRow + y);

                if (clear)
                    ClearLine();

                Console.Write(str);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Clears the line y-rows away from the current cursor position.
        /// </summary>
        /// <param name="y">The number of lines away from the current cursor user wants to clear.</param>
        public static void ClearLine(int y = 0)
        {
            int currentLineCursor = Console.CursorTop + y;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <summary>
        /// Writes an animated "Working..." to the console.
        /// </summary>
        /// <param name="waiting"></param>
        /// <param name="procNotExited"></param>
        public static void Working(bool waiting = true, bool procNotExited = true)
        {
            // Since process exited event triggers printing of status above "Working..." statement.
            // HACK: waiting and procNotExited are used to determine when to exit and reprint.
            // Only kind of works.
            if (waiting && procNotExited)
            {
                WriteAtPosition("Working ", 0, 0, true);
            }
            else return;
            if (waiting && procNotExited)
            {
                Thread.Sleep(1000);
                WriteAtPosition(". ", Console.CursorLeft, 0);
            }
            else return;
            if (waiting && procNotExited)
            {
                Thread.Sleep(1000);
                WriteAtPosition(". ", Console.CursorLeft, 0);
            }
            else return;
            if (waiting && procNotExited)
            {
                Thread.Sleep(1000);
                WriteAtPosition(". ", Console.CursorLeft, 0);
            }
            else return;
            if (waiting && procNotExited)
            {
                Thread.Sleep(1000);
            }
            else return;
        }

        /// <summary>
        /// Prints to console details about the exited process.
        /// </summary>
        /// <param name="proc">The ProcessInfo instance of the exited process.</param>
        /// <param name="procCount">Number of processes still running.</param>
        public static void ProcessExited(ProcessInfo proc, int procCount)
        {
            string tempstr = String.Format("{0} proccesses left. ", procCount);
#if !DEBUG
            ClearLine(-1);
#endif
            WriteAtPosition(tempstr, 0, 0);
            Console.WriteLine("{0} has exited.\n", proc.CDFName);
            OrigRow = Console.CursorTop;
        }

        /// <summary>
        /// Prints the total execution time to the console.
        /// </summary>
        /// <param name="time">Elapsed time as a prettified string.</param>
        public static void ElapsedTime(string time)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("\nTotal execution time: " + time);
        }
    }
}
