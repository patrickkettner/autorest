using System.Collections.Generic;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class SequenceValue : ValueBase
    {
        public IEnumerable<ValueBase> Value { get; }

        public SequenceValue(IEnumerable<ValueBase> value)
        {
            Value = value;
        }
    }
}
