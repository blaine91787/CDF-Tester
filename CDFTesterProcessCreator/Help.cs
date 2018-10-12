using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    /// <summary>
    /// Help class provides methods for console usage examples.
    /// </summary>
    public class Help
    {
        /// <summary>
        /// Get method simply writes to console what arguments are accepted.
        /// </summary>
        public static void Get()
        {
            Console.WriteLine();
            Console.WriteLine("  Arguments:\n");
            Console.WriteLine("    > One or more parent directories whose children contain CDF files.");
            Console.WriteLine("        > Uses depth first search to find directories.");
            Console.WriteLine("        > If no CDF files exist, the program will exit.\n");
            Console.WriteLine();
            Console.WriteLine("  -h     : help");
            Console.WriteLine("  -coll  : CDFTester will write log to one file versus many.");
            Console.WriteLine();
        }

        /// <summary>
        /// NoArgs method gives a hint to use -h when the program is not provided any arguments.
        /// </summary>
        public static void NoArgs()
        {
            Console.WriteLine();
            Console.WriteLine("  No arguments given.");
            Console.WriteLine();
            Console.WriteLine("  Use -h to view usage.");
            Console.WriteLine();
        }
    }
}
