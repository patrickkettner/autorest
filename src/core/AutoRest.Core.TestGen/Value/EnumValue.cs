namespace AutoRest.Core.TestGen.Value
{
    public sealed class EnumValue : ValueBase
    {
        public string Value { get; }

        public EnumValue(string value)
        {
            Value = value;
        }
    }
}
