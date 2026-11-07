using SimpleSQL;

namespace Library.Data
{
    public abstract class HashedData<T>
    {
        protected HashedData()
        {
        }

        protected HashedData(string prefName, T value, string hash)
        {
            PrefName = prefName;
            Value = value;
            Hash = hash;
        }

        public HashedData(string name)
        {
            PrefName = name;
        }

        [PrimaryKey] public string PrefName { get; set; }

        public T Value { get; set; }
        public string Hash { get; set; }

        public abstract bool IsDefault();
        protected abstract T GetDefault();

        public string GetStringForHash()
        {
            return "" + PrefName + Value;
        }

        public void Reset()
        {
            Value = GetDefault();
            Hash = "";
        }
    }
}