// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace AutoRest.SdkTestServer
{
    public class Program
    {
        static ManualResetEventSlim s_start = new ManualResetEventSlim();
        static ManualResetEventSlim s_exit = new ManualResetEventSlim();

        public static int Main(string[] args)
        {
            var options = ((Parsed<Options>)Parser.Default.ParseArguments<Options>(args)).Value;

            ResponseParser.Initialize(options.ResponseJson);

            // create thread to list for HTTP requests
            var hostThread = new Thread(new ParameterizedThreadStart(WebHost));
            hostThread.Start(options);

            // wait for the HTTP listener to start
            s_start.Wait();

            // now run the tests
            var testRunner = TestRunnerFactory.CreateTestRunner(options.TestRunner);
            bool failed = testRunner.Execute(options.TestDirectory, options.TestFiles);

            // tests are finished, signal the listener to exit
            s_exit.Set();

            // if any tests failed return non-zero exit code
            if (failed)
            {
                return 1;
            }

            return 0;
        }

        private static void WebHost(object o)
        {
            var options = (Options)o;

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls($"http://*:{options.PortNumber}")
                .UseStartup<Startup>()
                .Build();

            host.Start();

            // signal host has started
            s_start.Set();

            // wait for signal to exit
            s_exit.Wait();
            host.Dispose();
        }
    }
}
