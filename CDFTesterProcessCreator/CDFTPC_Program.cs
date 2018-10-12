#if DEBUG
    #define USEDUMMYDATA
#endif

using CDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public class CDFTPC_Program
    {
        /// <summary>
        /// The main entry point for the CDFTester Solution.
        /// Starts at a directory root and recursively starts processes for folders containing CDFs.
        /// CDFTPC then creates processes for each directory using the CDFTester software.
        /// Once all processes complete the CDFTPC continues and exits.
        /// </summary>
        /// <param name="args">One or more root directories containing CDFs. May also be -h for usage help.</param>
        public static void Main(string[] args)
        {

            // If no directories are provided while in debug mode this replaces args with programmer defined directories.
#if DEBUG
            if (args.Length == 0)
            {
                args = new string[]
                {
                    @"../../../../CDFTester/TestFiles",
                };
            }
#endif

            if (args.Length == 0)
            {
                Help.NoArgs();
            }
            else if (args.Contains("-h"))
            {
                Help.Get();
            }
            else
            {
                bool collection = false;
                if (args.Contains("-coll"))
                    collection = true;

                ProcessCreator pc = new ProcessCreator();
                pc.Run(args, collection);

                Console.WriteLine("CDFTester has finished...\n");
            }

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
