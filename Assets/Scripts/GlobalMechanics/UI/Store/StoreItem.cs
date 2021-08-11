// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using Player;
using UnityEngine;

namespace GlobalMechanics.UI.Store
{
    [CreateAssetMenu(fileName = "Item for Sale", menuName = "ScriptableObjects/Store Item", order = 0)]
    public class StoreItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private string tooltip;
        [SerializeField] private uint cost;
        [SerializeField] private uint initialStock;
        [SerializeField] private bool hasSharedStock;
        [SerializeField] private StoreItem[] sharedStockWith;

        private uint _cart;
        private uint _stock;
        private uint _inventory;
        public string ItemName => itemName;
        public bool IsSoldOut => _stock == 0;
        public uint Price => cost;
        public uint Stock => _stock;
        public uint Cart => _cart;

        public uint Inventory => _inventory;
        public Sprite Icon => icon;
        public string Tooltip => tooltip;

        private void OnEnable()
        {
            _stock = initialStock;
            _inventory = 0;
        }

        public void Buy()
        {
            _inventory += _cart;
        }

        public PlayerInventory.Package BuyPhysical()
        {
            var itemsToBuy = _cart;
            return new PlayerInventory.Package(() => IncreaseInventoryCount(itemsToBuy));
        }

        private void IncreaseInventoryCount(uint amount)
        {
            _inventory += amount;
            Debug.Log(ItemName + " + " + amount);
        }

        public void AddToCart()
        {
            if (_stock <= 0) return;
            _cart++;
            if (hasSharedStock)
            {
                foreach (var item in sharedStockWith)
                {
                    item._stock--;
                }
            }
            _stock--;
        }

        public void ResetCart()
        {
            _cart = 0;
        }
        
        public void EmptyCart()
        {
            _stock += _cart;
            if (hasSharedStock)
            {
                foreach (var storeItem in sharedStockWith)
                {
                    storeItem._stock += _cart;
                }
            }
            _cart = 0;
        }

        public void RemoveFromCart()
        {
            if (_cart <= 0) return;
            
            if (hasSharedStock)
            {
                foreach (var item in sharedStockWith)
                {
                    item._stock++;
                }
            }
            _cart--;
            _stock++;
        }
    }
}