// MIT License
// Copyright (c) 2020 obrda
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace TutorialSystem
{
    public class TutorialTriggerObjectEnabled : MonoBehaviour
    {
        private TutorialSystem system;

        void Start()
        {
            system = GameObject.Find("TutorialObjects").GetComponent<TutorialSystem>();
            system.Triggered();
        }
    }
}
