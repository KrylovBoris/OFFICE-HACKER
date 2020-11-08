using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private float pickUpLat = 1.0f;
        private Vector3 _inputDirection;
        private MovementScript _movementScript;
        private Interactor _interactor;
        private PickUpSystem _pickUpSystem;
        private CameraScript _cameraScript;
        private PasswordPeek _passwordPeek;
        private float _pickUpStartTime;

        private bool _controlsLocked = false;
        public bool ControlsLocked => _controlsLocked;

        // Start is called before the first frame update
        void Start()
        {
            _pickUpSystem = GetComponent<PickUpSystem>();
            _interactor = GetComponent<Interactor>();
            _passwordPeek = GetComponent<PasswordPeek>();
            _movementScript = GetComponent<MovementScript>();
            _cameraScript = GetComponent<CameraScript>();
            _cameraScript.LockCursor();

        }

        // Update is called once per frame
        void Update()
        {
            if (!_controlsLocked)
            {
                _inputDirection = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0,
                    CrossPlatformInputManager.GetAxisRaw("Vertical"));
                if (CrossPlatformInputManager.GetButtonDown("Sprint"))
                {
                    _movementScript.IsSprinting = true;
                }

                if (CrossPlatformInputManager.GetButtonUp("Sprint"))
                {
                    _movementScript.IsSprinting = false;
                }

                if (!GameManager.gm.IsPaused)
                    _cameraScript.RotateCamera(new Vector2(CrossPlatformInputManager.GetAxisRaw("Mouse X"),
                        CrossPlatformInputManager.GetAxisRaw("Mouse Y")));
                _movementScript.MoveInDirection(_inputDirection);

                if (CrossPlatformInputManager.GetButton("PickUp")) _pickUpStartTime += Time.deltaTime;

                if (_pickUpStartTime > pickUpLat) _interactor.PickUpObject();

                if (CrossPlatformInputManager.GetButtonUp("PickUp")) _pickUpStartTime = 0;

                if (CrossPlatformInputManager.GetButtonDown("PickUp") && _pickUpSystem.IsCarryingSmomething)
                    _pickUpSystem.Drop();

                if (CrossPlatformInputManager.GetButtonDown("Action"))
                {
                    if (_passwordPeek.CanPeek)
                    {
                        _passwordPeek.ActivatePeeking();
                    }
                }

                if (CrossPlatformInputManager.GetButtonUp("Action"))
                {
                    _passwordPeek.DeactivatePeeking();
                }

                if (CrossPlatformInputManager.GetButtonDown("Interact"))
                {
                    _interactor.InteractWithObject();
                }
            }


            if (CrossPlatformInputManager.GetButtonDown("Smartphone"))
            {
                if (GameManager.gm.IsLookingAtSmartPhone)
                {
                    GameManager.gm.CloseSmartphone();
                }
                else
                {
                    GameManager.gm.OpenSmartphone();
                }
            }

            if (CrossPlatformInputManager.GetButtonDown("Cancel")) GameManager.gm.Escape();


        }

        public void LockControls()
        {
            _controlsLocked = true;
        }

        public void UnLockControls()
        {
            _controlsLocked = false;
        }
    }
}

