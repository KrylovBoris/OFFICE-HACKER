using System;
using System.Collections.Generic;
using System.Linq;
using Agent;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(IWaitingZone))]
    public abstract class TokenDispenser : MonoBehaviour
    {
        [SerializeField] private Transform[] availableSpots;
        public IWaitingZone WaitingZone => _waitingZone;
        private IWaitingZone _waitingZone;

        private Queue<RequestToken> _pendingRequests;
        private Queue<Transform> _availableQueue;
        private Queue<BaseAgent> _waitingAgents;
        

        private void Start()
        {
            _pendingRequests = new Queue<RequestToken>();
            _availableQueue = new Queue<Transform>(availableSpots);
            _waitingAgents = new Queue<BaseAgent>();
            _waitingZone = GetComponent<IWaitingZone>();
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

            if (_waitingAgents.Count > 1)
            {
                //TODO make agents choose friends as dialog targets
                var agent = _waitingAgents.Dequeue();

                var interlocutor1 = new Interlocutor(agent.Head, agent.Personality, agent);
                agent = _waitingAgents.Dequeue();
                var interlocutor2 = new Interlocutor(agent.Head, agent.Personality, agent);
                var conversation = new Conversation(interlocutor1, interlocutor2);
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