using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pujario.Utils
{    
    public enum CollectionMode : byte
    {
        /// <summary>Use processed collection</summary> 
        Processed,

        /// <summary>Use raw collection, containing all the items</summary>
        Raw,
    }
    
    /// <summary>
    /// An collection presents similar behavior to "Microsoft.Xna.Framework.Game.SortingFilteringCollection[T]"/> which is internal.
    ///
    /// Needs n of memory; good when you access collection rarely
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class SortingFilteredCollectionLite<T> : ICollection<T>
    {
        public class Enumerator : IEnumerator<T>
        {
            private readonly SortingFilteredCollectionLite<T> _collection;
            private int _currantIndex;

            public bool IsDisposed { get; private set; }
            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public Enumerator(SortingFilteredCollectionLite<T> collection)
            {
                _collection = collection;
                Current = default;
                IsDisposed = false;
                _currantIndex = -1;
            }

            public void Dispose() => IsDisposed = true;

            public bool MoveNext()
            {
                if (++_currantIndex < _collection._filteredItemsCount)
                {
                    Current = _collection._items[_currantIndex];
                    return true;
                }

                Current = default;
                return false;
            }

            public void Reset()
            {
                _currantIndex = -1;
                IsDisposed = false;
            }
        }

        private readonly IComparer<T> _sortingComparer;
        private readonly Predicate<T> _filterPredicate;

        // that comparision puts unsuitable items to the end of collection;
        private readonly Comparison<T> _mixedComparision;

        private readonly Action<T, EventHandler<EventArgs>> _sortFactorChangedSubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _sortFactorChangedUnsubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _filterFactorChangedSubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _filterFactorChangedUnsubscribe;

        private readonly List<T> _items;
        private int _filteredItemsCount;
        private bool _needSorting;

        private readonly Enumerator _cachedEnumerator;

        public CollectionMode Mode { get; set; }

        /// <summary>
        /// Gives collection count, based on <see cref="Mode"/> 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When <see cref="Mode"/> is invalid</exception>
        public int Count => Mode switch
        {
            CollectionMode.Processed => _filteredItemsCount,
            CollectionMode.Raw => _items.Count,
            _ => throw new ArgumentOutOfRangeException(nameof(Mode)),
        };

        public bool IsReadOnly => false;

        private void _onSortFactorChanged(object sender, EventArgs args)
        {
            if (!_needSorting)
            {
                var obj = (T)sender;
                var index = _items.IndexOf(obj);

                _needSorting = index != 0 && _mixedComparision(_items[index - 1], obj) == 1 ||
                               index != _items.Count && _mixedComparision(_items[index + 1], obj) == -1;
            }
        }

        private void _onFilterFactorChanged(object sender, EventArgs args)
        {
            var obj = (T)sender;
            if (_filterPredicate(obj))
            {
                _needSorting = true;
                ++_filteredItemsCount;
            }
            else
            {
                _items.Remove(obj);
                _items.Add(obj);
                --_filteredItemsCount;
            }
        }

        private void _subscribeToItemChanging(T item)
        {
            _filterFactorChangedSubscribe(item, _onFilterFactorChanged);
            _sortFactorChangedSubscribe(item, _onSortFactorChanged);
        }

        private void _unsubscribeToItemChanging(T item)
        {
            _filterFactorChangedUnsubscribe(item, _onFilterFactorChanged);
            _sortFactorChangedUnsubscribe(item, _onSortFactorChanged);
        }

        public SortingFilteredCollectionLite(
            IComparer<T> sortingComparer,
            Predicate<T> filterPredicate,
            Action<T, EventHandler<EventArgs>> sortFactorChangedSubscribe,
            Action<T, EventHandler<EventArgs>> sortFactorChangedUnsubscribe,
            Action<T, EventHandler<EventArgs>> filterFactorChangedSubscribe,
            Action<T, EventHandler<EventArgs>> filterFactorChangedUnsubscribe,
            int capacity = 0
        )
        {
            _sortingComparer = sortingComparer;
            _filterPredicate = filterPredicate;
            _sortFactorChangedSubscribe = sortFactorChangedSubscribe;
            _sortFactorChangedUnsubscribe = sortFactorChangedUnsubscribe;
            _filterFactorChangedSubscribe = filterFactorChangedSubscribe;
            _filterFactorChangedUnsubscribe = filterFactorChangedUnsubscribe;

            _mixedComparision = (x, y) =>
            {
                if (_filterPredicate(x))
                {
                    if (_filterPredicate(y)) return _sortingComparer.Compare(x, y);
                    return -1;
                }

                return _filterPredicate(y) ? 1 : 0;
            };

            _items = new List<T>(capacity);
            _filteredItemsCount = 0;
            _needSorting = false;
            _cachedEnumerator = new Enumerator(this);
            _cachedEnumerator.Dispose();
        }

        public void FlushChanges()
        {
            if (!_needSorting) return;
            _items.Sort(_mixedComparision);
        }

        public void Add(T item)
        {
            if (_filterPredicate(item))
            {
                ++_filteredItemsCount;
                _needSorting = true;
            }

            _items.Add(item);
            _subscribeToItemChanging(item);
        }

        public void Clear()
        {
            foreach (var item in _items) _unsubscribeToItemChanging(item);
            _items.Clear();
            _filteredItemsCount = 0;
            _needSorting = false;
        }

        /// <summary>Check if raw collection contains item</summary>
        public bool Contains(T item) => _items.Contains(item);

        /// <summary>
        /// Copy collection to array. Which collection to copy depends on <see cref="Mode"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When <see cref="Mode"/> is invalid</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            switch (Mode)
            {
                case CollectionMode.Processed:
                    FlushChanges();
                    _items.CopyTo(0, array, arrayIndex, _filteredItemsCount);
                    return;

                case CollectionMode.Raw:
                    _items.CopyTo(array, arrayIndex);
                    return;

                default: throw new ArgumentOutOfRangeException(nameof(Mode));
            }
        }

        public bool Remove(T item)
        {
            bool existed = _items.Remove(item);
            if (existed)
            {
                _unsubscribeToItemChanging(item);
                if (_filterPredicate(item)) --_filteredItemsCount;
            }

            return existed;
        }

        /// <summary>
        /// Such as <see cref="GetEnumerator()"/>, but you can configure collection to enumerate
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>enumerator</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <param name="mode"></param> is invalid</exception>
        public IEnumerator<T> GetEnumerator(CollectionMode mode)
        {
            switch (mode)
            {
                case CollectionMode.Processed:
                    FlushChanges();
                    if (!_cachedEnumerator.IsDisposed) return new Enumerator(this);
                    _cachedEnumerator.Reset();
                    return _cachedEnumerator;

                case CollectionMode.Raw: return ((IEnumerable<T>)_items).GetEnumerator();
                default: throw new ArgumentOutOfRangeException(nameof(Mode));
            }
        }

        /// <summary>
        /// Presents enumeration thew sorted, filtered partial collection if <see cref="Mode"/> is <see cref="CollectionMode.Processed"/>,
        /// threw full collection if <see cref="Mode"/> is <see cref="CollectionMode.Raw"/>.
        /// 
        /// If collection changes while enumeration, you can receive unexpected behavior  
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator() => GetEnumerator(Mode);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(Mode);
    }


    /// <summary>
    /// An collection presents similar behavior to "Microsoft.Xna.Framework.Game.SortingFilteringCollection[T]"/> which is internal.
    /// Close copy of Xna`s realisation
    ///
    /// Needs ~2n of memory; gives good performance when there are many short changes and then accessing to collection    
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class SortingFilteredCollection<T> : ICollection<T>
    {
        private readonly IComparer<T> _sortingComparer;
        private readonly Predicate<T> _filterPredicate;
        private readonly Action<T, EventHandler<EventArgs>> _sortFactorChangedSubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _sortFactorChangedUnsubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _filterFactorChangedSubscribe;
        private readonly Action<T, EventHandler<EventArgs>> _filterFactorChangedUnsubscribe;

        private readonly List<T> _items;
        private readonly List<T> _filteredItems;
        private bool _needFiltering;

        private List<T> _itemsToAdd;
        private List<int> _indexesToRemove;

        public CollectionMode Mode { get; set; }

        /// <summary>
        /// Gives collection count, based on <see cref="Mode"/> 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When <see cref="Mode"/> is invalid</exception>
        public int Count => Mode switch
        {
            CollectionMode.Processed => _filteredItems.Count,
            CollectionMode.Raw => _items.Count,
            _ => throw new ArgumentOutOfRangeException(nameof(Mode)),
        };

        public bool IsReadOnly => false;

        private void _onSortFactorChanged(object sender, EventArgs args)
        {
            var obj = (T)sender;
            _itemsToAdd.Add(obj);
            _indexesToRemove.Add(_items.IndexOf(obj));
            _unsubscribeToItemChanging(obj);
            if (!_needFiltering) _needFiltering = !_filterPredicate(obj);
        }

        private void _onFilterFactorChanged(object sender, EventArgs args) => _needFiltering = true;

        private void _subscribeToItemChanging(T item)
        {
            _filterFactorChangedSubscribe(item, _onFilterFactorChanged);
            _sortFactorChangedSubscribe(item, _onSortFactorChanged);
        }

        private void _unsubscribeToItemChanging(T item)
        {
            _filterFactorChangedUnsubscribe(item, _onFilterFactorChanged);
            _sortFactorChangedUnsubscribe(item, _onSortFactorChanged);
        }

        private void _flushChanges()
        {
            if (_indexesToRemove.Count != 0)
            {
                _indexesToRemove.Sort();
                for (int i = _indexesToRemove.Count - 1; i >= 0; --i)
                    _items.RemoveAt(_indexesToRemove[i]);
                _indexesToRemove.Clear();
            }

            if (_itemsToAdd.Count == 0) return;

            _itemsToAdd.Sort(_sortingComparer);
            int sCount = _itemsToAdd.Count,
                dCount = _items.Count,
                si = 0,
                di = _items.BinarySearch(_itemsToAdd[0], _sortingComparer);
            di = di < 0 ? di ^ -1 : di;
            for (; si < sCount && di < dCount; ++di)
            {
                var item = _itemsToAdd[si];
                if (_sortingComparer.Compare(item, this._items[di]) < 0)
                {
                    _subscribeToItemChanging(item);
                    _items.Insert(di, item);
                    ++si;
                }
            }

            for (; si < sCount; ++si)
            {
                var item = _itemsToAdd[si];
                _subscribeToItemChanging(item);
                _items.Add(item);
            }

            _itemsToAdd.Clear();
        }

        private void _rebuildFilteredCollection()
        {
            _filteredItems.Clear();
            foreach (var item in _items)
            {
                if (_filterPredicate(item)) _filteredItems.Add(item);
            }

            _needFiltering = false;
        }

        public SortingFilteredCollection(
            IComparer<T> sortingComparer,
            Predicate<T> filterPredicate,
            Action<T, EventHandler<EventArgs>> sortFactorChangedSubscribe,
            Action<T, EventHandler<EventArgs>> sortFactorChangedUnsubscribe,
            Action<T, EventHandler<EventArgs>> filterFactorChangedSubscribe,
            Action<T, EventHandler<EventArgs>> filterFactorChangedUnsubscribe,
            int capacity = 0
        )
        {
            _sortingComparer = sortingComparer;
            _filterPredicate = filterPredicate;
            _sortFactorChangedSubscribe = sortFactorChangedSubscribe;
            _sortFactorChangedUnsubscribe = sortFactorChangedUnsubscribe;
            _filterFactorChangedSubscribe = filterFactorChangedSubscribe;
            _filterFactorChangedUnsubscribe = filterFactorChangedUnsubscribe;

            _items = new List<T>(capacity);
            _filteredItems = new List<T>(capacity);
            _itemsToAdd = new List<T>();
            _indexesToRemove = new List<int>();

            _needFiltering = false;
        }

        public void Add(T item)
        {
            _itemsToAdd.Add(item);
            if (!_needFiltering) _needFiltering = !_filterPredicate(item);
        }

        public void Clear()
        {
            foreach (var item in _items) _unsubscribeToItemChanging(item);
            _items.Clear();
            _filteredItems.Clear();
            _indexesToRemove.Clear();
            _itemsToAdd.Clear();
            _needFiltering = false;
        }

        /// <summary>Check if raw collection contains item</summary>
        public bool Contains(T item) => _items.Contains(item);

        /// <summary>
        /// Copy collection to array. Which collection to copy depends on <see cref="Mode"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When <see cref="Mode"/> is invalid</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _flushChanges();
            switch (Mode)
            {
                case CollectionMode.Processed:
                    if (_needFiltering) _rebuildFilteredCollection();
                    _filteredItems.CopyTo(array, arrayIndex);
                    return;

                case CollectionMode.Raw:
                    _items.CopyTo(array, arrayIndex);
                    return;

                default: throw new ArgumentOutOfRangeException(nameof(Mode));
            }
        }

        public bool Remove(T item)
        {
            var count = _items.Count;
            var indToRemove = _items.IndexOf(item);
            while (indToRemove >= 0)
            {
                var existedInd = _indexesToRemove.IndexOf(indToRemove);
                if (existedInd < 0)
                {
                    _indexesToRemove.Add(indToRemove);
                    _unsubscribeToItemChanging(item);
                    return true;
                }

                if (++indToRemove == count) return false;
                indToRemove = _items.IndexOf(item, indToRemove);
            }

            return false;
        }

        /// <summary>
        /// Such as <see cref="GetEnumerator()"/>, but you can configure collection to enumerate
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>enumerator</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <param name="mode"></param> is invalid</exception>
        public IEnumerator<T> GetEnumerator(CollectionMode mode)
        {
            _flushChanges();
            switch (mode)
            {
                case CollectionMode.Processed:
                    if (_needFiltering) _rebuildFilteredCollection();
                    return ((IEnumerable<T>)_filteredItems).GetEnumerator();

                case CollectionMode.Raw: return ((IEnumerable<T>)_items).GetEnumerator();
                default: throw new ArgumentOutOfRangeException(nameof(Mode));
            }
        }

        /// <summary>
        /// Presents enumeration thew sorted, filtered partial collection if <see cref="Mode"/> is <see cref="CollectionMode.Processed"/>,
        /// threw full collection if <see cref="Mode"/> is <see cref="CollectionMode.Raw"/>.
        /// 
        /// If collection changes while enumeration, you can receive unexpected behavior  
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator() => GetEnumerator(Mode);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(Mode);

        public void FlushChanges()
        {
            _flushChanges();
            if (_needFiltering) _rebuildFilteredCollection();
        }
    }
}