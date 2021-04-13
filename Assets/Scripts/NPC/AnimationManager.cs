using NPC;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private bool _isAnimatingAction;
    private Animator _animator;

    [SerializeField]
    [Range(0f, 1f)]
    private float _maxIkStrength;
    private Transform _sightDirection;
    

    public bool IsAnimatingAction
    {
        get => _isAnimatingAction;
        set => _isAnimatingAction = value;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void UpdateMoveAnimation(Vector3 movementRelativeDirection)
    {
        var animationVector = transform.InverseTransformDirection(movementRelativeDirection);
        _animator.SetFloat("Vertical", animationVector.z);
        _animator.SetFloat("Horizontal", animationVector.x);
    }

    public void SitDownOnChair()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger("Sit");
    }
    
    public void StandUpFromChair()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger("Stand");
    }

    public void StartTyping()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger("StartTyping");
    }

    public void StopTyping()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger("StopTyping");
    }

    public void StartSearchingFillingCabinet(SearchArchiveMethod searchArchiveMethod)
    {
        _isAnimatingAction = true;
        _animator.SetInteger("Action", searchArchiveMethod == SearchArchiveMethod.SitSearch ? 2 : 3);
    }

    public void StartSendingFax()
    {
        _isAnimatingAction = true;
        _animator.SetInteger("Action", 1);
    }

    public void ForceStop()
    {
        _animator.SetInteger("Emote",0);
        _animator.SetInteger("Action",0);
        _animator.SetTrigger("Stop");
    }

    public void LookAt(Transform lookAtTransform)
    {
        _animator.SetLookAtPosition(lookAtTransform.position);
        _animator.SetLookAtWeight(_maxIkStrength);
    }

}
