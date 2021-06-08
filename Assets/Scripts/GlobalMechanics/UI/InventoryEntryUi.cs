
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
