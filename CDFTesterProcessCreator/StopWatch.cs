using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    /// <summary>
    /// An abstraction of .NET Stopwatch. Provides stopwatch features for CDFTester.
    /// </summary>
    public class SWatch
    {
        /// <summary>
        /// Miliseconds to be acted upon by SWatch methods.
        /// </summary>
        public long MS { get; set; }

        /// <summary>
        /// The TimeSpan used to save the converted milliseconds to be displayed nicely.
        /// </summary>
        public TimeSpan TS { get; set; }

        /// <summary>
        /// An estimated time remaining for all processes to end.
        /// </summary>
        public string Average { get; set; }

        /// <summary>
        /// SWatch constructor.
        /// </summary>
        public SWatch()
        {
            MS = 0;
            TS = default(TimeSpan);
        }

        /// <summary>
        /// Converts the SWatch.MS property to a readable time and calls ConsoleWriter to print to console.
        /// </summary>
        /// <param name="showErrors">Set to true if errors should be printed to console.</param>
        public void PrintTime(bool showErrors = false)
        {
            try
            {
                TS = TimeSpan.FromMilliseconds(MS);
                string str = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                TS.Hours,
                                TS.Minutes,
                                TS.Seconds,
                                TS.Milliseconds);
                ConsoleWriter.ElapsedTime(str);
            }
            catch (Exception e)
            {
                if(showErrors)
                {
                    ConsoleWriter.WriteAtPosition(e.Message, 0, 0);
                }
            }
        }

        /// <summary>
        /// Calculates estimated time left for the program to run and saves to SWatch.Average property.
        /// </summary>
        /// <param name="numProcsExited">An integer representing the number of processes exited.</param>
        /// <param name="numProcsRunning">An integer representing the number of processes still running.</param>
        public void CalculateAverage(int numProcsExited, int numProcsRunning)
        {
            MS = MS / numProcsExited;
            MS = MS * numProcsRunning;
            TS = TimeSpan.FromMilliseconds(MS);
            Average = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        TS.Hours,
                                        TS.Minutes,
                                        TS.Seconds,
                                        TS.Milliseconds);
        }
    }
}
