using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI.Store
{
    public class Store : MonoBehaviour, ISmartphoneService
    {
        private const int THREE_DIGIT = 1000;

        private const int MAX_MONEY_AMOUNT = 1000000;

        [SerializeField]
        private PlayerInventory inventory;
    
        [Header("UI Display")] 
        [SerializeField]
        private GameObject shopItemDisplayUi;
        [SerializeField]
        private RectTransform displayUiHolder;

        [Header("Cart Display Settings")]
        public TextMeshProUGUI totalUi;

        [Header("Player Inventory Display")] 
        public GameObject inventoryDisplay;
        [SerializeField] 
        private GameObject inventoryEntryPrefab;
        [SerializeField]
        private Transform inventoryEntryHolder;

        private Dictionary<int, InventoryEntryUi> _inventoryEntries;
    
        public TextMeshProUGUI playerMoney;

        private uint TotalAmount => inventory.Items.
            Select(item => item.Price * item.Cart).Aggregate((u, u1) => u + u1);

        public MailBox box;
        private bool _isInventoryOpen = false;
    
        private List<ShopItemAdapter> _itemAdapters;

        // Start is called before the first frame update
        void Start()
        {
            if (_itemAdapters == null || _inventoryEntries == null) Init();
            ResetCart();
            UpdateCartPrice();
            playerMoney.text = FormatMoney(inventory.Money);
        }

        private void Init()
        {
            _itemAdapters = new List<ShopItemAdapter>();
            _inventoryEntries = new Dictionary<int, InventoryEntryUi>();
            var items = inventory.Items;
            foreach (var i in items)
            {
                var itemUi = Instantiate(shopItemDisplayUi, displayUiHolder);
                var entryUi = Instantiate(inventoryEntryPrefab, inventoryEntryHolder);
                var adapter = itemUi.GetComponent<ShopItemAdapter>();
                var entry = entryUi.GetComponent<InventoryEntryUi>();
                _itemAdapters.Add(adapter);
                _inventoryEntries.Add(i.GetInstanceID(), entry);
                adapter.AttachStoreItem(this, i);
            }
        }

        public void Buy()
        {
            inventory.SpendMoney(TotalAmount, out var successful);
            if (!successful)
            {
                foreach (var i in inventory.Items)
                {
                    i.EmptyCart();
                }
            
                foreach (var a in _itemAdapters)
                {
                    a.UpdateCartCount();
                }
                return;
            }

            var package = new PlayerInventory.Package(() => Debug.Log("Package opened"));
        
            foreach (var i in inventory.Items)
            {
                package.UniteWith(i.BuyPhysical());
            }
        
            box.Fill(package);
            ResetCart();
            UpdateCartPrice();
            playerMoney.text = FormatMoney(inventory.Money);
            ShowSoldOutPanels();
        }
    

        public void Home()
        {
            this.gameObject.SetActive(false);
        }

        public void Open()
        {
            this.gameObject.SetActive(true);
            if (_itemAdapters == null || _inventoryEntries == null) Init();
        
            ResetCart();
            UpdateCartPrice();
        
            if (inventory == null) return;
            playerMoney.text = FormatMoney(inventory.Money);
        }

        public bool IsOpened()
        {
            return this.gameObject.activeInHierarchy;
        }
    
        public void Back()
        {
            if (_isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                foreach (var i in inventory.Items)
                {
                    i.EmptyCart();
                }
                this.gameObject.SetActive(false); 
            }
        }

        public void UpdateCartPrice()
        {
            totalUi.text = FormatMoney(TotalAmount);
        }

        private string FormatMoney(uint money)
        {
            string outputPrice;

            if (money < MAX_MONEY_AMOUNT)
            {
                outputPrice = (money > 1000) ? (money / 1000).ToString() + " " : "";
                outputPrice += money.ToString() + " $";
            }
            else
            {
                outputPrice = "999 999+ $";
            }

            return outputPrice;
        }

        public void OpenInventory()
        {
            if (_isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                inventoryDisplay.SetActive(true);
                foreach (var item in inventory.Items)
                {
                    _inventoryEntries[item.GetInstanceID()].SetText(item.ItemName, item.Inventory);
                }
                _isInventoryOpen = true;
            }
        }

        private void CloseInventory()
        {
            inventoryDisplay.SetActive(false);
        
            _isInventoryOpen = false;
        }
    
        private void ResetCart()
        {
            foreach (var i in inventory.Items)
            {
                i.ResetCart();
            }

            foreach (var adapter in _itemAdapters)
            {
                adapter.UpdateCartCount();
            }
        }

        private void ShowSoldOutPanels()
        {
            foreach (var shopItemAdapter in _itemAdapters)
            {
                shopItemAdapter.CheckSoldOut();
            }
        }
    }
}
