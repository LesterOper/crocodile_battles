using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Library.Common;
using UnityEngine;

namespace Library.L10n
{
    [DisallowMultipleComponent]
    public class LocaleHelper : MonoBehaviour
    {
        public const string PrefName = "LocaleHelper_key";

        [SerializeField] private LocaleItem[] _localeItems;

        private string _localeKey;
        private TextHelper _textHelper;

        private void Awake()
        {
            _textHelper = new TextHelper();
            ApplyLocale(PlayerPrefs.GetString(PrefName));
            DontDestroyOnLoad(this);
        }

        public void SetLocale([CanBeNull] string localeKey)
        {
            PlayerPrefs.SetString(PrefName, localeKey);
            ApplyLocale(localeKey);
        }

        private void ApplyLocale(string localeKey)
        {
            if (localeKey == null || !_localeItems.Select(item => item.localeKey).Contains(localeKey))
                localeKey = GetDefaultLocaleKey();

            _localeKey = localeKey;
            _textHelper.SetLocale(_localeKey);
        }

        private string GetDefaultLocaleKey()
        {
            foreach (var item in _localeItems)
                if (item.localeLanguages.Contains(Application.systemLanguage))
                    return item.localeKey;

            return _localeItems[0].localeKey;
        }

        public string GetCurrentLocaleKey()
        {
            return _localeKey;
        }

        public string GetNextLocale()
        {
            var currentIndex = Array.FindIndex(_localeItems, item => item.localeKey.Equals(GetCurrentLocaleKey()));
            return _localeItems[(currentIndex + 1) % _localeItems.Length].localeKey;
        }


        public string GetPrevLocale()
        {
            var currentIndex = Array.FindIndex(_localeItems, item => item.localeKey.Equals(GetCurrentLocaleKey()));
            if (currentIndex - 1 >= 0) return _localeItems[(currentIndex - 1) % _localeItems.Length].localeKey;

            return _localeItems[_localeItems.Length - 1].localeKey;
        }

        public string GetPrevLocaleKey()
        {
            return GetPrevLocale();
        }

        public string GetNextLocaleKey()
        {
            return GetNextLocale();
        }

        public string GetText(string key)
        {
            return _textHelper.GetText(key);
        }

        public List<string> GetText(IEnumerable<string> keys)
        {
            return _textHelper.GetText(keys);
        }

        [Serializable]
        public struct LocaleItem
        {
            public string localeKey;
            public SystemLanguage[] localeLanguages;
        }
    }
}