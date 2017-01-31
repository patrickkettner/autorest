using AutoRest.Core.Model;

namespace AutoRest.Core.TestGen.Value
{
    public abstract class ValueBase
    {
        public abstract IModelType GetModelType();
    }

    public abstract class ValueBase<T> : ValueBase
        where T : IModelType
    {
        public T Type { get; }

        public override IModelType GetModelType()
            => Type;

        protected ValueBase(T type)
        {
            Type = type;
        }
    }
}
