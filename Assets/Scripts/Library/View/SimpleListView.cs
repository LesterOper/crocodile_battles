using System.Collections.Generic;
using Library.Collections;
using UnityEngine;

namespace Library.View
{
    public class SimpleListView : MonoBehaviour
    {
        private IListAdapter _adapter;

        private Pool<GameObject> _freeItems;
        private List<GameObject> _items;
        [SerializeField] private GameObject _itemSample;

        private void Awake()
        {
            _freeItems = new GameObjectPool(_itemSample, GetComponent<RectTransform>());
            _items = new List<GameObject>();
        }

        protected void Start()
        {
            _itemSample.SetActive(false);

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
            var position = _adapter.GetItemsCount() - 1;

            var item = _freeItems.Obtain();
            _items.Add(item);
            item.SetActive(true);

            _adapter.SetupView(position, _items[position]);
        }

        public void OnItemChanged(int position)
        {
            _adapter.SetupView(position, _items[position]);
        }

        public void OnItemRemoved(int position)
        {
            var poolObject = _items[position];
            poolObject.SetActive(false);
            _freeItems.Free(poolObject);
            _items.RemoveAt(position);
        }

        private void UpdateCreateItems()
        {
            if (_adapter == null)
                return;

            for (var i = 0; i < _adapter.GetItemsCount(); i++)
            {
                GameObject item;

                if (i >= _items.Count)
                {
                    item = _freeItems.Obtain();
                    _items.Add(item);
                }
                else
                {
                    item = _items[i];
                }

                item.SetActive(true);
                _adapter.SetupView(i, item);
            }

            for (var i = _items.Count - 1; i >= _adapter.GetItemsCount(); i--)
            {
                var poolObject = _items[i];
                poolObject.SetActive(false);
                _freeItems.Free(poolObject);
                _items.RemoveAt(i);
            }
        }

        public interface IListAdapter
        {
            int GetItemsCount();
            void SetupView(int i, GameObject itemObject);
        }
    }
}