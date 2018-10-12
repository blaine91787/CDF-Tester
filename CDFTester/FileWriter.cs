using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTester
{
    public class FileWriter
    {
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public void Write(List<Result> results, string productName, bool collection, bool process)
        {
            FileInfo logFile = GetLogFile(productName, collection, process);

            if (results.Count > 0)
            {
                try
                {
                    using (var mutex = new Mutex(false, "INVALID_CDF_LOG_FILE_MUTEX"))
                    {
                        mutex.WaitOne();
                        using (StreamWriter sw = new StreamWriter(logFile.FullName, true))
                        {
                            if (collection)
                            {
                                if (logFile.LastWriteTime.Day < DateTime.Now.Day)
                                    File.WriteAllText(logFile.FullName, String.Empty);
                            }
                            foreach (Result result in results)
                            {
                                sw.WriteLine("############    {0}    ############", result.Exception);
                                sw.WriteLine("Path: {0}", result.Path);
                                sw.WriteLine("Last modify before running: {0}", result.PrevModified);
                                sw.WriteLine("Last modify after running: {0}", result.CurrModified);
                                sw.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------");
                            }
                        }
                        mutex.ReleaseMutex();
                    }

                }
                catch
                {
                    Console.WriteLine("Unable to access file: " + logFile);
                }
                finally
                {
                    //locker.ExitWriteLock();
                }
            }
        }

        private FileInfo GetLogFile(string productName, bool collection, bool process)
        {
            // Create desktop folder if one doesn't exist.
            string logPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            logPath = Path.Combine(logPath, "INVALID_CDFs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string logFile = String.Empty;
            if (!collection) // {Desktop}/INVALID_CDFs/TOFxEH2013_1.txt
            {
                int logNum = 1;
                logFile = Path.Combine(logPath, String.Format("{0}_{1}.txt", productName, logNum.ToString()));
                while (File.Exists(logFile))
                    logFile = Path.Combine(logPath, String.Format("{0}_{1}.txt", productName, (logNum++).ToString()));
            }
            else if (collection) // {Desktop}/INVALID_CDFs/InvalidCdfCollection.txt
            {
                logFile = Path.Combine(logPath, String.Format("{0}_{1}.txt", productName, DateTime.Now.ToString("yyMMdd")));
            }

            return new FileInfo(logFile);
        }
    }
}
