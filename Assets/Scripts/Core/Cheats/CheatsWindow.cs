using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cheats
{
    internal class CheatsWindow : MonoBehaviour
    {
        private class Tab
        {
            public readonly string Name;
            public readonly List<CheatsSection> Sections = new List<CheatsSection>();

            public CheatsTabButton Button;

            public Tab(string name)
            {
                Name = name;
            }

            public CheatsSection FindSection(string name)
            {
                for (int i = 0; i < Sections.Count; ++i)
                {
                    if (Sections[i].Header == name) return Sections[i];
                }

                return null;
            }
        }
        
        private const string DefaultTabName = "Default";
        
        private readonly List<Tab> tabs = new List<Tab>();

        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private Button closeBtn;
        [SerializeField] private ScrollRect tabsScrollRect;
        [SerializeField] private ScrollRect contentScrollRect;
        [SerializeField] private GameObject sectionPrefab;
        [SerializeField] private GameObject tabButtonPrefab;

        private bool toggleCheats;

        private int currentTab;

        public bool IsShown { get; private set; } = true;

        public float MaxRowWidth => GetContentWidth();

        public void Show()
        {
            if (IsShown || !Cheats.IsEnabled) return;

            IsShown = true;
            
            root.SetActive(true);

            StartCoroutine(RebuildLayoutTask());
        }

        public void Hide()
        {
            if (!IsShown) return;

            root.SetActive(false);
            IsShown = false;
        }

        public void Toggle()
        {
            if (IsShown)
                Hide();
            else
                Show();
        }

        public CheatsSection GetSection(string header, string tabName = null)
        {
            tabName ??= DefaultTabName;
            
            var tab = EnsureTab(tabName);
            var tabIdx = tabs.IndexOf(tab);

            var section = tab.FindSection(header);

            if (section != null)
            {
                return section;
            }

            var obj = Instantiate(sectionPrefab, contentHolder);
            
            section = obj.GetComponent<CheatsSection>();
            
            section.Init();
            
            tab.Sections.Add(section);
            
            section.SetHeader(header);

            section.LayoutBecameDirty += () => {
                if (currentTab == tabIdx)
                {
                    RebuildLayout();
                }
            };

            section.Expanded += () => OnSectionExpanded(section);

            section.CloseRequested += Hide;
            
            section.IsActive = currentTab == tabIdx;

            return section;
        }

        private void OnSectionExpanded(CheatsSection section)
        {
            RebuildLayout();
            FocusOnIfRequired(section);
        }

        private void FocusOnIfRequired(CheatsSection section)
        {
            var top = section.RectTransform.localPosition.y;
            var bottom = top - section.Height;
            
            var toViewportSpace = contentHolder.TransformTo(contentScrollRect.viewport);

            var viewportTop = toViewportSpace.MultiplyPoint(new Vector3(0, top, 0)).y;
            var viewportBottom = toViewportSpace.MultiplyPoint(new Vector3(0, bottom, 0)).y;

            var height = contentScrollRect.viewport.GetPixelHeight();
            if (viewportBottom < -height)
            {
                var shift = height < section.Height
                    ? Mathf.Abs(viewportTop)
                    : Mathf.Abs(height + viewportBottom);
                contentHolder.Shift(new Vector2(0, shift));
            }
        }

        private Tab EnsureTab(string name)
        {
            for (int i = 0; i < tabs.Count; ++i)
            {
                if (tabs[i].Name == name) return tabs[i];
            }

            var tab = new Tab(name);
            tabs.Add(tab);
            
            tab.Button = CreateTabButton(tab);
            
            RecalcTabsLayout();

            return tab;
        }

        private CheatsTabButton CreateTabButton(Tab tab)
        {
            var tabGo = Instantiate(tabButtonPrefab, tabsScrollRect.content);
            tabGo.transform.localScale = Vector3.one;
            tabGo.transform.localPosition = Vector3.zero;
            
            var tabButton = tabGo.GetComponent<CheatsTabButton>();
            var idx = tabs.IndexOf(tab);
            
            tabButton.Button.onClick.AddListener(() => SwitchToTab(idx));
            tabButton.CaptionText.text = tab.Name;

            return tabButton;
        }

        private void RecalcTabsLayout()
        {
            var totalWidth = 0f;
            for (int i = 0; i < tabs.Count; ++i)
            {
                totalWidth += Cheats.CalcTextWidth(tabs[i].Name);
            }

            var k = MaxRowWidth / totalWidth;
            for (int i = 0; i < tabs.Count; ++i)
            {
                var width = Cheats.CalcTextWidth(tabs[i].Name);
                var adjustedWidth = width * k;
                tabs[i].Button.GetComponent<RectTransform>().SetWidth(Mathf.Max(adjustedWidth, width));
            }
            
            tabsScrollRect.content.SetWidth(totalWidth);
        }

        private void SwitchToTab(int idx)
        {
            for (var i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                var isVisible = idx == i;
                for (var j = 0; j < tab.Sections.Count; j++)
                {
                    var s = tab.Sections[j];
                    s.IsActive = isVisible;
                }

                if (isVisible && tab.Sections.Count == 1)
                {
                    tab.Sections[0].Expand();
                }
            }

            currentTab = idx;

            RebuildLayout();
        }

        private IEnumerator RebuildLayoutTask()
        {
            yield return null;
            
            RebuildLayout();
        }

        private void RebuildLayout()
        {
            if (!IsShown) return;
            
            float accumulatedY = 0;

            for (int j = 0; j < tabs.Count; ++j)
            {
                var tab = tabs[j];
                for (int i = 0; i < tab.Sections.Count; ++i)
                {
                    var section = tab.Sections[i];
                
                    section.MaxRowWidth = GetContentWidth();
                
                    section.RectTransform.anchoredPosition = new Vector2(0, -accumulatedY);

                    section.Rebuild();

                    if (section.IsActive)
                    {
                        accumulatedY += section.Height;
                    }
                }
            }

            contentHolder.SetHeight(accumulatedY);
        }

        private float GetContentWidth() => contentHolder.GetPixelWidth();
        
        private void OnCloseBtnClick()
        {
            Hide();
        }

        private void Start()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        private void Update()
        {
            int touchCount = Input.touchCount;
            if ((touchCount == 4 || Input.GetKeyDown(KeyCode.BackQuote)) && toggleCheats)
            {
                Toggle();
                toggleCheats = false;
            }
            else
            {
                if (touchCount != 4)
                    toggleCheats = true;
            }
        }

        private void OnDestroy()
        {
            Cheats.window = null;
        }
    }
}