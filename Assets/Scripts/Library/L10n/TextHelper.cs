using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Library.L10n
{
    internal class TextHelper
    {
        private const string Folder = "Languages/";

        private static string _currentLocaleKey;
        private static Dictionary<string, string> _textTable;

        public TextHelper()
        {
            _textTable = new Dictionary<string, string>();
        }

        internal void SetLocale([NotNull] string localeKey)
        {
            if (localeKey.Equals(_currentLocaleKey))
                return;

            _textTable.Clear();

            foreach (TextAsset textAsset in Resources.LoadAll(Folder + localeKey))
                PropertiesUtils.Load(_textTable, new MemoryStream(textAsset.bytes));

            _currentLocaleKey = localeKey;
        }

        public string GetText(string key)
        {
            if (key != null && _textTable.ContainsKey(key))
                return _textTable[key];
            return key;
        }

        public List<string> GetText(IEnumerable<string> keys)
        {
            var result = new List<string>();

            foreach (var key in keys) result.Add(GetText(key));
            return result;
        }
    }
}