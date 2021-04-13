using JetBrains.Collections.Viewable;
using UnityEngine;

namespace NPC
{
    public enum TokenStatus
    {
        Issued,
        InProgress,
        Expired
    }
    
    public abstract class AiToken
    {
        public AiToken()
        {
            Status = TokenStatus.Issued;
        }
        
        public virtual TokenStatus Status { get; private set; }

        public virtual void Accept()
        {
            Status = TokenStatus.InProgress;
        }

        public virtual void Finish()
        {
            Status = TokenStatus.Expired;
        }
    }

    public class RequestToken : AiToken
    {
        private readonly Signal<AiToken> _requestedTokenSignal = new Signal<AiToken>();

        public ISource<AiToken> RequestedTokenSignal => _requestedTokenSignal;

        public override void Accept()
        {
            base.Accept();

            _requestedTokenSignal.AdviseOnce(GameManager.gm.ProjectLifetime, token =>
            {
                Finish();
            });
        }

        public void FulfilRequest(AiToken requestedToken)
        {
            if (requestedToken is RequestToken)
            {
                Debug.LogError("You can't fulfil a request with another request.");
                return;
            }
            _requestedTokenSignal.Fire(requestedToken);
        }
    }
}