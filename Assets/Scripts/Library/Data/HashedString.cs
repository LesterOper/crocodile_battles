namespace Library.Data
{
    public class HashedString : HashedData<string>
    {
        public HashedString(string name, string value, string hash) : base(name, value, hash)
        {
        }

        public HashedString(string name) : this(name, "", "")
        {
        }

        public HashedString()
        {
        }

        public override bool IsDefault()
        {
            return "".Equals(Value);
        }

        protected override string GetDefault()
        {
            return "";
        }
    }
}