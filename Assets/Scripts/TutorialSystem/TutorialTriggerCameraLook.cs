// MIT License
// Copyright (c) 2020 obrda
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace TutorialSystem
{
    public class TutorialTriggerCameraLook : MonoBehaviour
    {
        [SerializeField] private float range = 2.0f;
        private Ray _ray;
        private TutorialSystem system;

        void Start()
        {
            system = transform.GetComponentInParent<TutorialSystem>();
        }

        void Update()
        {
            _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(_ray, out var hit, range, LayerMask.GetMask("Tutorial")))
                system.Triggered();
        }
    }
}
