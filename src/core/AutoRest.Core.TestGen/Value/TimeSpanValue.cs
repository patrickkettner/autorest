using AutoRest.Core.Model;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class TimeSpanValue : ValueBase<PrimaryType>
    {
        public string Value { get; }

        public TimeSpanValue(PrimaryType type, string value) : base(type)
        {
            Value = value;
        }
    }
}
