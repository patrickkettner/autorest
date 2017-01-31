using AutoRest.Core.Model;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class StreamValue : ValueBase<PrimaryType>
    {
        public string Value { get; }

        public StreamValue(PrimaryType type, string value): base(type)
        {
            Value = value;
        }
    }
}
