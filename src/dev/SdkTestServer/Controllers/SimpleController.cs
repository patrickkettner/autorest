// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AutoRest.SdkTestServer.Controllers
{
    /// <summary>
    /// This controller will act on all routes.
    /// </summary>
    public class SimpleController : Controller
    {
        [Route("{*route}")]
        public JsonResult Index(string route)
        {
            // check that all query strings are specified with the expected values
            foreach (var qs in ResponseParser.Instance.QueryStrings)
            {
                if (!Request.Query.ContainsKey(qs.Key))
                    return MakeError("bad query", $"missing query string '{qs.Key}'");

                if (!Request.Query[qs.Key].SequenceEqual(qs.Value))
                    return MakeError("bad query", $"bad value '{qs.Value}' for '{qs.Key}'");
            }

            // if any parameters were provided verify they match what's expected
            var param = ResponseParser.Instance.GetParameters(route, Request.Method);
            if (param != null)
            {
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEnd();
                }

                var jbody = JObject.Parse(body);

                // compare param value with jbody
                if (!JToken.DeepEquals(param.Value, jbody))
                {
                    // TODO: clean up
                    Console.WriteLine("Request params don't match expected values.");
                }
            }

            // return the appropriate response
            var resp = ResponseParser.Instance.GetResponse(route, Request.Method);
            if (resp != null)
            {
                // json will contain the status code and the response body

                var statusCode = int.Parse(resp.Name);
                var respBody = resp.Value.SelectToken("body");

                var jr = new JsonResult(respBody);
                jr.StatusCode = statusCode;
                return jr;
            }

            return MakeError("bad request", $"no response for '{Request.Method}:{route}'");
        }

        private JsonResult MakeError(string code, string message)
        {
            var error = string.Format("{{ \"error\": {{ \"code\": \"{0}\", \"message\": \"{1}\" }} }}", code, message);
            var jr = new JsonResult(JObject.Parse(error));
            jr.StatusCode = 400;
            return jr;
        }
    }
}
