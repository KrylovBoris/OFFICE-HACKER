using GlobalMechanics;
using UnityEngine;

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
                _inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                    Input.GetAxisRaw("Vertical"));
                if (Input.GetButtonDown("Sprint"))
                {
                    _movementScript.IsSprinting = true;
                }

                if (Input.GetButtonUp("Sprint"))
                {
                    _movementScript.IsSprinting = false;
                }

                if (!GameManager.gm.IsPaused)
                    _cameraScript.RotateCamera(new Vector2(Input.GetAxisRaw("Mouse X"),
                        Input.GetAxisRaw("Mouse Y")));
                _movementScript.MoveInDirection(_inputDirection);

                if (Input.GetButton("PickUp")) _pickUpStartTime += Time.deltaTime;

                if (_pickUpStartTime > pickUpLat) _interactor.PickUpObject();

                if (Input.GetButtonUp("PickUp")) _pickUpStartTime = 0;

                if (Input.GetButtonDown("PickUp") && _pickUpSystem.IsCarryingSmomething)
                    _pickUpSystem.Drop();

                if (Input.GetButtonDown("Action"))
                {
                    if (_passwordPeek.CanPeek)
                    {
                        _passwordPeek.ActivatePeeking();
                    }
                }

                if (Input.GetButtonUp("Action"))
                {
                    _passwordPeek.DeactivatePeeking();
                }

                if (Input.GetButtonDown("Interact"))
                {
                    _interactor.InteractWithObject();
                }
            }


            if (Input.GetButtonDown("Smartphone"))
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

            if (Input.GetButtonDown("Cancel")) GameManager.gm.Escape();


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

