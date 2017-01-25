// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace AutoRest.SdkTestServer
{
    public class Options
    {
        [Option('d', "directory", Required = true)]
        public string TestDirectory { get; set; }

        [Option('t', "tests", Required = true)]
        public string TestFiles { get; set; }

        [Option('r', "runner", Required = true)]
        public TestRunnerType TestRunner { get; set; }

        [Option('p', "port", Required = true)]
        public int PortNumber { get; set; }

        [Option('j', "json", Required = true)]
        public string ResponseJson { get; set; }
    }
}
