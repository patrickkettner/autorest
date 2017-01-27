using AutoRest.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AutoRest.Core.TestGen.Value
{
    public static class ModelTypeEx
    {
        public static ValueBase CreateValueModel(this IModelType type, JToken value)
        {
            if (value == null)
            {
                return null;
            }

            var primiryType = type as PrimaryType;
            if (primiryType != null)
            {
                var jValue = value as JValue;
                switch (primiryType.KnownPrimaryType)
                {
                    case KnownPrimaryType.String:
                        return new PrimaryValue<string>(jValue.ToObject<string>());
                    case KnownPrimaryType.Long:
                        return new PrimaryValue<long>(jValue.ToObject<long>());
                    case KnownPrimaryType.Stream:
                        return new StreamValue(jValue.ToObject<string>());
                }
                return null;
            }

            var sequenceType = type as SequenceType;
            if (sequenceType != null)
            {
                var jArray = value as JArray;
                var elementType = sequenceType.ElementType;
                return new SequenceValue(jArray.Select(v => elementType.CreateValueModel(v)));
            }

            var enumType = type as EnumType;
            if (enumType != null)
            {
                return new EnumValue(value.ToObject<string>());
            }

            return new CompositeValue((CompositeType)type, value as JObject);
        }
    }
}
