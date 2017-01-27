namespace AutoRest.Core.TestGen.Value
{
    public abstract class PrimaryValue : ValueBase
    {
    }

    public sealed class PrimaryValue<T> : PrimaryValue
    {
        public T Value { get; }

        public PrimaryValue(T value)
        {
            Value = value;
        }
    }
}
