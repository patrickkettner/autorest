using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AutoRest.Core.TestGen
{
    public sealed class Response
    {
        public Dictionary<string, string> Headers { get; set; }

        public JToken Body { get; set; }
    }
}
