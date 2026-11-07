using Library.Utils;

namespace Library.Data
{
    public class ObusfactedLong
    {
        private long _value1;
        private long _value2;

        public ObusfactedLong() : this(0)
        {
        }

        public ObusfactedLong(long value)
        {
            Value = value;
        }

        public long Value
        {
            get => _value1 + _value2;
            set
            {
                _value1 = value >= 0
                    ? MathA.Random(0, MathA.Ceil(value * 0.9d))
                    : MathA.Random(MathA.Ceil(value * 0.9d), 0);
                _value2 = value - _value1;
            }
        }
    }
}