#if DEBUG
    #define USEDUMMYDATA
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTester
{
    public class CDFT_Program
    {
        public static void Main(string[] args)
        {
            bool process = false;
            bool collection = false;
            if (args.Length == 0)
            {
                args = new string[]
                {
                    @"../../../../CDFTester/TestFiles",
                };
            }

            if (args.Contains("-proc"))
                process = true;

            if (args.Contains("-coll"))
                collection = true;

            FileTester ft = new FileTester();
            ft.Run(args, process, collection);
            if (!process)
                Console.ReadKey();
        }
    }
}
