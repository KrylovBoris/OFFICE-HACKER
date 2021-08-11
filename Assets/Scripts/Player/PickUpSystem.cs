// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace Player
{
    public class PickUpSystem : MonoBehaviour
    {
        public bool IsCarryingSmomething { get; private set; }
        [SerializeField] private GameObject objectHolder;
        [SerializeField]
        private float range = 2.0f;

        [SerializeField] private float forceLimit = 50.0f;
        private Ray _ray;
    
        private FixedJoint _joint;

        private void Start()
        {
            _joint = objectHolder.GetComponent<FixedJoint>();
        }

        private void Update()
        {
            if (TestForce())
            {
                Drop();
            }
        }

        private bool TestForce()
        {
            return _joint.currentForce.magnitude > forceLimit;
        }
    
        public void PickUp(GameObject item)
        {
            var rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                _joint.connectedBody = rb;
                IsCarryingSmomething = true;
            }
            else
            {
                Debug.LogError("PickUp object has no rigidbody");
            }
        }

        public void Drop()
        {
            _joint.connectedBody = null;
            IsCarryingSmomething = false;
        }
    }
}
