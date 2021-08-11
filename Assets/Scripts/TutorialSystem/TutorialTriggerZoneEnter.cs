// MIT License
// Copyright (c) 2020 obrda
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace TutorialSystem
{
    public class TutorialTriggerZoneEnter : MonoBehaviour
    {
        private TutorialSystem system;

        void Start()
        {
            system = transform.GetComponentInParent<TutorialSystem>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.name == "Player")
                system.Triggered();
        }

    }
}
