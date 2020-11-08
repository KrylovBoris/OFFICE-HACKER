using UnityEngine;

namespace Scripts.Test.UI
{
    public class SmartphoneService: MonoBehaviour
    {
        public bool IsOpened()
        {
            return this.gameObject.activeInHierarchy;
        }

        public void Open()
        {
            this.gameObject.SetActive(true);
        }
        
        public void Home()
        {
            this.gameObject.SetActive(false);
        }
        
        public virtual void Back()
        {
            this.gameObject.SetActive(false);
        }
    }
}