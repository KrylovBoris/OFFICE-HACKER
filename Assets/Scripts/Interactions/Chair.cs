using System.Threading.Tasks;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public float turningSpeed = 5.0f;
    private Quaternion _defaultDirection;
    private Quaternion _targetTransform;
    private bool _isRotating = false;

    private void Start()
    {
        _defaultDirection = transform.rotation;
    }

    public void ResetSeat()
    {
        _targetTransform = _defaultDirection;
    }

    public async void Turn(float angle)
    {
        _isRotating = true;
        await TurnByAngle(angle);
        _isRotating = false;
    }

    private async Task TurnByAngle(float angle)
    {
        _targetTransform = Quaternion.AngleAxis(angle, Vector3.up);
        
        while (!IsChairAllignedWith(_targetTransform))
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation, _targetTransform, turningSpeed * Time.deltaTime);
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
