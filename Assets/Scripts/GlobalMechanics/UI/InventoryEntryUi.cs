// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class InventoryEntryUi : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI caption;
        [SerializeField]
        private TextMeshProUGUI count;

        public void SetText(string itemName, uint inventoryCount)
        {
            caption.text = itemName;
            count.text = "x " + inventoryCount;
        }
    }
}
