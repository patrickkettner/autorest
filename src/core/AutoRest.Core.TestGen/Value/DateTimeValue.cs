using AutoRest.Core.Model;
using System;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class DateTimeValue : ValueBase<PrimaryType>
    {
        public DateTime Value { get; }

        public DateTimeValue(PrimaryType type, DateTime value): base(type)
        {
            Value = value;
        }

        public string GetWebUtcString()
            => Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}
