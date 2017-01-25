using AutoRest.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class CompositeValue : ValueBase
    {
        public CompositeType Type { get; }

        public IEnumerable<Tuple<string, ValueBase>> Properties { get; }

        public CompositeValue(CompositeType type, JObject value)
        {
            Type = type;
            Properties = type.Properties.Select(c => Tuple.Create(
                c.SerializedName.RawValue,
                c.ModelType.CreateValueModel(value[c.SerializedName])));
        }
    }
}
