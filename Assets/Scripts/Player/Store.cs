using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

public class Store : MonoBehaviour, ISmartphoneService
{
    private const int THREE_DIGIT = 1000;
    private const int MAX_MONEY_AMOUNT = 1000000;
    
    private PlayerInventory _inventory;
    [Header("Virus Settings")]
    public uint virusCost;
    public uint virusStock = 99;
    public uint VirusCart => _cart[(int) Item.Virus];
    public TextMeshProUGUI virusPriceDisplay;
    public GameObject virusSoldOutPanel;
    
    [Header("Consealed Virus Settings")]
    public uint concealedVirusCost;
    public uint concealedVirusStock = 99;
    public uint ConcealedVirusCart => _cart[(int) Item.ConcealedVirus];
    public TextMeshProUGUI concealedVirusPriceDisplay;
    public GameObject concealedVirusSoldOutPanel;
    
    [Header("Phishing Settings")]
    public uint phishingCost;
    public uint phishingStock = 99;
    public uint PhishingCart => _cart[(int) Item.PhishingWebSite];
    public TextMeshProUGUI phishingPriceDisplay;
    public GameObject phishingSoldOutPanel;

    
    [Header("Convincing Phishing Settings")]
    public uint convincingPhishingCost;
    public uint convincingPhishingStock = 99;
    public uint ConvincingPhishingCart => _cart[(int) Item.ConvincingPhishingWebSite];
    public TextMeshProUGUI convincingPhishingPriceDisplay;
    public GameObject convincingPhishingSoldOutPanel;
    
    [Header("USB Cord Settings")]
    public uint usbCordCost;
    public uint usbCordStock = 99;
    public uint UsbCordCart => _cart[(int) Item.UsbCord];
    public TextMeshProUGUI usbCordPriceDisplay;
    public GameObject usbCordSoldOutPanel;

    
    [Header("Adapter Settings")]
    public uint adapterCost;
    public uint adapterStock = 99;
    public uint AdapterCart => _cart[(int) Item.UsbToMicroUsbAdapter];
    public TextMeshProUGUI adapterPriceDisplay;
    public GameObject adapterSoldOutPanel;

    [Header("Cart Display Settings")] 
    public TMP_InputField[] cartUi;
    public TextMeshProUGUI totalUi;

    [Header("Player Inventory Display")] 
    public GameObject inventoryDisplay;

    public TextMeshProUGUI playerMoney;
    public TextMeshProUGUI[] itemDisplay;

    private GameObject[] _soldOutPanels;
    private uint[] _stock;
    private uint[] _cart;
    private uint[] _prices;
    private uint _totalAmount;


    public MailBox _box;
    private bool _isInventoryOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        _inventory = GameManager.gm.player.GetComponent<PlayerInventory>();
        _stock = new uint[(int) Item.UsbToMicroUsbAdapter + 1];
        _cart = new uint[(int) Item.UsbToMicroUsbAdapter + 1];
        _prices = new uint[(int) Item.UsbToMicroUsbAdapter + 1];
        _soldOutPanels = new GameObject[(int) Item.UsbToMicroUsbAdapter + 1];
        virusPriceDisplay.text = "Цена: " + virusCost + " $";
        concealedVirusPriceDisplay.text = "Цена: " + concealedVirusCost + " $";
        phishingPriceDisplay.text = "Цена: " + phishingCost + " $";
        convincingPhishingPriceDisplay.text = "Цена: " + convincingPhishingCost + " $";
        usbCordPriceDisplay.text = "Цена: " + usbCordCost + " $";
        adapterPriceDisplay.text = "Цена: " + adapterCost + " $";

        _stock[(int) Item.Virus] = virusStock;
        _stock[(int) Item.ConcealedVirus] = concealedVirusStock;
        _stock[(int) Item.PhishingWebSite] = phishingStock;
        _stock[(int) Item.ConvincingPhishingWebSite] = convincingPhishingStock;
        _stock[(int) Item.UsbCord] = usbCordStock;
        _stock[(int) Item.UsbToMicroUsbAdapter] = adapterStock;
        
        _prices[(int) Item.Virus] = virusCost;
        _prices[(int) Item.ConcealedVirus] = concealedVirusCost;
        _prices[(int) Item.PhishingWebSite] = phishingCost;
        _prices[(int) Item.ConvincingPhishingWebSite] = convincingPhishingCost;
        _prices[(int) Item.UsbCord] = usbCordCost;
        _prices[(int) Item.UsbToMicroUsbAdapter] = adapterCost;
        
        _soldOutPanels[(int) Item.Virus] = virusSoldOutPanel;
        _soldOutPanels[(int) Item.ConcealedVirus] = concealedVirusSoldOutPanel;
        _soldOutPanels[(int) Item.PhishingWebSite] = phishingSoldOutPanel;
        _soldOutPanels[(int) Item.ConvincingPhishingWebSite] = convincingPhishingSoldOutPanel;
        _soldOutPanels[(int) Item.UsbCord] = usbCordSoldOutPanel;
        _soldOutPanels[(int) Item.UsbToMicroUsbAdapter] = adapterSoldOutPanel;
        
        ResetCart();
        UpdateCartPrice();
        playerMoney.text = FormatMoney(_inventory.ItemAmount(Item.Money));
    }

    public void Buy()
    {
        _inventory.SpendMoney(_totalAmount, out var successful);
        if (!successful)
        {
            return;
        }
        
        Dictionary<Item, uint> package = new Dictionary<Item, uint>();
        foreach (var item in (Item[]) Enum.GetValues(typeof(Item)))
        {
            if (item != Item.Money && _cart[(int) item] > 0)
            {
                if (_stock[(int) item] >= _cart[(int) item])
                {
                    package.Add(item, _cart[(int) item]);
                }
                else
                {
                    Debug.LogError("Cart can't exceed stock");
                }
                            
                _stock[(int) item] -= _cart[(int) item];

                if (item == Item.Virus)
                {
                    _stock[(int) Item.ConcealedVirus] -= _cart[(int) item];
                }
            
                if (item == Item.ConcealedVirus)
                {
                    _stock[(int) Item.Virus] -= _cart[(int) item];
                }
            }
        }
        _box.Fill(package);
        ResetCart();
        UpdateCartPrice();
        playerMoney.text = FormatMoney(_inventory.ItemAmount(Item.Money));
        ShowSoldOutPanels();
    }

    public void AddToCart(int i)
    {
        if (i == (int) Item.Virus || i == (int) Item.ConcealedVirus)
        {
            if ( _stock[i] > (_cart[(int) Item.Virus] + _cart[(int) Item.ConcealedVirus]))
            {
                _cart[i]++;
                cartUi[i].text = _cart[i].ToString();
                _totalAmount += _prices[i];
                UpdateCartPrice();
            }
        }
        else
        {
            if (_stock[i] > _cart[i])
            {
                _cart[i]++;
                cartUi[i].text = _cart[i].ToString();
                _totalAmount += _prices[i];
                UpdateCartPrice();
            }
        }
    }
    
    public void RemoveFromCart(int i)
    {
        if (_cart[i] > 0)
        {
            _cart[i]--;
            cartUi[i].text = _cart[i].ToString();
            _totalAmount -= _prices[i];
            UpdateCartPrice();
        }
    }

    public void Home()
    {
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        ResetCart();
        UpdateCartPrice();
        if (_inventory == null) return;
        playerMoney.text = FormatMoney(_inventory.ItemAmount(Item.Money));
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
            this.gameObject.SetActive(false); 
        }
    }

    private void UpdateCartPrice()
    {
        totalUi.text = FormatMoney(_totalAmount);

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
            itemDisplay[(int) Item.Virus].text = "x " + _inventory.ItemAmount(Item.Virus).ToString();
            itemDisplay[(int) Item.ConcealedVirus].text = "x " + _inventory.ItemAmount(Item.ConcealedVirus).ToString();
            itemDisplay[(int) Item.PhishingWebSite].text = "x " + _inventory.ItemAmount(Item.PhishingWebSite).ToString();
            itemDisplay[(int) Item.ConvincingPhishingWebSite].text = "x " + _inventory.ItemAmount(Item.ConvincingPhishingWebSite).ToString();
            itemDisplay[(int) Item.UsbCord].text = "x " + _inventory.ItemAmount(Item.UsbCord).ToString();
            itemDisplay[(int) Item.UsbToMicroUsbAdapter].text = "x " + _inventory.ItemAmount(Item.UsbToMicroUsbAdapter).ToString();
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
        if (_cart == null) return;
        for (int i = 0; i < _cart.Length; i++)
        {
            _cart[i] = 0;
            cartUi[i].text = _cart[i].ToString();
        }

        _totalAmount = 0;
    }

    private void ShowSoldOutPanels()
    {
        foreach (var item in (Item[]) Enum.GetValues(typeof(Item)))
        {
            var i = (int) item;
            if (i < 0) return;
            if (_stock[i] == 0)
            {
                _soldOutPanels[i].SetActive(true);
            }
        }
    }
}
