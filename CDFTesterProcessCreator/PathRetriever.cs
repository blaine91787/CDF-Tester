using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    /// <summary>
    /// PathRetriever is in charge of finding paths that contain CDF files.
    /// </summary>
    public static class PathRetriever
    {
        /// <summary>
        /// A list of parent directories to prevent double adding during recursion.
        /// </summary>
        private static List<string> _parentDirectories = new List<string>();

        /// <summary>
        /// Gets a list of paths containting CDF files.
        /// </summary>
        /// <param name="paths">An IEnumerable containing root directories containing CDF files</param>
        /// <returns>A list of paths which contain CDF files</returns>
        public static List<string> GetPaths(IEnumerable<string> paths)
        {
            List<string> cdfPaths = new List<string>();

            foreach(string path in paths)
            {
                // HACK: In the case of FTECS data archive and data processing folders, if it's not a number (a year) it's a parent directory.
                int x = 0;
                if(!Int32.TryParse(path.Split('\\').Last(), out x))
                    _parentDirectories.Add(path);
                RecursivelyGetPaths(path, cdfPaths);
            }

            return cdfPaths;
        }

        /// <summary>
        /// Creates a list of paths containing CDF files using depth first search.
        /// </summary>
        /// <param name="path">The current directory to check for CDFs and add to the list of CDF paths</param>
        /// <param name="cdfPaths">A list to add each CDF directory</param>
        private static void RecursivelyGetPaths(string path, List<string> cdfPaths)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
                if (dirs.Length != 0)
                    RecursivelyGetPaths(dir, cdfPaths);

            if (!_parentDirectories.Contains(path))
            {
                _parentDirectories.Add(Directory.GetParent(path).FullName);
                if (Directory.GetFiles(path, "*.cdf").Count() != 0)
                    cdfPaths.Add(path);
            }
        }
    }
}
