using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Library.Text
{
    [DisallowMultipleComponent]
    public sealed class TextAdapter : MonoBehaviour
    {
        private readonly List<Tuple<string, object[]>> _textKeyArgsPairList = new List<Tuple<string, object[]>>();

        private string _delimiter;
        private string _formattedBase;
        private bool _initialized;
        private bool _l10nEnabled = true;

        private bool _textApplied;
        private TMP_Text _tmpText;
        private TextMeshProUGUI _tmpUiText;
        private Type _type;
        private UnityEngine.UI.Text _unityUiText;

        private void Awake()
        {
            InitializeIfNeed();
        }

        private void InitializeIfNeed()
        {
            if (!_initialized)
            {
                _unityUiText = GetComponent<UnityEngine.UI.Text>();
                if (_unityUiText != null)
                {
                    _type = Type.UNITY_UI_TEXT;
                }
                else
                {
                    _tmpText = GetComponent<TMP_Text>();
                    if (_tmpText != null)
                    {
                        _type = Type.TMP_TEXT;
                    }
                    else
                    {
                        _tmpUiText = GetComponent<TextMeshProUGUI>();
                        if (_tmpUiText != null)
                            _type = Type.TMP_UI_TEXT;
                        else
                            _type = Type.NONE;
                    }
                }

                _initialized = true;
            }
        }

        public void SetExactText(string text)
        {
            _l10nEnabled = false;
            _textKeyArgsPairList.Clear();
            _textKeyArgsPairList.Add(new Tuple<string, object[]>(text, null));
            UpdateText();
        }
        public void SetText(string textKey)
        {
            _l10nEnabled = true;
            _formattedBase = "";
            _delimiter = "";
            _textKeyArgsPairList.Clear();
            _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, null));
            UpdateText();
        }

        public void SetText(string textKey, params object[] args)
        {
            _l10nEnabled = true;
            _formattedBase = "";
            _delimiter = "";
            _textKeyArgsPairList.Clear();
            _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, args));
            UpdateText();
        }

        private void UpdateText()
        {
            _textApplied = true;
            var textBuilder = new StringBuilder();
            for (var i = 0; i < _textKeyArgsPairList.Count; i++)
            {
                var textKeyArgsPair = _textKeyArgsPairList[i];

                if (!string.IsNullOrEmpty(textKeyArgsPair.Item1) && textKeyArgsPair.Item2 != null)
                    textBuilder.Append(string.Format(textKeyArgsPair.Item1, textKeyArgsPair.Item2));
                else if (!string.IsNullOrEmpty(textKeyArgsPair.Item1))
                    textBuilder.Append(textKeyArgsPair.Item1);
                else 
                    textBuilder.Append("");

                if (i < _textKeyArgsPairList.Count - 1) textBuilder.Append(_delimiter);
            }
            SetTextInView(textBuilder.ToString());
        }

        private void SetTextInView(string text)
        {
            InitializeIfNeed();

            switch (_type)
            {
                case Type.UNITY_UI_TEXT:
                    _unityUiText.text = text;
                    break;
                case Type.TMP_TEXT:
                    _tmpText.text = text;
                    break;
                case Type.TMP_UI_TEXT:
                    _tmpUiText.text = text;
                    break;
            }
        }

        private enum Type
        {
            NONE,

            UNITY_UI_TEXT,
            TMP_TEXT,
            TMP_UI_TEXT
        }
    }
}