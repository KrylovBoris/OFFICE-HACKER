// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using JetBrains.Collections.Viewable;
using NPC.Tokens;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    public class AnimationManager : MonoBehaviour
    {
        private bool _isAnimatingAction;
        private Animator _animator;

        [SerializeField]
        [Range(0f, 1f)]
        private float maxIkStrength;
        private ISightController _sightController;
        private BaseAgent _agent;
        private bool _isLookingAtInterlocutor = false;
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Sit = Animator.StringToHash("Sit");
        private static readonly int Stand = Animator.StringToHash("Stand");
        private static readonly int Typing = Animator.StringToHash("StartTyping");
        private static readonly int StopTyping1 = Animator.StringToHash("StopTyping");
        private static readonly int Action = Animator.StringToHash("Action");
        private static readonly int EmoteHash = Animator.StringToHash("Emote");
        private static readonly int SpeakHash = Animator.StringToHash("Talking");
        private static readonly int Stop = Animator.StringToHash("Stop");
        private static readonly int StopTalking1 = Animator.StringToHash("StopTalking");

        public bool IsAnimatingAction
        {
            get => _isAnimatingAction;
            set => _isAnimatingAction = value;
        }
    
        public ViewableProperty<bool> IsSpeaking { get; } = new ViewableProperty<bool>();

        void Start()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<BaseAgent>();
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
                _animator.SetLookAtPosition(_sightController.GetSightTarget(_agent));
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

        public void LookAt(ISightController lookAt)
        {
            _isLookingAtInterlocutor = true;
            _sightController = lookAt;
        }

        public void EmoteAny()
        {
            _isAnimatingAction = true;
            _animator.SetInteger(EmoteHash, Random.Range(1, 18));
        }

        public void SayLine()
        {
            IsSpeaking.Value = true;
            _animator.SetInteger(SpeakHash, Random.Range(1, 4));
        }

        public void ResetHead()
        {
            _isLookingAtInterlocutor = false;
            _sightController = null;
        }

        public void StopSpeaking()
        {
            IsSpeaking.Value = false;
        }

        public void StopTalking()
        {
            _animator.SetTrigger(StopTalking1);
        }
    }
}