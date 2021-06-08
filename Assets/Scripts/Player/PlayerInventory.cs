using System;
using System.Collections.Generic;
using System.Linq;
using GlobalMechanics.UI.Store;
using UnityEngine;

namespace Player
{

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        private StoreItem[] inventory;

        public uint Money { get; private set; }
        public IReadOnlyList<StoreItem> Items => inventory.ToList();

        public void GetPaid(uint amount)
        {
            Money += amount;
        }

        public void SpendMoney(uint amount, out bool isTransactionSuccessful)
        {
            if (Money >= amount)
            {
                Money -= amount;
                isTransactionSuccessful = true;
            }
            else
            {
                isTransactionSuccessful = false;
            }
        }

        public class Package
        {
            private Action _packageUnpack;

            internal Package(Action increaseAmountAction)
            {
                _packageUnpack = increaseAmountAction;
            }

            public Package UniteWith(Package pack2)
            {
                _packageUnpack += pack2._packageUnpack;
                return this;
            }

            public void Unpack()
            {
                _packageUnpack.Invoke();
            }
        }
    }
}