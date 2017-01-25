// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AutoRest.SdkTestServer.TestRunners
{
    /// <summary>
    /// Executes tests written in the Go programming language.
    /// </summary>
    public class TestRunnerGo : TestRunner
    {
        public override bool Execute(string directory, string pattern)
        {
            bool failed = false;

            var tests = GetTestFiles(directory, pattern);
            foreach (var test in tests)
            {
                if (StartAndWaitForProcess("go", $"run {test}") != 0)
                {
                    failed = true;
                }
            }

            return failed;
        }
    }
}
