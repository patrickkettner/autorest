using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
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

        public SampleModel(CodeModel model, Sample sample)
        {
            OperationId = sample.OperationId;
            Method = model.Methods.First(m => m.SerializedName == sample.OperationId);
            Parameters = Method.Parameters.Select(p => new Parameter(sample.Parameters, p));
        }

        public static IEnumerable<SampleModel> GetSampleModelSeq(CodeModel codeModel, Settings settings)
        {
            var input = settings.Input;
            var files = Directory.GetFiles($"{input}.tests\\");

            var fileSystem = new FileSystem();

            return files.Select(file => new SampleModel(codeModel, Sample.Load(file, fileSystem)));
        }

    }
}
