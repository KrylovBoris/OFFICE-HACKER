// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace Player
{
    public class MovementScript : MonoBehaviour
    {
        [SerializeField]
        private float walkingSpeed = 6.0f;
        [SerializeField]
        private float sprintingCoefficient = 1.25f;

        [SerializeField] private float mass = 60.0f;
        [SerializeField] private bool useGravity = true;
        [SerializeField] private Transform groundChecker;
        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private string FloorLayer = "Default";
        private int ground;
        private CharacterController _controller;
        private Vector3 _intendedDirection;
        private bool _isGrounded;
        public bool IsSprinting { get; set; }
    
    // Start is called before the first frame update
        void Start()
        {
            ground = LayerMask.NameToLayer(FloorLayer);
            _controller = GetComponent<CharacterController>();
            
        }

        public void MoveInDirection(Vector3 direction)
        {
            _intendedDirection.x = (direction.normalized * walkingSpeed * ((IsSprinting) ? sprintingCoefficient : 1)).x;
            _intendedDirection.z = (direction.normalized * walkingSpeed * ((IsSprinting) ? sprintingCoefficient : 1)).z;
            Debug.Assert(_intendedDirection != Vector3.zero);
        }

      
        private void FixedUpdate()
        {
            _intendedDirection = transform.localRotation * _intendedDirection;
            _isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
            if (useGravity && !_controller.isGrounded)
            {
            _intendedDirection += Physics.gravity * mass;
            }
            else
            {
                _intendedDirection.y = (Physics.gravity * mass).y;
            }

            _controller.Move(_intendedDirection * Time.deltaTime);
            _intendedDirection.x = 0;
            _intendedDirection.z = 0;
        }
}
}
