using AutoRest.Core.Model;
using Newtonsoft.Json.Linq;

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
                        return new StringValue(jValue.ToObject<string>());
                }
                return null;
            }

            var sequenceType = type as SequenceType;
            if (sequenceType != null)
            {
                return new SequenceValue();
            }

            var enumType = type as EnumType;
            if (enumType != null)
            {
                return new EnumValue();
            }

            return new CompositeValue((CompositeType)type, value as JObject);
        }
    }
}
