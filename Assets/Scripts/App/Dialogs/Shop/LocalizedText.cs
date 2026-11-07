using System;
using System.Collections.Generic;
using System.Text;
using Library.L10n;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[DisallowMultipleComponent]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _textKey = string.Empty;
    [Inject] private LocaleHelper _localeHelper;
    private Text _text;
    private bool _isTextNotNull;

    private TMP_Text _tmpText;
    private bool _isTmpTextNotNull;

    private TextMeshProUGUI _tmpGuiText;
    private bool _isTmpGuiTextNotNull;

    private bool _initialized;

    private bool _enabled = true;

    private string _delimiter;

    private string _exactText = string.Empty;

    private string _formattedBase;

    private readonly List<Tuple<string, object[]>> _textKeyArgsPairList = new List<Tuple<string, object[]>>();

    private bool _textApplied;

    private void Awake()
    {
        InitializeIfNeed();
    }

    private void InitializeIfNeed()
    {
        if (!_initialized)
        {
            _text = GetComponent<Text>();
            _isTextNotNull = _text != null;

            _tmpText = GetComponent<TMP_Text>();
            _isTmpTextNotNull = _tmpText != null;

            _tmpGuiText = GetComponent<TextMeshProUGUI>();
            _isTmpGuiTextNotNull = _tmpGuiText != null;

            _initialized = true;
        }
    }

    private void Start()
    {
        if (!_textApplied && _enabled)
            SetText(_textKey);
    }

    public void SetExactText(string text)
    {
        _enabled = false;
        _exactText = text;
        UpdateText();
    }

    public void SetText(string textKey)
    {
        _enabled = true;
        _formattedBase = "";
        _delimiter = "";
        _textKeyArgsPairList.Clear();
        _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, null));
        UpdateText();
    }

    public void SetText(string textKey, params object[] args)
    {
        _enabled = true;
        _formattedBase = "";
        _delimiter = "";
        _textKeyArgsPairList.Clear();
        _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, args));
        UpdateText();
    }

    public void SetMultiText(string delimiter, params string[] textKeys)
    {
        _enabled = true;
        _formattedBase = null;
        _delimiter = delimiter;
        _textKeyArgsPairList.Clear();

        foreach (var textKey in textKeys)
            _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, null));
        UpdateText();
    }

    public void SetMultiText(string delimiter, params Tuple<string, object[]>[] textData)
    {
        _enabled = true;
        _formattedBase = null;
        _delimiter = delimiter;
        _textKeyArgsPairList.Clear();
        _textKeyArgsPairList.AddRange(textData);
        UpdateText();
    }

    public void SetFormattedMultiText(string formattedBase, params string[] textKeys)
    {
        _enabled = true;
        _formattedBase = formattedBase;
        _delimiter = "";
        _textKeyArgsPairList.Clear();

        foreach (var textKey in textKeys)
            _textKeyArgsPairList.Add(new Tuple<string, object[]>(textKey, null));
        UpdateText();
    }

    public void SetFormattedMultiText(string formattedBase, Tuple<string, object[]>[] textData)
    {
        _enabled = true;
        _formattedBase = formattedBase;
        _delimiter = "";
        _textKeyArgsPairList.Clear();
        _textKeyArgsPairList.AddRange(textData);
        UpdateText();
    }

    public void ReloadText()
    {
        if (!_textApplied)
        {
            SetText(_textKey);
        }
        else
        {
            UpdateText();
        }
    }

    protected void UpdateText()
    {
        _textApplied = true;

        if (!_enabled)
        {
            SetTextInView(_exactText);
        }
        else if (string.IsNullOrEmpty(_formattedBase))
        {
            var textBuilder = new StringBuilder();

            for (var i = 0; i < _textKeyArgsPairList.Count; i++)
            {
                var textKeyArgsPair = _textKeyArgsPairList[i];

                if (!string.IsNullOrEmpty(textKeyArgsPair.Item1) && textKeyArgsPair.Item2 != null)
                    textBuilder.Append(string.Format(_localeHelper.GetText(textKeyArgsPair.Item1),
                        textKeyArgsPair.Item2));
                else if (!string.IsNullOrEmpty(textKeyArgsPair.Item1))
                    textBuilder.Append(_localeHelper.GetText(textKeyArgsPair.Item1));
                else
                    textBuilder.Append("");

                if (i < _textKeyArgsPairList.Count - 1) textBuilder.Append(_delimiter);
            }
            SetTextInView(textBuilder.ToString());
        }
        else
        {
            var strings = new string[_textKeyArgsPairList.Count];

            for (var i = 0; i < _textKeyArgsPairList.Count; i++)
            {
                var textKeyArgsPair = _textKeyArgsPairList[i];

                if (!string.IsNullOrEmpty(textKeyArgsPair.Item1) && textKeyArgsPair.Item2 != null)
                    strings[i] = string.Format(_localeHelper.GetText(textKeyArgsPair.Item1),
                        textKeyArgsPair.Item2);
                else if (!string.IsNullOrEmpty(textKeyArgsPair.Item1))
                    strings[i] = _localeHelper.GetText(textKeyArgsPair.Item1);
                else
                    strings[i] = "";
            }
            SetTextInView(string.Format(_formattedBase, strings));
        }
    }

    protected virtual void SetTextInView(string text)
    {
        InitializeIfNeed();

        if (_isTextNotNull)
            _text.text = text;
        else if (_isTmpTextNotNull)
            _tmpText.text = text;
        else if (_isTmpGuiTextNotNull)
            _tmpGuiText.text = text;
    }

    public void SetColor(Color color)
    {
        InitializeIfNeed();

        if (_isTextNotNull)
            _text.color = color;
        else if (_isTmpTextNotNull)
            _tmpText.color = color;
        else if (_isTmpGuiTextNotNull)
            _tmpGuiText.color = color;
    }
}