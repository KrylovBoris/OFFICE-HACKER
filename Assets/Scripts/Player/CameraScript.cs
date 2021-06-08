using UnityEngine;

namespace Player
{
    public class CameraScript : MonoBehaviour
    {
        [SerializeField] private float sensetivityX = 5.0f;
        [SerializeField] private float sensetivityY = 5.0f;
        [SerializeField] private float maxAngle = 85.0f;
        [SerializeField] private float minAngle = -85.0f;
        [SerializeField] private Transform FPSCamera;
        private float _maxAllowedAngle, _minAllowedAngle;
        private Transform ComputerCamera;

        private bool _cameraLocked;
        public bool CameraLocked => _cameraLocked;

        private Quaternion tempRotation;
        // Start is called before the first frame update
        void Start()
        {
            _maxAllowedAngle = FPSCamera.transform.localRotation.eulerAngles.x + maxAngle;
            _minAllowedAngle = FPSCamera.transform.localRotation.eulerAngles.x + minAngle;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void RotateCamera(Vector2 input)
        {
            tempRotation = FPSCamera.localRotation * Quaternion.AngleAxis(-input.y, Vector3.right);
            var angle = tempRotation.eulerAngles.x + ((tempRotation.eulerAngles.x > 180) ? -360 : 0);
            if (angle < _maxAllowedAngle && angle > _minAllowedAngle)
            {
                FPSCamera.localRotation = tempRotation;
            }
            else
            {
                Debug.Log("Nope");
            }

            transform.localRotation *= Quaternion.AngleAxis(input.x, Vector3.up);
        }

        public void LockCamera()
        {
            _cameraLocked = true;
        }
        public void UnLockCamera()
        {
            _cameraLocked = false;
        }
    }
}
