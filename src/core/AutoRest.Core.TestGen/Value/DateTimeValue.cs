using System;

namespace AutoRest.Core.TestGen.Value
{
    public sealed class DateTimeValue : ValueBase
    {
        public DateTime Value { get; }

        public DateTimeValue(DateTime value)
        {
            Value = value;
        }

        public string GetWebUtcString()
            => Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}
