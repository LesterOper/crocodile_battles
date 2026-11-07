namespace Library.Data
{
    public class HashedLong : HashedData<long>
    {
        public HashedLong(string name, long value, string hash) : base(name, value, hash)
        {
        }

        public HashedLong(string name) : this(name, 0, "")
        {
        }

        public HashedLong()
        {
        }

        public override bool IsDefault()
        {
            return Value == 0;
        }

        protected override long GetDefault()
        {
            return 0;
        }
    }
}