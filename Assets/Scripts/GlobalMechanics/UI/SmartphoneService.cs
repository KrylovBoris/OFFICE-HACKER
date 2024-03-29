// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace GlobalMechanics.UI
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