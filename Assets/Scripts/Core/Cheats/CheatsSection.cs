using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core.Cheats
{
    /// <summary>
    /// Fold that contains cheat controls.
    /// All cheat controls live only inside this objects.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CheatsSection : MonoBehaviour
    {
        private readonly List<ICheatsElement> allElements = new List<ICheatsElement>();
        
        [SerializeField] private Button headerButton;
        [SerializeField] private TextMeshProUGUI headerLabel;
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject togglePrefab;
        [SerializeField] private GameObject labelPrefab;
        [SerializeField] private GameObject sliderPrefab;
        [SerializeField] private GameObject buttonWithInputPrefab;

        private RectTransform headerRectTransform;
        
        private float headerHeight;
        private float contentHeight;

        /// <summary>
        /// Section title, displayed in its header.
        /// </summary>
        internal string Header { get; private set; }
        
        /// <summary>
        /// Is currently expanded.
        /// </summary>
        internal bool IsExpanded { get; private set; }

        /// <summary>
        /// Is currently visible.
        /// </summary>
        internal bool IsActive
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
        
        internal float MaxRowWidth { get; set; }
        
        internal float Height => IsExpanded ? headerHeight + contentHeight : headerHeight;
        
        internal RectTransform RectTransform { get; private set; }
        
        /// <summary>
        /// Raised when layout of section has changed (new control added, expanded/collapsed, child element size changed,
        /// etc.).
        /// </summary>
        internal event Action LayoutBecameDirty;

        /// <summary>
        /// Raised when section was expanded.
        /// </summary>
        internal event Action Expanded;

        /// <summary>
        /// Raised when button with closeOnClick=true was clicked.
        /// </summary>
        internal event Action CloseRequested;

        /// <summary>
        /// Replace for Unity's Awake.
        /// </summary>
        internal void Init()
        {
            RectTransform = GetComponent<RectTransform>();

            headerRectTransform = headerButton.GetComponent<RectTransform>();
            headerHeight = headerRectTransform.sizeDelta.y;
            
            headerButton.onClick.AddListener(OnHeaderClick);
            
            contentHolder.gameObject.SetActive(false);
            
            IsExpanded = false;
        }
        
        /// <summary>
        /// Expand section if <see cref="IsExpanded"/> false.
        /// </summary>
        internal void Expand()
        {
            if (IsExpanded) return;
            
            contentHolder.gameObject.SetActive(true);
            IsExpanded = true;

            Expanded?.Invoke();
        }

        /// <summary>
        /// Collapse section if <see cref="IsExpanded"/> true.
        /// </summary>
        internal void Collapse()
        {
            if (!IsExpanded) return;
            
            contentHolder.gameObject.SetActive(false);
            IsExpanded = false;

            UpdateLayout();
        }

        /// <summary>
        /// Expand if collapsed, collapse if expanded.
        /// </summary>
        internal void Toggle()
        {
            if (IsExpanded)
                Collapse();
            else
                Expand();
        }

        /// <summary>
        /// Set section title.
        /// </summary>
        /// <param name="text">New title</param>
        internal void SetHeader(string text)
        {
            Header = text;
            headerLabel.text = text;
        }

        /// <summary>
        /// Create button.
        /// </summary>
        /// <param name="caption">Button title</param>
        /// <param name="onClick">Click handler</param>
        /// <param name="isEnabled">Boolean callback for dynamic determination of button interactivity</param>
        /// <param name="closeAfterClick">Close cheats window after click</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddButton(string caption, Action onClick, Func<bool> isEnabled = null, bool closeAfterClick = false)
        {
            CreateButton(caption, onClick, isEnabled, closeAfterClick);
            
            return this;
        }

        /// <summary>
        /// Create toggle.
        /// </summary>
        /// <param name="caption">Toggle title</param>
        /// <param name="onChanged">Callback on value change event.</param>
        /// <param name="isEnabled">Boolean callback for dynamic determination of button interactivity</param>
        /// <param name="isOn">Is checked by default</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddToggle(string caption, UnityAction<bool> onChanged, Func<bool> isEnabled = null, bool isOn = false)
        {
            var toggle = Instantiate(togglePrefab).GetComponent<CheatsToggle>();
            toggle.SetCaption(caption);
            toggle.Toggle.onValueChanged.AddListener(onChanged);
            toggle.IsEnabledCallback = isEnabled;
            toggle.Toggle.isOn = isOn;
            
            AddElement(toggle);

            return this;
        }

        /// <summary>
        /// Create label with static text.
        /// </summary>
        /// <param name="caption">Text for label</param>
        /// <param name="boundsVisible">Show label bounds</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddLabel(string caption, bool boundsVisible = false)
        {
            CreateLabel(caption, boundsVisible: boundsVisible);

            return this;
        }

        /// <summary>
        /// Create label with dynamic text.
        /// </summary>
        /// <param name="captionGenerator">String callback for label text generation</param>
        /// <param name="boundsVisible">Show label bounds</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddDynamicLabel(Func<string> captionGenerator, bool boundsVisible = false)
        {
            CreateLabel(captionGenerator(), captionGenerator, boundsVisible);

            return this;
        }

        /// <summary>
        /// Split current layout row. Call has no effect if row is empty.
        /// </summary>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddLineBreaker()
        {
            AddElement(new CheatsLineBreaker());

            return this;
        }

        /// <summary>
        /// Create slider.
        /// </summary>
        /// <param name="caption">Slider caption</param>
        /// <param name="onChange">Value change callback</param>
        /// <param name="min">Minimal slider value</param>
        /// <param name="max">Maximal slider value</param>
        /// <param name="initialValue">Initial slider value</param>
        /// <param name="onlyWhole">Iterate only whole (1, 2, 3, ...) numbers</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddSlider(string caption, Action<float> onChange, float min = 0, float max = 1, float initialValue = 0, bool onlyWhole = false)
        {
            CreateSlider(caption, onChange, min, max, initialValue, onlyWhole);
            
            return this;
        }

        /// <summary>
        /// Create button with integer input field.
        /// </summary>
        /// <param name="caption">Button title</param>
        /// <param name="onClick">Click handler. Input value parsed as int.</param>
        /// <param name="isEnabled">Boolean callback for dynamic determination of button interactivity</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddButtonInt(string caption, Action<int> onClick, Func<bool> isEnabled = null)
        {
            CreateButtonInt(caption, onClick, isEnabled);
            
            return this;
        }

        /// <summary>
        /// Create button with float input field.
        /// </summary>
        /// <param name="caption">Button title</param>
        /// <param name="onClick">Click handler. Input value parsed as float.</param>
        /// <param name="isEnabled">Boolean callback for dynamic determination of button interactivity</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddButtonFloat(string caption, Action<float> onClick, Func<bool> isEnabled = null)
        {
            CreateButtonFloat(caption, onClick, isEnabled);

            return this;
        }
        
        /// <summary>
        /// Create button with string input field.
        /// </summary>
        /// <param name="caption">Button title</param>
        /// <param name="onClick">Click handler. Input value passed as is.</param>
        /// <param name="isEnabled">Boolean callback for dynamic determination of button interactivity</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddButtonString(string caption, Action<string> onClick, Func<bool> isEnabled = null)
        {
            CreateButtonString(caption, onClick, isEnabled);

            return this;
        }

        /// <summary>
        /// Creates following structure:
        ///         Static label with caption
        ///       Dynamic label with current value
        ///        [Buttons for decreasing value]
        ///        [Buttons for increasing value]
        ///
        /// <example>
        ///     E.g., this code
        ///     <code>
        ///         ...
        ///         .AddValueChangeShortcut("Label", 100, null, 1, 10, 100)
        ///         ... 
        ///     </code>
        ///     will create following structure:
        ///             Label
        ///              100
        ///         [-1][-10][-100]
        ///         [+1][+10][+100]
        /// </example>
        /// </summary>
        /// <param name="caption">Value title</param>
        /// <param name="value">Initial value</param>
        /// <param name="valueChanged">Value changed callback</param>
        /// <param name="values">Values for dec/inc</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddValueChangeShortcut(
            string caption,
            int value,
            Action<int> valueChanged,
            params int[] values)
        {
            CreateValueChangeShortcut(
                caption,
                value, 
                (a, b) => a - b,
                (a, b) => a + b,
                valueChanged, 
                values);

            return this;
        }
        
        /// <summary>
        /// Creates following structure:
        ///         Static label with caption
        ///       Dynamic label with current value
        ///        [Buttons for decreasing value]
        ///        [Buttons for increasing value]
        ///
        /// <example>
        ///     E.g., this code
        ///     <code>
        ///         ...
        ///         .AddValueChangeShortcut("Label", 100, null, 1, 10, 100)
        ///         ... 
        ///     </code>
        ///     will create following structure:
        ///             Label
        ///              100
        ///         [-1][-10][-100]
        ///         [+1][+10][+100]
        /// </example>
        /// </summary>
        /// <param name="caption">Value title</param>
        /// <param name="value">Initial value</param>
        /// <param name="valueChanged">Value changed callback</param>
        /// <param name="values">Values for dec/inc</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddValueChangeShortcut(
            string caption,
            long value,
            Action<long> valueChanged,
            params long[] values)
        {
            CreateValueChangeShortcut(
                caption,
                value,
                (a, b) => a - b,
                (a, b) => a + b,
                valueChanged, 
                values);

            return this;
        }
        
        /// <summary>
        /// Creates following structure:
        ///         Static label with caption
        ///       Dynamic label with current value
        ///        [Buttons for decreasing value]
        ///        [Buttons for increasing value]
        ///
        /// <example>
        ///     E.g., this code
        ///     <code>
        ///         ...
        ///         .AddValueChangeShortcut("Label", 100, null, 1, 10, 100)
        ///         ... 
        ///     </code>
        ///     will create following structure:
        ///             Label
        ///              100
        ///         [-1][-10][-100]
        ///         [+1][+10][+100]
        /// </example>
        /// </summary>
        /// <param name="caption">Value title</param>
        /// <param name="value">Initial value</param>
        /// <param name="valueChanged">Value changed callback</param>
        /// <param name="values">Values for dec/inc</param>
        /// <returns>Section method was invoked on</returns>
        public CheatsSection AddValueChangeShortcut(
            string caption,
            float value,
            Action<float> valueChanged,
            params float[] values)
        {
            CreateValueChangeShortcut(
                caption,
                value,
                (a, b) => a - b,
                (a, b) => a + b,
                valueChanged, 
                values);

            return this;
        }

        internal void Rebuild()
        {
            BuildRows(SplitElementsByRows());
        }
        
        private void CreateButton(string caption, Action onClick, Func<bool> isEnabled = null, bool closeAfterClick = false)
        {
            var button = Instantiate(buttonPrefab).GetComponent<CheatsButton>();
            button.SetCaption(caption);
            button.Button.onClick.AddListener(() => onClick?.Invoke());
            button.IsEnabledCallback = isEnabled;

            if (closeAfterClick)
            {
                button.Button.onClick.AddListener(() => CloseRequested?.Invoke());
            }

            AddElement(button);
        }
        
        private void CreateLabel(string caption, Func<string> update = null, bool boundsVisible = false)
        {
            var label = Instantiate(labelPrefab).GetComponent<CheatsLabel>();
            label.SetCaption(caption);
            label.SetBoundsVisible(boundsVisible);
            label.UpdateCallback = update;
            label.CaptionUpdated += () => LayoutBecameDirty?.Invoke();

            AddElement(label);
        }

        private void CreateSlider(string caption, Action<float> onChange, float min, float max, float initialValue, bool onlyWhole)
        {
            var slider = Instantiate(sliderPrefab).GetComponent<CheatsSlider>();
            slider.SetCaption(caption);
            slider.Configure(min, max, initialValue, onlyWhole);
            slider.ValueChanged += onChange;
            
            AddElement(slider);
        }

        private void CreateButtonInt(string caption, Action<int> onClick, Func<bool> isEnabled = null)
        {
            var button = CreateButtonWithInput(caption, CheatsButtonWithInput.InputType.Integer, isEnabled);
            button.ButtonClickedInteger += onClick;
            
            AddElement(button);
        }
        
        private void CreateButtonFloat(string caption, Action<float> onClick, Func<bool> isEnabled = null)
        {
            var button = CreateButtonWithInput(caption, CheatsButtonWithInput.InputType.Float, isEnabled);
            button.ButtonClickedFloat += onClick;
            
            AddElement(button);
        }

        private void CreateButtonString(string caption, Action<string> onClick, Func<bool> isEnabled = null)
        {
            var button = CreateButtonWithInput(caption, CheatsButtonWithInput.InputType.String, isEnabled);
            button.ButtonClickedString += onClick;
            
            AddElement(button);
        }

        private CheatsButtonWithInput CreateButtonWithInput(
            string caption,
            CheatsButtonWithInput.InputType type,
            Func<bool> isEnabled = null)
        {
            var button = Instantiate(buttonWithInputPrefab).GetComponent<CheatsButtonWithInput>();
            button.IsEnabledCallback = isEnabled;
            button.SetCaption(caption);
            button.SetType(type);

            return button;
        }

        private void CreateValueChangeShortcut<TType>(
            string caption,
            TType value,
            Func<TType, TType, TType> dec,
            Func<TType, TType, TType> inc,
            Action<TType> valueChanged,
            params TType[] values)
            where TType : struct
        {
            var locVal = value;
            
            AddLineBreaker();
            
            AddLabel(caption);
            
            AddLineBreaker();
            
            AddDynamicLabel(() => locVal.ToString());
            
            AddLineBreaker();

            for (int i = 0; i < values.Length; ++i)
            {
                var val = values[i];
                AddButton($"-{val}", () =>
                {
                    locVal = dec.Invoke(locVal, val);
                    
                    valueChanged?.Invoke(locVal);
                });
            }

            AddLineBreaker();
            
            for (int i = 0; i < values.Length; ++i)
            {
                var val = values[i];
                AddButton($"+{val}", () =>
                {
                    locVal = inc.Invoke(locVal, val);
                    
                    valueChanged?.Invoke(locVal);
                });
            }

            AddLineBreaker();
        }

        private void AddElement(ICheatsElement element)
        {
            allElements.Add(element);

            if (element.RectTransform != null)
            {
                element.RectTransform.SetParent(contentHolder.transform);
                element.RectTransform.localScale = Vector3.one;
            }
            
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            LayoutBecameDirty?.Invoke();
        }

        private void BuildRows(List<List<ICheatsElement>> rows)
        {
            var widths = new List<float>();
            var heights = new List<float>();
            
            contentHeight = 0;
            
            for (int i = 0; i < rows.Count; ++i)
            {
                var row = rows[i];
                float rowWidth = 0;
                float rowHeight = 0;

                for (int j = 0; j < row.Count; ++j)
                {
                    rowWidth += row[j].GetWidth();
                    rowHeight = Mathf.Max(rowHeight, row[j].RectTransform.sizeDelta.y);
                }
                
                widths.Add(Mathf.Min(MaxRowWidth, rowWidth));
                heights.Add(rowHeight);

                contentHeight += rowHeight;
            }

            float rowY = 0;
            for (int i = 0; i < rows.Count; ++i)
            {
                var row = rows[i];

                if (row.Count > 1)
                {
                    float currentX = 0;
                    var k = MaxRowWidth / widths[i];
                    for (int j = 0; j < row.Count; ++j)
                    {
                        var e = row[j];
                        var tr = e.RectTransform;
                        tr.anchoredPosition = new Vector2(currentX - MaxRowWidth / 2, -rowY + contentHeight / 2);

                        e.SetWidth(e.GetWidth() * k);
                        
                        currentX += e.GetWidth() * k;
                    }
                }
                else
                {
                    var e = row[0];
                    e.SetWidth(MaxRowWidth);

                    var tr = e.RectTransform;
                    tr.anchoredPosition = new Vector2(-MaxRowWidth / 2, -rowY + contentHeight / 2);
                }
                
                rowY += heights[i];
            }

            headerRectTransform.sizeDelta = new Vector2(MaxRowWidth, headerRectTransform.sizeDelta.y);
            contentHolder.sizeDelta = new Vector2(MaxRowWidth, contentHeight);
            contentHolder.anchoredPosition = new Vector2(contentHolder.anchoredPosition.x, -100);
        }

        private List<List<ICheatsElement>> SplitElementsByRows()
        {
            var rows = new List<List<ICheatsElement>>();
            var currentRow = new List<ICheatsElement>();
            float currentRowWidth = 0;
            for (int i = 0; i < allElements.Count; ++i)
            {
                var element = allElements[i];

                var elementSize = Mathf.Min(MaxRowWidth, element.GetWidth());

                bool elementIsLineBreaker = element is CheatsLineBreaker;
                bool rowIsFull = currentRowWidth + elementSize > MaxRowWidth;

                if (elementIsLineBreaker && currentRow.Count > 0 || rowIsFull)
                {
                    rows.Add(currentRow);

                    currentRow = new List<ICheatsElement>();
                    currentRowWidth = 0;
                }
                
                if (elementIsLineBreaker) continue;
                
                currentRow.Add(element);

                currentRowWidth += elementSize;
            }

            if (currentRow.Count != 0)
            {
                rows.Add(currentRow);
            }

            return rows;
        }

        private void OnHeaderClick()
        {
            Toggle();
        }
    }
}