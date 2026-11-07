using System;
using System.Collections.Generic;
using System.Linq;
using Library.Collections;
using Library.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    public class RecycleListView : MonoBehaviour
    {
        public enum Alignment
        {
            Left,
            Right,
            Middle
        }


        public enum Orientation
        {
            Horizontal,
            Vertical
        }


        public enum PositionSourceType
        {
            LINEAR,
            GRID
        }

        private readonly Dictionary<int, GameObjectPool> _freeItemsByType = new Dictionary<int, GameObjectPool>();
        private readonly Dictionary<int, ViewData> _usedItemsByPosition = new Dictionary<int, ViewData>();

        [SerializeField] private Alignment _alignment = Alignment.Middle;
        [SerializeField] private GameObject[] _itemSamples;
        [SerializeField] private Vector2 _itemsPadding;
        [SerializeField] private PositionSourceType _layoutType = PositionSourceType.LINEAR;
        [SerializeField] private int _lineSize = 2;
        [SerializeField] private Orientation _orientation = Orientation.Vertical;

        private PositionSource _positionSource;
        [SerializeField] private float _scrollPadding;
        [SerializeField] private ScrollRect _scrollRect;

        public IListAdapter Adapter { get; private set; }

        public RectTransform Parent { get; private set; }

        public ScrollRect ScrollRect => _scrollRect;

        public Orientation LayoutOrientation => _orientation;

        public Alignment LayoutAlignment => _alignment;

        public float ScrollPadding => _scrollPadding;

        public Vector2 ItemsPadding
        {
            get => _itemsPadding;
            set => _itemsPadding = value;
        }

        public PositionSourceType LayoutType => _layoutType;

        public int LineSize => _lineSize;

        private void Awake()
        {
            Parent = GetComponent<RectTransform>();
            _scrollRect.onValueChanged.AddListener(OnScroll);

            switch (_layoutType)
            {
                case PositionSourceType.LINEAR:
                    _positionSource = new LinearPositionSource(this);
                    break;
                case PositionSourceType.GRID:
                    _positionSource = new GridPositionSource(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Start()
        {
            foreach (var itemSample in _itemSamples)
                itemSample.SetActive(false);

            if (Adapter != null)
            {
                UpdateCreateItems();
                SetupParentSize();
                UpdateVisibleItems();
            }
        }

        public void SetAdapter(IListAdapter adapter)
        {
            Adapter = adapter;

            OnItemsChanged();
        }

        public void OnItemsChanged()
        {
            UpdateCreateItems();
            SetupParentSize();
            UpdateVisibleItems();
        }

        public void OnItemChanged(int position)
        {
            SetupParentSize();

            if (_usedItemsByPosition.Keys.Contains(position))
                FreeItemView(position);
            if (IsItemVisible(position))
                SetupView(position);

            for (var i = position; i < Adapter.GetItemsCount(); i++)
                if (_usedItemsByPosition.ContainsKey(i))
                    SetupItemPosition(i);
        }

        public void OnItemRemoved(int position)
        {
            if (_usedItemsByPosition.Keys.Contains(position))
                FreeItemView(position);
            SetupParentSize();

            for (var i = position; i < Adapter.GetItemsCount() + 1; i++)
                if (_usedItemsByPosition.ContainsKey(i))
                {
                    _usedItemsByPosition[i - 1] = _usedItemsByPosition[i];
                    _usedItemsByPosition.Remove(i);
                    SetupItemPosition(i - 1);
                }

            UpdateVisibleItems();
        }

        public void OnItemAdded()
        {
            OnItemAdded(Adapter.GetItemsCount() - 1);
        }

        public void OnItemAdded(int position)
        {
            SetupParentSize();

            for (var i = Adapter.GetItemsCount() - 2; i >= position; i--)
                if (_usedItemsByPosition.ContainsKey(i))
                {
                    _usedItemsByPosition[i + 1] = _usedItemsByPosition[i];
                    _usedItemsByPosition.Remove(i);
                    SetupItemPosition(i + 1);
                }

            UpdateVisibleItems();
        }

        private void UpdateCreateItems()
        {
            var currentVisibleItems = new HashSet<int>(_usedItemsByPosition.Keys);
            foreach (var position in currentVisibleItems)
                FreeItemView(position);

            _usedItemsByPosition.Clear();

            for (var i = 0; i < Adapter.GetItemsCount(); i++)
                if (IsItemVisible(i))
                {
                    SetupView(i);
                    SetupItemPosition(i);
                }
        }

        private void FreeItemView(int position)
        {
            var usedItem = _usedItemsByPosition[position];
            _usedItemsByPosition.Remove(position);
            _freeItemsByType[usedItem.ViewType].Free(usedItem.ItemView);

            usedItem.ItemView.SetActive(false);
        }

        private void SetupView(int position)
        {
            var itemType = Adapter.GetViewTypeNumber(position);

            if (!_freeItemsByType.ContainsKey(itemType))
                _freeItemsByType[itemType] = new GameObjectPool(_itemSamples[itemType], Parent);

            var itemView = _freeItemsByType[itemType].Obtain();
            _usedItemsByPosition[position] = new ViewData
            {
                ViewType = itemType,
                ItemView = itemView
            };

            itemView.SetActive(true);

            Adapter.SetupView(position, itemType, itemView);
        }

        private void OnScroll(Vector2 scrollPosition)
        {
            UpdateVisibleItems();
        }

        private void UpdateVisibleItems()
        {
            var currentVisibleItems = new HashSet<int>(_usedItemsByPosition.Keys);

            foreach (var position in currentVisibleItems)
                if (!IsItemVisible(position))
                    FreeItemView(position);

            for (var i = 0; i < Adapter.GetItemsCount(); i++)
                if (!currentVisibleItems.Contains(i) && IsItemVisible(i))
                {
                    SetupView(i);
                    SetupItemPosition(i);
                }
        }

        private Vector2 GetItemSize(int position)
        {
            return _itemSamples[Adapter.GetViewTypeNumber(position)].GetComponent<RectTransform>().sizeDelta;
        }

        private Vector2 GetItemPosition(int position)
        {
            return _positionSource.GetItemPosition(position, Parent);
        }

        private bool IsItemVisible(int position)
        {
            try
            {
                
                if (Parent.anchoredPosition == null) return false;
                var parentLocalPosition = Parent.anchoredPosition;

                switch (_orientation)
                {
                    case Orientation.Horizontal:
                        var sizeDeltaX = _scrollRect.GetComponent<RectTransform>().rect.width;
                        return GetItemPosition(position).x + GetItemSize(position).x / 2f + parentLocalPosition.x
                               > -sizeDeltaX * 0.55f &&
                               GetItemPosition(position).x - GetItemSize(position).x / 2f + parentLocalPosition.x
                               < sizeDeltaX * 0.55f;

                    case Orientation.Vertical:
                        var sizeDeltaY = _scrollRect.GetComponent<RectTransform>().rect.height;
                        return GetItemPosition(position).y + GetItemSize(position).y / 2f + parentLocalPosition.y
                               > -sizeDeltaY * 1.05f &&
                               GetItemPosition(position).y - GetItemSize(position).y / 2f + parentLocalPosition.y
                               < sizeDeltaY * 0.05f;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch 
            {
     
                throw;
            }
        }

        private void SetupItemPosition(int position)
        {
            var itemView = _usedItemsByPosition[position].ItemView;
            itemView.GetComponent<RectTransform>().anchoredPosition = GetItemPosition(position);
        }

        private void SetupParentSize()
        {
            Parent.sizeDelta = _positionSource.GetParentSize(Parent);
        }


        public interface IListAdapter
        {
            int GetItemsCount();
            int GetViewTypeNumber(int position);
            void SetupView(int position, int type, GameObject itemObject);
        }


        private class ViewData
        {
            public int ViewType { get; set; }
            public GameObject ItemView { get; set; }
        }


        private abstract class PositionSource
        {
            public PositionSource(RecycleListView listView)
            {
                ListView = listView;
            }

            protected RecycleListView ListView { get; }

            public abstract Vector2 GetItemPosition(int position, RectTransform parent);
            public abstract Vector2 GetParentSize(RectTransform parent);
        }

        private class LinearPositionSource : PositionSource
        {
            public LinearPositionSource(RecycleListView listView) : base(listView)
            {
            }

            public override Vector2 GetItemPosition(int position, RectTransform parent)
            {
                var result = ListView.GetItemSize(position) / 2;

                switch (ListView.LayoutOrientation)
                {
                    case Orientation.Horizontal:
                        result.x += ListView.ScrollPadding;
                        result.x += ListView.ItemsPadding.x * position;
                        for (var i = 0; i < position; i++)
                            result.x += ListView.GetItemSize(i).x;
                        result.y = 0;

                        break;

                    case Orientation.Vertical:
                        result.x = 0;
                        result.y *= -1;
                        result.y -= ListView.ScrollPadding;
                        result.y -= ListView.ItemsPadding.y * position;
                        for (var i = 0; i < position; i++)
                            result.y -= ListView.GetItemSize(i).y;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return result;
            }

            public override Vector2 GetParentSize(RectTransform parent)
            {
                var width = 0f;
                var height = 0f;

                switch (ListView.LayoutOrientation)
                {
                    case Orientation.Horizontal:
                        for (var i = 0; i < ListView.Adapter.GetItemsCount(); i++)
                            width += ListView.GetItemSize(i).x;
                        width += ListView.ScrollPadding * 2;
                        width += ListView.ItemsPadding.x * (ListView.Adapter.GetItemsCount() - 1);
                        height = parent.sizeDelta.y;
                        break;

                    case Orientation.Vertical:
                        width = parent.sizeDelta.x;
                        for (var i = 0; i < ListView.Adapter.GetItemsCount(); i++)
                            height += ListView.GetItemSize(i).y;
                        height += ListView.ScrollPadding * 2;
                        height += ListView.ItemsPadding.y * (ListView.Adapter.GetItemsCount() - 1);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new Vector2(width, height);
            }
        }

        private class GridPositionSource : PositionSource
        {
            public GridPositionSource(RecycleListView listView) : base(listView)
            {
            }

            public override Vector2 GetItemPosition(int position, RectTransform parent)
            {
                var result = new Vector2();

                var linesBefore = (int) MathA.Floor(position / (float) ListView.LineSize);

                switch (ListView.LayoutOrientation)
                {
                    case Orientation.Horizontal:
                        result.x += ListView.ScrollPadding;
                        for (var i = 0; i < linesBefore; i++)
                            result.x += GetLineSize(i).x;

                        result.x += GetLineSize(position / ListView.LineSize).x / 2;
                        result.x += ListView.ItemsPadding.x * linesBefore;

                        for (var i = linesBefore * ListView.LineSize; i < position; i++)
                        {
                            result.y -= ListView.GetItemSize(i).y;
                            result.y -= ListView.ItemsPadding.y;
                        }

                        result.y -= ListView.GetItemSize(position).y / 2f;
                        result.y += GetLineSize(position / ListView.LineSize).y / 2f;

                        break;

                    case Orientation.Vertical:
                        result.y -= ListView.ScrollPadding;
                        for (var i = 0; i < linesBefore; i++)
                            result.y -= GetLineSize(i).y;

                        result.y -= GetLineSize(position / ListView.LineSize).y / 2;
                        result.y -= ListView.ItemsPadding.y * linesBefore;

                        for (var i = linesBefore * ListView.LineSize; i < position; i++)
                        {
                            result.x += ListView.GetItemSize(i).x;
                            result.x += ListView.ItemsPadding.x;
                        }

                        result.x += ListView.GetItemSize(position).x / 2f;
                        result.x -= GetLineSize(position / ListView.LineSize).x / 2f;

                        if (position > ListView.Adapter.GetItemsCount() - ListView.LineSize)
                            if (ListView.LayoutAlignment == Alignment.Left ||
                                ListView.LayoutAlignment == Alignment.Right)
                                result.x = result.x + GetLineSize(position / ListView.LineSize).x / 2f *
                                    (ListView.LayoutAlignment == Alignment.Left ? -1 : 1);

                        // print($"[RecycleListView] GetItemPosition : {position} - {result} - {linesBefore} - {MathA.Ceil(position / (float) ListView.LineSize)}");

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return result;
            }

            public override Vector2 GetParentSize(RectTransform parent)
            {
                var width = 0f;
                var height = 0f;

                var linesCount = MathA.Ceil(ListView.Adapter.GetItemsCount() / (float) ListView.LineSize);

                switch (ListView.LayoutOrientation)
                {
                    case Orientation.Horizontal:
                        for (var i = 0; i < linesCount; i++)
                            width += GetLineSize(i).x;
                        width += ListView.ScrollPadding * 2;
                        width += ListView.ItemsPadding.x *
                                 (MathA.Ceil(ListView.Adapter.GetItemsCount() / (float) ListView.LineSize) - 1);
                        height = parent.sizeDelta.y;
                        break;

                    case Orientation.Vertical:
                        width = parent.sizeDelta.x;
                        for (var i = 0; i < linesCount; i++)
                            height += GetLineSize(i).y;
                        height += ListView.ItemsPadding.y *
                                  (MathA.Ceil(ListView.Adapter.GetItemsCount() / (float) ListView.LineSize) - 1);
                        height += ListView.ScrollPadding * 2;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new Vector2(width, height);
            }

            private Vector2 GetLineSize(int line)
            {
                var vector2 = new Vector2();
                for (var i = line * ListView.LineSize;
                    i < Math.Min(ListView.Adapter.GetItemsCount(), (line + 1) * ListView.LineSize);
                    i++)
                    switch (ListView.LayoutOrientation)
                    {
                        case Orientation.Horizontal:
                            var itemSizeH = ListView.GetItemSize(i);

                            if (vector2.x < itemSizeH.x)
                                vector2.x = itemSizeH.x;

                            vector2.y += itemSizeH.y;

                            break;

                        case Orientation.Vertical:
                            var itemSizeV = ListView.GetItemSize(i);

                            vector2.x += itemSizeV.x;

                            if (vector2.y < itemSizeV.y)
                                vector2.y = itemSizeV.y;

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                switch (ListView.LayoutOrientation)
                {
                    case Orientation.Horizontal:
                        vector2.y += ListView.ItemsPadding.y * (ListView.LineSize - 1);
                        break;

                    case Orientation.Vertical:
                        vector2.x += ListView.ItemsPadding.x * (ListView.LineSize - 1);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return vector2;
            }
        }
    }
}