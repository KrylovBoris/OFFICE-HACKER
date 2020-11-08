using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    
    public enum Item {
        Money = -1, 
        Virus = 0, 
        ConcealedVirus = 1, 
        PhishingWebSite = 2, 
        ConvincingPhishingWebSite = 3, 
        UsbCord = 4, 
        UsbToMicroUsbAdapter = 5
        
    }
    
    public class PlayerInventory : MonoBehaviour
    {       
        private Dictionary<Item, uint> _inventory;

        // Start is called before the first frame update
        void Start()
        {
            _inventory = new Dictionary<Item, uint>();
            foreach (var item in (Item[]) Enum.GetValues(typeof(Item)))
            {
                _inventory.Add(item, 0);
            }
        }
        
        public void Convert(Item itemFrom, uint amountFrom, Item itemTo, uint amountTo, out bool isTransactionSuccessful)
        {
            if (_inventory[itemFrom] >= amountFrom)
            {
                _inventory[itemFrom] -= amountFrom;
                _inventory[itemTo] += amountTo;
                isTransactionSuccessful = true;
            }
            else
            {
                isTransactionSuccessful = false;
            }
        }

        public void GetPaid(uint amount)
        {
            IncreaseItem(Item.Money, amount);
        }

        public void SpendMoney(uint amount, out bool isTransactionSuccessful)
        {
            if (_inventory[Item.Money] >= amount)
            {
                _inventory[Item.Money] -= amount;
                isTransactionSuccessful = true;
            }
            else
            {
                isTransactionSuccessful = false;
            }
        }
        
        public void IncreaseItem(Item itemToIncrease, uint increaseAmount)
        {
            _inventory[itemToIncrease] += increaseAmount;
        }

        public uint ItemAmount(Item i)
        {
            return _inventory[i];
        }
        
    }
}