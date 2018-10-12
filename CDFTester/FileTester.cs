using CDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTester
{
    /// <summary>
    /// Represents a CDf file unable to be opened.
    /// Holds record of the last modified date before and after the attempt to open.
    /// </summary>
    public class Result
    {
        public string Path { get; set; }
        public string Exception { get; set; }
        public DateTime PrevModified { get; set; }
        public DateTime CurrModified { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// CDFTester is currently set up to take in an array of paths and recursively find
    /// subdirectories that contain CDF files. This process should happen in ProcessCreator
    /// and shouldn't be needed. I'm leaving since it only create one thread when used with
    /// CDFTesterProcessCreator.
    /// </remarks>
    public class FileTester
    {
        public bool IsAProcess { get; set; }
        public bool WriteAsACollection { get; set; }
        private bool Debug { get; set; }
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public List<Result> Results { get; private set; }

        public FileTester()
        {
            Results = new List<Result>();
        }

        /// <summary>
        /// Handles the process of testing the CDF.
        /// Makes the calls to other CDFTester methods.
        /// </summary>
        /// <param name="paths"></param>
        public void Run(string[] directoryStrings, bool process = false, bool collection = false)
        {
            IsAProcess = process;
            WriteAsACollection = collection;

            List<DirectoryInfo> directories = new List<DirectoryInfo>();
            ConvertToDirectoryInfo(directoryStrings, ref directories);

            List<DirectoryInfo> dirsToTest = new List<DirectoryInfo>();
            string productName = String.Empty;
            int productCount = 0;
            foreach (DirectoryInfo dir in directories)
            {
                if (dir.Exists)
                {
                    GetDirsContainingCDFs(dir, ref dirsToTest, ref productName, ref productCount);
                }
            }

            // If FileTester creates multiple directories to test, then change the name of log file
            // from using product names to simply a collection. Typically when CDTester is running
            // in debug mode as CDFTesterProcessCreator should be sending a single product/year to check.
            // Or if user requests to be written as a collection.
            if (productCount > 1 || WriteAsACollection)
                productName = "InvalidCdfCollection";

            List<Thread> threads = new List<Thread>();
            GetThreads(dirsToTest, ref threads);

            foreach (Thread thread in threads)
                thread.Start();

            foreach (Thread thread in threads)
                thread.Join();

            FileWriter fw = new FileWriter();
            fw.Write(Results, productName, WriteAsACollection, IsAProcess);
        }

        /// <summary>
        /// Converts an array of directory path strings into DirectoryInfo objects then adds to a list reference
        /// </summary>
        /// <param name="directoryStrings">An array of directory paths to be converted to DirectoryInfo objects</param>
        /// <param name="listOfDirectories">A list to be populated with DirectoryInfo objects.</param>
        private void ConvertToDirectoryInfo(string[] directoryStrings, ref List<DirectoryInfo> listOfDirectories)
        {
            foreach (string directory in directoryStrings)
            {
                if (Directory.Exists(directory))
                    listOfDirectories.Add(new DirectoryInfo(directory));
            }
        }

        /// <summary>
        /// For each path in the paths list, create a thread, and add to the reference variable: threads.
        /// </summary>
        private void GetThreads(List<DirectoryInfo> directories, ref List<Thread> threads)
        {
            threads = new List<Thread>();
            foreach (DirectoryInfo dir in directories)
            {
                Thread th;
                th = new Thread(delegate ()
                {
                    TryReadingCDFThread(dir);
                });
                threads.Add(th);
            }
        }

        /// <summary>
        /// Gets all the directories in the path provided and makes a call to GetDirsWithCDFs to
        /// recursively find CDFs in subdirectories.
        /// </summary>
        /// <param name="path">A directory whose subdirectories may contain CDF files.</param>
        private void GetDirsContainingCDFs(DirectoryInfo directory, ref List<DirectoryInfo> dirs, ref string productName, ref int productCount)
        {
            string temp = String.Empty;
            if (directory.GetDirectories().Count() > 0)
            {
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    GetDirsContainingCDFs(dir, ref dirs, ref productName, ref productCount);
                }
            }

            if (directory.GetFiles("*.cdf").Count() > 0)
            {
                // Get the product name and the year, save as productName
                // ex: ...\\TOFxEH\\2013
                string[] dirSplit = directory.FullName.Split('\\');
                productName = dirSplit[dirSplit.Count() - 2] + dirSplit[dirSplit.Count() - 1];
                dirs.Add(directory);
                productCount++;
            }
        }

        /// <summary>
        /// For each file in the directory provided, attempts to open the CDF.
        /// If there's an error (corrupted file), the error is caught and saved as a Result object
        /// to be written to file later.
        /// </summary>
        /// <param name="path">A directory containing CDF files.</param>
        private void TryReadingCDFThread(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                // If this instance of CDFTester was _not_ started by CDFTPC, then print to console.
                if(!IsAProcess)
                    Console.WriteLine(file.FullName);
                // Used to catch if we're updating last modified dates.
                // (Bug was found in CS_CDF software, but should be fixed)
                // This has been modified to not use CS_CDF.
                DateTime prevmod = file.LastWriteTime;
                DateTime currmod = default(DateTime);

                // Try opening file, close file, and check if last modified date has changed.
                // If error occurs, create result object to write to file later.
                if (file.Extension == ".cdf")
                {
                    IntPtr _fileID = default(IntPtr);
                    int curStatus;
                    try
                    {
                        try
                        {
                            unsafe
                            {
                                void* id = null;
                                try
                                {
                                    locker.EnterWriteLock();
                                    curStatus = CDFAPIs.CDFopenCDF(file.FullName, &id);
                                }
                                catch (AccessViolationException exc) { throw exc; }
                                _fileID = (IntPtr)id;
                            }
                        }
                        catch (Exception exc) { throw exc; }

                        try { unsafe { curStatus = CDFAPIs.CDFcloseCDF((void*)_fileID); } }
                        catch (Exception exc) { throw exc; }

                        file.Refresh();
                        currmod = file.LastWriteTime;
                        if (prevmod != currmod)
                            throw new DateModChangeException();
                    }
                    catch (Exception e)
                    {
                        if (e.GetType() == typeof(DateModChangeException))
                            throw new DateModChangeException();

                        file.Refresh();
                        currmod = file.LastWriteTime;

                        Result res = new Result
                        {
                            Path = file.FullName,
                            Exception = e.Message,
                            PrevModified = prevmod,
                            CurrModified = currmod,
                        };

                        Results.Add(res);

                        if (locker.IsWriteLockHeld)
                            locker.ExitWriteLock();
                    }
                    finally
                    {
                        if (locker.IsWriteLockHeld)
                            locker.ExitWriteLock();
                    }
                }
            }
        }
    }
}
