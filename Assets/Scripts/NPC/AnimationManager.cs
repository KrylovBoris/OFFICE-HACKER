using System;
using NPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationManager : MonoBehaviour
{
    private bool _isAnimatingAction;
    private Animator _animator;

    [SerializeField]
    [Range(0f, 1f)]
    private float maxIkStrength;
    private Transform _sightDirection;
    private bool _isLookingAtInterlocutor = false;
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Sit = Animator.StringToHash("Sit");
    private static readonly int Stand = Animator.StringToHash("Stand");
    private static readonly int Typing = Animator.StringToHash("StartTyping");
    private static readonly int StopTyping1 = Animator.StringToHash("StopTyping");
    private static readonly int Action = Animator.StringToHash("Action");
    private static readonly int EmoteHash = Animator.StringToHash("Emote");
    private static readonly int Stop = Animator.StringToHash("Stop");

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
        _animator.SetFloat(Vertical, animationVector.z);
        _animator.SetFloat(Horizontal, animationVector.x);
    }

    public void OnAnimatorIK(int layerIndex)
    {
        if (_isLookingAtInterlocutor)
        {
            _animator.SetLookAtPosition(_sightDirection.position);
            _animator.SetLookAtWeight(maxIkStrength);
        }
    }

    public void SitDownOnChair()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger(Sit);
    }
    
    public void StandUpFromChair()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger(Stand);
    }

    public void StartTyping()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger(Typing);
    }

    public void StopTyping()
    {
        _isAnimatingAction = true;
        _animator.SetTrigger(StopTyping1);
    }

    public void StartSearchingFillingCabinet(SearchArchiveMethod searchArchiveMethod)
    {
        _isAnimatingAction = true;
        _animator.SetInteger(Action, searchArchiveMethod == SearchArchiveMethod.SitSearch ? 2 : 3);
    }

    public void StartSendingFax()
    {
        _isAnimatingAction = true;
        _animator.SetInteger(Action, 1);
    }

    public void ForceStop()
    {
        _animator.SetInteger(EmoteHash,0);
        _animator.SetInteger(Action,0);
        _animator.SetTrigger(Stop);
    }

    public void LookAt(Transform lookAtTransform)
    {
        _isLookingAtInterlocutor = true;
        _sightDirection = lookAtTransform;
    }

    public void EmoteAny()
    {
        _isAnimatingAction = true;
        _animator.SetInteger(EmoteHash, Random.Range(1, 18));
    }

    public void SayLine()
    {
        _isAnimatingAction = true;
        _animator.SetInteger(EmoteHash, Random.Range(1, 4));
    }

    public void ResetHead()
    {
        _isLookingAtInterlocutor = false;
        _sightDirection = null;
    }
}
