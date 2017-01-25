// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace AutoRest.SdkTestServer
{
    /// <summary>
    /// Represents a harness capable of executing test cases.
    /// </summary>
    public abstract class TestRunner
    {
        /// <summary>
        /// Finds all test case files based on the provided parameters and executes them.
        /// </summary>
        /// <param name="directory">The directory in which to search for test files.</param>
        /// <param name="pattern">The pattern used to search for test files.</param>
        /// <returns>True if one or more test cases fail (i.e. their process has a non-zero exit code).</returns>
        public abstract bool Execute(string directory, string pattern);

        /// <summary>
        /// Spawns a new process with the specified name and arguments and waits for it to exit.
        /// </summary>
        /// <param name="name">The name of the executable to start.</param>
        /// <param name="args">The arguments to be passed to the executable.</param>
        /// <returns>The exit code of the process.</returns>
        protected virtual int StartAndWaitForProcess(string name, string args)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = name;
            psi.Arguments = args;

            var p = new Process();
            p.StartInfo = psi;
            p.Start();

            p.WaitForExit();
            return p.ExitCode;
        }

        /// <summary>
        /// Returns an array of files that match the specified search pattern in the specified directory.
        /// </summary>
        /// <param name="directory">The directory in which to search for test files.</param>
        /// <param name="pattern">The pattern used to search for test files.</param>
        /// <returns>An array of fully qualified file names.</returns>
        protected virtual string[] GetTestFiles(string directory, string pattern)
        {
            var files = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                throw new ArgumentException($"No tests were found in {directory} matching the pattern {pattern}.");
            }

            return files;
        }
    }
}
