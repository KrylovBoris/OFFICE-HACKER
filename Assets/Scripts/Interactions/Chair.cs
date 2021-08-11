// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System.Threading.Tasks;
using UnityEngine;

namespace Interactions
{
    public class Chair : MonoBehaviour
    {
        public float turningSpeed = 5.0f;
        private Quaternion _defaultDirection;
        private Quaternion _targetTransform;
        private bool _isRotating = false;
        private float _turningStartTime;
        private Quaternion _startingPos;

        private void Start()
        {
            _defaultDirection = transform.rotation;
        }

        public async void ResetSeat()
        {
            _isRotating = true;
            _turningStartTime = Time.time;
            await ReturnToDefault();
            _isRotating = false;
        }

        public async void Turn(float angle)
        {
            _isRotating = true;
            _turningStartTime = Time.time;
            await TurnByAngle(angle);
            _isRotating = false;
        }

        private async Task TurnByAngle(float angle)
        {
            _startingPos = transform.rotation;
            _targetTransform = Quaternion.AngleAxis(angle, Vector3.up);
        
            while (!IsChairAllignedWith(_targetTransform))
            {
                Quaternion rot = Quaternion.Slerp(_startingPos, _targetTransform, turningSpeed * (Time.time - _turningStartTime));
                transform.rotation = rot;
                await Task.Yield();
            }
        }

        private async Task ReturnToDefault()
        {
            _startingPos = transform.rotation;
            _targetTransform = _defaultDirection;
        
            while (!IsChairAllignedWith(_targetTransform))
            {
                Quaternion rot = Quaternion.Slerp(_startingPos, _targetTransform, turningSpeed * (Time.time - _turningStartTime));
                transform.rotation = rot;
                await Task.Yield();
            }
        }

        public bool IsChairAllignedWith(Quaternion rot)
        {
            Vector3 v1 = transform.rotation * Vector3.forward - rot * Vector3.forward;
            return v1.magnitude < 0.0005;
        }

        public bool IsRotationComplete()
        {
            return !_isRotating;
        }
    }
}
