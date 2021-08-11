// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using GlobalMechanics.UI.Store;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalMechanics.UI
{
    public class ShopItemAdapter : MonoBehaviour
    {
        private StoreItem _attachedItem;
        private Store.Store _store;
        [SerializeField]
        private TextMeshProUGUI nameDisplay;
        [SerializeField]
        private TextMeshProUGUI priceDisplay;
        [SerializeField]
        private TMP_InputField amountDisplay;
        [SerializeField]
        private Image iconDisplay;
        [SerializeField]
        private GameObject soldOutPanel;

    
        public void AttachStoreItem(Store.Store store, StoreItem item)
        {
            _store = store;
            _attachedItem = item;
            GetComponentInChildren<TipUi>().tipText = _attachedItem.Tooltip;
            nameDisplay.text = _attachedItem.name;
            priceDisplay.text = "Цена: " + _attachedItem.Price + " $";
            iconDisplay.sprite = _attachedItem.Icon;
            amountDisplay.text = _attachedItem.Cart.ToString();
        }

        public void Add()
        {
            _attachedItem.AddToCart();
            amountDisplay.text = _attachedItem.Cart.ToString();
            _store.UpdateCartPrice();
        }

        public void Remove()
        {
            _attachedItem.RemoveFromCart();
            amountDisplay.text = _attachedItem.Cart.ToString();
            _store.UpdateCartPrice();
        }

        public void UpdateCartCount()
        {
            amountDisplay.text = _attachedItem.Cart.ToString();
        }

        public void CheckSoldOut()
        {
            if (_attachedItem.IsSoldOut) soldOutPanel.SetActive(true);
        }
    }
}
