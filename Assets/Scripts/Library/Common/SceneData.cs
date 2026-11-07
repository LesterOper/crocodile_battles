using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class SceneData : MonoBehaviour
    {
        private Bundle _activeBundle;
        private Bundle _nextBundle;

        private void OnEnable()
        {
            _activeBundle = new Bundle();
            _nextBundle = new Bundle();

            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            _activeBundle = _nextBundle;
            _nextBundle = new Bundle();
        }

        public string GetString(string key)
        {
            return _activeBundle.GetString(key);
        }

        public void AddString(string key, string value)
        {
            _nextBundle.AddString(key, value);
        }

        public object GetObject(string key)
        {
            return _activeBundle.GetObject(key);
        }

        public void AddObject(string key, object value)
        {
            _nextBundle.AddObject(key, value);
        }


        public class Bundle
        {
            private readonly Dictionary<string, object> _arbitraryData = new Dictionary<string, object>();
            private readonly Dictionary<string, string> _stringData = new Dictionary<string, string>();

            public string GetString(string key)
            {
                return _stringData.ContainsKey(key) ? _stringData[key] : "";
            }

            public void AddString(string key, string value)
            {
                _stringData[key] = value;
            }

            public object GetObject(string key)
            {
                return _arbitraryData.ContainsKey(key) ? _arbitraryData[key] : null;
            }

            public void AddObject(string key, object value)
            {
                _arbitraryData[key] = value;
            }
        }
    }
}