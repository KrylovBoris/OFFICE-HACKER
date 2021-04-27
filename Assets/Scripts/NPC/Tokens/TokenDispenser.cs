using System;
using System.Collections.Generic;
using System.Linq;
using Agent;
using UnityEngine;

namespace NPC
{
    public abstract class TokenDispenser<T> : MonoBehaviour
        where T : AiToken 
    {
        [SerializeField] private Transform[] availableSpots;
        [SerializeField] private float waitingZoneRadius;

        public Transform WaitingZone => this.transform;

        public float WaitingZoneRadius => waitingZoneRadius;

        private Queue<RequestToken> _pendingRequests;
        private Queue<Transform> _availableQueue;
        private Queue<BaseAgent> _waitingAgents;

        private void Start()
        {
            _pendingRequests = new Queue<RequestToken>();
            _availableQueue = new Queue<Transform>(availableSpots);
            _waitingAgents = new Queue<BaseAgent>();
        }

        private void Update()
        {
            if (_pendingRequests.Any())
            {
                var spot = GetArchiveAvailableSpot();
                if (spot)
                {
                    var request = _pendingRequests.Dequeue();
                    var token = ArchiveSearchToken.MakeToken(spot);
                    request.FulfilRequest(token);
                }
            }
        }

        public abstract AiToken RequestToken();

        protected RequestToken MakeRequestToken()
        {
            var requestToken = new RequestToken();
            _pendingRequests.Enqueue(requestToken);
            return requestToken;
        }

        public bool HasAvailableSpots() => _availableQueue.Any();

        public void FreeSpot(Transform token)
        {
            _availableQueue.Enqueue(token);
        }

        protected Transform GetArchiveAvailableSpot()
        {
            return !_availableQueue.Any() ? null : _availableQueue.Dequeue();
        }
        
        public void PlaceWaitingAgent(BaseAgent baseAgent)
        {
            _waitingAgents.Enqueue(baseAgent);
        }
    }
}