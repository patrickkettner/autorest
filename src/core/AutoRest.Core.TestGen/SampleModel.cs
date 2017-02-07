using AutoRest.Core.Model;
using AutoRest.Core.TestGen.Value;
using AutoRest.Core.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoRest.Core.TestGen
{
    public sealed class SampleModel
    {
        public string OperationId { get; }

        public Method Method { get; }

        public IEnumerable<Parameter> Parameters { get; }

        public ValueBase ReturnValue { get; }

        public SampleModel(CodeModel model, Sample sample)
        {
            OperationId = sample.OperationId;

            var methodSeq = model.Methods.Where(m => m.SerializedName == sample.OperationId);
            if (!methodSeq.Any())
            {
                Console.Error.WriteLine($"Unknown operation: {sample.OperationId}");
            }
            Method = methodSeq.First();

            Parameters = Method.Parameters.Select(p => new Parameter(sample.Parameters, p));

            var returnType = Method.ReturnType.Body;
            ReturnValue = returnType == null ? null : returnType.CreateValueModel(GetReturnValue(sample));
        }

        public static IEnumerable<SampleModel> GetSampleModelSeq(CodeModel codeModel, Settings settings)
        {
            var input = settings.Input;
            var files = Directory.GetFiles($"{input}.tests\\");

            var fileSystem = new FileSystem();

            return files.Select(file => new SampleModel(codeModel, Sample.Load(file, fileSystem)));
        }

        private static JToken GetReturnValue(Sample sample)
        {
            if (sample.Responses == null)
            {
                return null;
            }
            Response result = null;
            sample.Responses.TryGetValue("200", out result);
            return result == null ? null : result.Body;
        }
    }
}
