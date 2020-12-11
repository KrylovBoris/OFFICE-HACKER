using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
