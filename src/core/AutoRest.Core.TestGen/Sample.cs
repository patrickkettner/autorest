using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AutoRest.Core.TestGen
{
    public sealed class Sample
    {
        public string Title { get; set; }

        public string OperationId { get; set; }

        public Dictionary<string, JToken> Parameters { get; set; }

        public Dictionary<string, Response> Responses { get; set; }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseContractResolver()
        };

        public static Sample Load(string fileName, IFileSystem fileSystem)
            => JsonConvert.DeserializeObject<Sample>(fileSystem.ReadFileAsText(fileName), Settings);

        public SampleModel CreateModel(CodeModel model)
            => new SampleModel(model, this);
    }
}
