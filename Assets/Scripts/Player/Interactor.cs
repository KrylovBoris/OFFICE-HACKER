using GlobalMechanics.UI;
using UnityEngine;

namespace Player
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform pointer;
        [SerializeField] private float range = 2.0f;
        private PickUpSystem _pickUpSystem;
        private Ray _ray;
        private Camera _camera;

        private Transform _objectInSight;
        // Start is called before the first frame update
        void Start()
        {
            _pickUpSystem = GetComponent<PickUpSystem>();
            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(_ray, out var hit, range))
            {
                _objectInSight = hit.transform;
                pointer.position = hit.point - _ray.direction * 0.1f;
            }
            else
            {
                _objectInSight = null;
                pointer.position = _ray.GetPoint(range);
            }
        }

        public void InteractWithObject()
        {
            _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(_ray, out var hit, range, LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Ignore)) 
                hit.transform.GetComponent<IInteractable>().Interact();
        }

        public void PickUpObject()
        {
            _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(_ray, out var hit, range, LayerMask.GetMask("PickUps"))) _pickUpSystem.PickUp(hit.transform.gameObject);
        }
    }
}
