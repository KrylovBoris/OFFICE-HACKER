// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using GlobalMechanics;
using Interactions;
using UnityEngine;

namespace Player
{
    public class PasswordPeekingActivator : MonoBehaviour
    {
        private float _passwordTypingStart;
        private PasswordPeek _player;
        private WorkPlace _workPlace;

        private void Start()
        {
            _player = GameManager.gm.player.GetComponent<PasswordPeek>();
            _workPlace = GetComponentInParent<WorkPlace>();
        }
    
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && _workPlace.IsVictimVulnerable)
            {
                _player = other.gameObject.GetComponent<PasswordPeek>();
                _player.MakePeekingPossible(_workPlace.WorkerOnSite, _workPlace.PasswordTypingStart);
            }

            if (!_workPlace.IsVictimVulnerable && _player.IsPeeking)
            {
                DisablePeeking();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && _workPlace.IsVictimVulnerable)
            {
                DisablePeeking();
            }
        }

        private void DisablePeeking()
        {
            _player.DeactivatePeeking();
            _player.MakePeekingImpossible();
        }
    }
}
