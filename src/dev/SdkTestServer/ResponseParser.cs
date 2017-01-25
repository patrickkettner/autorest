// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutoRest.SdkTestServer
{
    /// <summary>
    /// Manages a set of responses indexed by route.
    /// </summary>
    public class ResponseParser
    {
        /// <summary>
        /// Manages a set of responses indexed by HTTP request verb.
        /// </summary>
        private class Response
        {
            /// <summary>
            /// Manages a set of responses indexed by test prefix.
            /// </summary>
            private class ResponseMgr
            {
                private Dictionary<string, JProperty> _responses;

                public ResponseMgr(JEnumerable<JToken> responses)
                {
                    _responses = new Dictionary<string, JProperty>(StringComparer.OrdinalIgnoreCase);

                    foreach (var response in responses)
                    {
                        var asProp = (JProperty)response;
                        _responses.Add(asProp.Name, asProp);
                    }
                }

                /// <summary>
                /// Returns a response based on the provided test prefix or null if no response exists.
                /// </summary>
                /// <param name="prefix">The test prefix.</param>
                /// <returns>A JProperty object containing the response code and body or null.</returns>
                public JProperty Get(string prefix)
                {
                    if (!_responses.ContainsKey(prefix))
                    {
                        return null;
                    }

                    return _responses[prefix];
                }
            }

            // responses per verb
            private Dictionary<string, ResponseMgr> _responses;

            public Response(JEnumerable<JToken> responses)
            {
                _responses = new Dictionary<string, ResponseMgr>(StringComparer.OrdinalIgnoreCase);

                foreach (var response in responses)
                {
                    var respProp = (JProperty)response;
                    _responses.Add(respProp.Name, new ResponseMgr(respProp.Value.Children()));
                }
            }

            /// <summary>
            /// Returns parameters based on the provided verb or null if no parameters exist.
            /// </summary>
            /// <param name="verb">The request verb.</param>
            /// <returns>A JProperty object containing the parameters or null.</returns>
            public JProperty GetParametersForVerb(string verb)
            {
                if (!_responses.ContainsKey(verb))
                {
                    return null;
                }

                return _responses[verb].Get("parameters");
            }

            /// <summary>
            /// Returns a response based on the provided verb and prefix or null if no response exists.
            /// </summary>
            /// <param name="verb">The request verb.</param>
            /// <param name="prefix">The test prefix.</param>
            /// <returns>A JProperty object containing the response code and body or null.</returns>
            public JProperty GetResponseForVerb(string verb, string prefix)
            {
                if (!_responses.ContainsKey(verb))
                {
                    return null;
                }

                return _responses[verb].Get(prefix);
            }
        }

        private Dictionary<string, Response> _responseMap;

        /// <summary>
        /// Initializes a new instance of the ResponseParser class using the specified response JSON file.
        /// </summary>
        /// <param name="responseJson">The response JSON file to be loaded.</param>
        public ResponseParser(string responseJson)
        {
            JObject doc;
            using (var tr = File.OpenText(responseJson))
            {
                using (var jr = new JsonTextReader(tr))
                {
                    doc = JObject.Load(jr);
                }
            }

            // load query string data

            var qs = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);

            var queryStrings = doc["query"].Children();
            foreach (var queryString in queryStrings)
            {
                var qsProp = (JProperty)queryString;
                qs.Add(qsProp.Name, qsProp.Value.ToString());
            }

            QueryStrings = qs;

            // now populate the response map

            _responseMap = new Dictionary<string, Response>(StringComparer.OrdinalIgnoreCase);

            var routes = doc.SelectTokens("routes.*", true);
            foreach (var route in routes)
            {
                var path = GetRouteFromJSONPath(route.Path);
                _responseMap.Add(path, new Response(route.Children()));
            }
        }

        /// <summary>
        /// Gets a dictionary of query strings and their values.
        /// </summary>
        public IReadOnlyDictionary<string, StringValues> QueryStrings { get; }

        /// <summary>
        /// Returns the parameters based on the provided path and verb or null if no parameters exists.
        /// </summary>
        /// <param name="route">The URL route of the request.</param>
        /// <param name="verb">The request verb.</param>
        /// <returns>A JProperty object containing the parameters or null.</returns>
        public JProperty GetParameters(string route, string verb)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException(nameof(route));
            }

            if (string.IsNullOrWhiteSpace(verb))
            {
                throw new ArgumentException(nameof(verb));
            }

            var tuple = SanitizePath(route);
            if (!_responseMap.ContainsKey(tuple.Item2))
            {
                return null;
            }

            return _responseMap[tuple.Item2].GetParametersForVerb(verb);
        }

        /// <summary>
        /// Returns a response based on the provided path and verb or null if no response exists.
        /// </summary>
        /// <param name="route">The URL route of the request.</param>
        /// <param name="verb">The request verb.</param>
        /// <returns>A JProperty object containing the reponse code and body or null.</returns>
        public JProperty GetResponse(string route, string verb)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException(nameof(route));
            }

            if (string.IsNullOrWhiteSpace(verb))
            {
                throw new ArgumentException(nameof(verb));
            }

            var tuple = SanitizePath(route);
            if (!_responseMap.ContainsKey(tuple.Item2))
            {
                return null;
            }

            return _responseMap[tuple.Item2].GetResponseForVerb(verb, tuple.Item1);
        }

        /// <summary>
        /// Remotes the test prefix from the route and returns a prefix/route tuple.
        /// </summary>
        /// <param name="route">The URL route with the test prefix.</param>
        /// <returns>A tuple containing the test prefix and route.</returns>
        private Tuple<string, string> SanitizePath(string route)
        {
            // path should have the test prefix as the first part
            // something like test123/foo/bar/baz

            var i = route.IndexOf('/');
            var prefix = route.Substring(0, i);
            route = route.Substring(i);
            return new Tuple<string, string>(prefix, route);
        }

        /// <summary>
        /// Extracts the route from a JSON path.
        /// </summary>
        /// <param name="path">The JSON path from which to extract the route.</param>
        /// <returns>A URL route.</returns>
        private string GetRouteFromJSONPath(string path)
        {
            // looks something like routes['/foo/bar/baz/']
            // we want just the /foo/bar/baz/ part.
            return path.Substring(8, path.Length - 10);
        }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static ResponseParser Instance { get; private set; }

        /// <summary>
        /// Initializes the ResponseParser instance with the provided response JSON file.
        /// </summary>
        /// <param name="responseJson">A response JSON file.</param>
        public static void Initialize(string responseJson)
        {
            Instance = new ResponseParser(responseJson);
        }
    }
}
