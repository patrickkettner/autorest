using AutoRest.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class DictionaryValue : ValueBase<DictionaryType>
    {
        public IEnumerable<Tuple<string, ValueBase>> Properties { get; }

        public IModelType ValueType { get; }

        public DictionaryValue(DictionaryType type, JObject value) : base(type)
        {
            ValueType = type.ValueType;
            Properties = value.Properties()
                .Select(p => Tuple.Create(p.Name, ValueType.CreateValueModel(p.Value)));
        }
    }
}
