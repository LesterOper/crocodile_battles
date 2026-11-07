using SimpleSQL;

namespace Library.Data
{
    public class PrefLong
    {
        [PrimaryKey] public string PrefName { get; set; }

        public long Value { get; set; }
    }
}