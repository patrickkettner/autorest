namespace AutoRest.Core.TestGen.Value
{
    public sealed class StringValue : PrimiryValue
    {
        public string Value { get; }

        public StringValue(string value)
        {
            Value = value;
        }
    }
}
