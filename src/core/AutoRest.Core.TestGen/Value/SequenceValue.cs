using AutoRest.Core.Model;
using System.Collections.Generic;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class SequenceValue : ValueBase<SequenceType>
    {
        public IEnumerable<ValueBase> Value { get; }

        public SequenceValue(SequenceType sequenceType, IEnumerable<ValueBase> value): base(sequenceType)
        {
            Value = value;
        }
    }
}
