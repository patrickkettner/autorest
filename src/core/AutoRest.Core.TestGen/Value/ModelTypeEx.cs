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

            var primaryType = type as PrimaryType;
            if (primaryType != null)
            {
                var jValue = value as JValue;
                switch (primaryType.KnownPrimaryType)
                {
                    case KnownPrimaryType.String:
                        return new PrimaryValue<string>(primaryType, jValue.ToObject<string>());
                    case KnownPrimaryType.Int:
                        return new PrimaryValue<int>(primaryType, jValue.ToObject<int>());
                    case KnownPrimaryType.Long:
                        return new PrimaryValue<long>(primaryType, jValue.ToObject<long>());
                    case KnownPrimaryType.Boolean:
                        return new PrimaryValue<bool>(primaryType, jValue.ToObject<bool>());
                    case KnownPrimaryType.Stream:
                        return new StreamValue(primaryType, jValue.ToObject<string>());
                    case KnownPrimaryType.DateTime:
                        return new DateTimeValue(primaryType, jValue.ToObject<DateTime>());
                    case KnownPrimaryType.TimeSpan:
                        return new TimeSpanValue(primaryType, jValue.ToObject<string>());
                }
                Console.Error.WriteLine($"Unknown Primary Type: {primaryType.KnownPrimaryType}");
                return null;
            }

            var sequenceType = type as SequenceType;
            if (sequenceType != null)
            {
                var jArray = value as JArray;
                if (jArray == null)
                {
                    return null;
                }
                var elementType = sequenceType.ElementType;
                return new SequenceValue(sequenceType, jArray.Select(v => elementType.CreateValueModel(v)));
            }

            var enumType = type as EnumType;
            if (enumType != null)
            {
                return new EnumValue(enumType, value.ToObject<string>());
            }

            var compositeType = type as CompositeType;
            if (compositeType != null)
            {
                return new CompositeValue(compositeType, value as JObject);
            }

            var dictionaryType = type as DictionaryType;
            if (dictionaryType != null)
            {
                var objectValue = value as JObject;
                if (objectValue == null)
                {
                    return null;
                }
                return new DictionaryValue(dictionaryType, objectValue);
            }

            Console.Error.WriteLine($"Unknown Type: {type.ToString()}");
            return null;
        }
    }
}
