using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float range = 2.0f;
    private PickUpSystem _pickUpSystem;
    private Ray _ray;

    private Transform _objectInSight;
    // Start is called before the first frame update
    void Start()
    {
        _pickUpSystem = GetComponent<PickUpSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(_ray, out var hit, range))
            _objectInSight = hit.transform;
        else
        {
            _objectInSight = null;
        }
    }

    public void InteractWithObject()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(_ray, out var hit, range, LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Ignore)) 
            hit.transform.GetComponent<IInteractable>().Interact();
    }

    public void PickUpObject()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(_ray, out var hit, range, LayerMask.GetMask("PickUps"))) _pickUpSystem.PickUp(hit.transform.gameObject);
    }
}
