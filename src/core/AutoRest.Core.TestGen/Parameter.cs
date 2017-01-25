using AutoRest.Core.Model;
using AutoRest.Core.TestGen.Value;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AutoRest.Core.TestGen
{
    public sealed class Parameter
    {
        public string Name { get; }

        public ValueBase Value { get; }

        public Parameter(Dictionary<string, JToken> parameters, Model.Parameter p)
        {
            Name = p.SerializedName;
            JToken source;
            parameters.TryGetValue(Name, out source);
            Value = p.ModelType.CreateValueModel(source);
        }
    }
}
