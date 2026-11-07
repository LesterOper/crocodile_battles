using System;
using System.Collections.Generic;
using UnityEngine;

namespace Library.View
{
    public class TypedListView : MonoBehaviour
    {
        private readonly Dictionary<int, List<GameObject>> _freeItemsDict = new Dictionary<int, List<GameObject>>();

        private readonly List<Tuple<int, GameObject>> _usedItems = new List<Tuple<int, GameObject>>();
        private IListAdapter _adapter;
        [SerializeField] private GameObject[] _itemSamples;

        protected void Start()
        {
            foreach (var itemSample in _itemSamples)
                itemSample.SetActive(false);

            UpdateCreateItems();
        }

        public void SetAdapter(IListAdapter adapter)
        {
            _adapter = adapter;
            UpdateCreateItems();
        }

        public void OnItemsChanged()
        {
            UpdateCreateItems();
        }

        public void OnItemAdded()
        {
            SetupItem(_adapter.GetItemsCount() - 1);
        }

        public void OnItemChanged(int position)
        {
            FreeItemView(position);
            SetupItem(position);
        }

        public void OnItemRemoved(int position)
        {
            FreeItemView(position);
        }

        private void UpdateCreateItems()
        {
            if (_adapter == null)
                return;

            for (var i = _usedItems.Count - 1; i >= 0; i--)
                FreeItemView(i);

            _usedItems.Clear();

            for (var i = 0; i < _adapter.GetItemsCount(); i++)
                SetupItem(i);
        }

        private void FreeItemView(int position)
        {
            var usedItem = _usedItems[position];
            _usedItems.RemoveAt(position);
            _freeItemsDict[usedItem.Item1].Add(usedItem.Item2);

            usedItem.Item2.SetActive(false);
        }

        private void SetupItem(int position)
        {
            var itemType = _adapter.GetViewTypeNumber(position);

            if (!_freeItemsDict.ContainsKey(itemType))
                _freeItemsDict[itemType] = new List<GameObject>();

            if (_freeItemsDict[itemType].Count == 0)
            {
                var sample = _itemSamples[itemType];
                var parentTransform = GetComponent<RectTransform>();
                _freeItemsDict[itemType].Add(Instantiate(sample, parentTransform));
            }

            var item = _freeItemsDict[itemType][0];
            _freeItemsDict[itemType].RemoveAt(0);
            _usedItems.Add(new Tuple<int, GameObject>(itemType, item));

            item.GetComponent<RectTransform>().SetSiblingIndex(position);
            item.SetActive(true);

            _adapter.SetupView(position, itemType, item);
        }

        public interface IListAdapter
        {
            int GetItemsCount();
            int GetViewTypeNumber(int position);
            void SetupView(int position, int type, GameObject itemObject);
        }
    }
}