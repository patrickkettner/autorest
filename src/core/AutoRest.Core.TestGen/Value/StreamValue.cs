namespace AutoRest.Core.TestGen.Value
{
    public sealed class StreamValue : ValueBase
    {
        public string Value { get; }

        public StreamValue(string value)
        {
            Value = value;
        }
    }
}
