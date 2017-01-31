using AutoRest.Core.Model;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class EnumValue : ValueBase<EnumType>
    {
        public string Value { get; }

        public EnumValue(EnumType type, string value): base(type)
        {
            Value = value;
        }
    }
}
