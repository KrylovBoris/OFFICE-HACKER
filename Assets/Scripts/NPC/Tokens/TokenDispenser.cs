using System.Collections.Generic;
using System.Linq;
using NPC.Conversations;
using UnityEngine;

namespace NPC.Tokens
{
    [RequireComponent(typeof(IWaitingZone))]
    public abstract class TokenDispenser : MonoBehaviour
    {
        [SerializeField] private int tokenCount;
        [SerializeField] private Transform[] availableSpots;
        public IWaitingZone WaitingZone => _waitingZone;
        private IWaitingZone _waitingZone;

        private int _availableTokens;
        private Queue<RequestToken> _pendingRequests;
        private Queue<Transform> _availableQueue;
        

        private void Start()
        {
            _availableTokens = tokenCount;
            _pendingRequests = new Queue<RequestToken>();
            _availableQueue = new Queue<Transform>(availableSpots);
            _waitingZone = GetComponent<IWaitingZone>();
        }

        private void Update()
        {
            if (_pendingRequests.Any())
            {
                var spot = GetAvailableSpot();
                if (spot)
                {
                    var request = _pendingRequests.Dequeue();
                    var token = MakeToken(spot);
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

        protected abstract AiToken MakeToken(Transform spot);

        public bool HasAvailableSpots() => _availableTokens > 0 && _availableQueue.Any();

        public void FreeSpot(Transform token)
        {
            _availableTokens++;
            _availableQueue.Enqueue(token);
        }

        protected Transform GetAvailableSpot()
        {
            if (_availableTokens == 0) return null;
            if (_availableQueue.Any())
            {
                _availableTokens--;
                return _availableQueue.Dequeue();
            }

            return null;
        }
    }
}