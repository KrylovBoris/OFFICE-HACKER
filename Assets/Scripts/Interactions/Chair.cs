using UnityEngine;

public class Chair : MonoBehaviour
{
    public float turningSpeed = 5.0f;
    private Quaternion _defaultDirection;
    private Quaternion _targetTransform;
    
    private void Start()
    {
        _defaultDirection = transform.rotation;
    }

    private void Update()
    {
        if (!IsRotationComplete())
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation, _targetTransform, turningSpeed * Time.deltaTime);
            transform.rotation = rot;
        }
    }

    public void ResetSeat()
    {
        _targetTransform = _defaultDirection;
    }

    public void Turn(float angle)
    {
        _targetTransform = Quaternion.AngleAxis(angle, Vector3.up);
    }

    public bool IsChairAllignedWith(Quaternion rot)
    {
        Vector3 v1 = transform.rotation * Vector3.forward - rot * Vector3.forward;
        return v1.magnitude < 0.0005;
    }

    
    public bool IsRotationComplete()
    {
        Vector3 v1 = transform.rotation * Vector3.forward - _targetTransform * Vector3.forward;
        return v1.magnitude < 0.0005;
    }
}
