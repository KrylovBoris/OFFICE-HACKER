using UnityEngine;

namespace NPC
{
    public class FaxMachineTokenDispenser : TokenDispenser
    {
        public override AiToken RequestToken()
        {
            var spot = GetAvailableSpot();
            if (spot) return this.MakeToken(spot);
        
            return MakeRequestToken();
        }
        
        protected override AiToken MakeToken(Transform spot) =>
            new FaxMachineToken(spot, this);
    }
}