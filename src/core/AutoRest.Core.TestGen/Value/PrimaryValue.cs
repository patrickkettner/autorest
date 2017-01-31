using AutoRest.Core.Model;

namespace AutoRest.Core.TestGen.Value
{
    public abstract class PrimaryValue : ValueBase<PrimaryType>
    {
        protected PrimaryValue(PrimaryType type) : base(type) { }
    }

    public sealed class PrimaryValue<T> : PrimaryValue
    {
        public T Value { get; }

        public PrimaryValue(PrimaryType type, T value): base(type)
        {
            Value = value;
        }
    }
}
