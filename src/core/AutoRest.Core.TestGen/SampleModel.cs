using AutoRest.Core.Model;
using System.Collections.Generic;
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
    }
}
