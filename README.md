# CDF-Tester
A simple utility to check if CDF files are corrupted.

Useful for large sets of CDF files, but will work on few as well.

Scans a root folder using depth first search looking for CDF files in sub-directories. For each sub-directory a process is created to search folders in parallel.

Originally made to check ~150,000 CDF files.
