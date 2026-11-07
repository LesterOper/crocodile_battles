using System;

namespace Library.Data
{
    public class HashedDouble : HashedData<double>
    {
        public HashedDouble(string name, double value, string hash) : base(name, value, hash)
        {
        }

        public HashedDouble(string name) : this(name, 0d, "")
        {
        }

        public HashedDouble()
        {
        }

        public override bool IsDefault()
        {
            return Math.Abs(Value) < double.Epsilon;
        }

        protected override double GetDefault()
        {
            return 0d;
        }
    }
}