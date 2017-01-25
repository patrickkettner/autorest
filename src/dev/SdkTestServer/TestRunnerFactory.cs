// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace AutoRest.SdkTestServer
{
    /// <summary>
    /// Factory class for creating test runners.
    /// </summary>
    public class TestRunnerFactory
    {
        /// <summary>
        /// Creates a test runner based on the specified test runner type.
        /// </summary>
        /// <param name="testRunner">The type of test runner to create.</param>
        /// <returns>An test runner object.</returns>
        public static TestRunner CreateTestRunner(TestRunnerType testRunner)
        {
            switch (testRunner)
            {
                case TestRunnerType.Go:
                    return new TestRunners.TestRunnerGo();
                default:
                    throw new ArgumentException($"unsupported test runner type {testRunner}");
            }
        }
    }
}
