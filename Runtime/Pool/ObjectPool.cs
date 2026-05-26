using HatzeLaboratory.GameBasicSystem.Runtime.Pool.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Pool
{
    /// <summary>
    /// <see cref="IObjectPoolItem"/> を管理するオブジェクトプール。
    /// 固定容量で初期化され、アイテムの取得・返却を行います。
    /// </summary>
    public sealed class ObjectPool
    {
        private readonly List<PoolItemData> _itemList = new();
        private readonly Queue<int> _availableIndexQueue = new();

        /// <summary>
        /// プール内の総アイテム数を取得
        /// </summary>
        public int Count => _itemList.Count;

        /// <summary>
        /// 指定した容量でプールを初期化し、アイテムを生成
        /// </summary>
        /// <param name="capacity">プールに生成するアイテム数。1 以上を指定してください。</param>
        /// <param name="poolItemCreateFunction">アイテムを生成する関数</param>
        public ObjectPool(int capacity, Func<IObjectPoolItem> poolItemCreateFunction)
        {
            if (capacity <= 0)
            {
                Debug.LogError("Object pool capacity must be greater than zero.");
                return;
            }

            if (poolItemCreateFunction == null)
            {
                Debug.LogError("Object pool create function cannot be null.");
                return;
            }

            for (int i = 0; i < capacity; i++)
            {
                IObjectPoolItem item = poolItemCreateFunction.Invoke();
                if (item == null)
                {
                    Debug.LogError("Object pool create function returned null item.");
                    continue;
                }

                _itemList.Add(new()
                {
                    Item = item,
                    IsUsed = false
                });

                _availableIndexQueue.Enqueue(i);
            }
        }

        /// <summary>
        /// プールからアイテムを取得
        /// </summary>
        /// <returns>利用可能なアイテム。空きがない場合は <c>null</c></returns>
        public IObjectPoolItem GetItem()
        {
            if (_availableIndexQueue.Count == 0)
            {
                Debug.LogError("No available items in the pool.");
                return null;    
            }

            int index = _availableIndexQueue.Dequeue();
            PoolItemData data = _itemList[index];
            if (data.Item == null)
            {
                Debug.LogError("Item in pool is null.");
                return null;
            }

            data.IsUsed = true;
            return data.Item;
        }

        /// <summary>
        /// アイテムをプールに返却。返却されたアイテムは自動的に <see cref="IObjectPoolItem.Hide"/> が呼ばれます。
        /// </summary>
        /// <param name="item">返却するアイテム</param>
        public void ReturnItem(IObjectPoolItem item)
        {
            if (item == null)
            {
                Debug.LogError("Cannot return a null item to the pool.");
                return;
            }

            item.Hide();
            int index = _itemList.FindIndex(x => x.Item == item);
            if (index < 0)
            {
                Debug.LogError("It is not pool item.");
                return;
            }

            PoolItemData data = _itemList[index];
            if (!data.IsUsed)
            {
                Debug.LogWarning("Item is not currently in use, cannot return it to the pool.");
                return;
            }

            data.IsUsed = false;
            _availableIndexQueue.Enqueue(index);
        }

        /// <summary>
        /// 使用中のすべてのアイテムをプールに返却
        /// </summary>
        public void ReturnAllItems()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                PoolItemData itemData = _itemList[i];
                if (itemData.Item == null)
                {
                    Debug.LogError("Item in pool is null.");
                    continue;
                }

                itemData.Item.Hide();
                if (itemData.IsUsed)
                {
                    itemData.IsUsed = false;
                    _availableIndexQueue.Enqueue(i);
                }
            }
        }

        private class PoolItemData
        {
            public IObjectPoolItem Item;
            public bool IsUsed;
        }
    }
}