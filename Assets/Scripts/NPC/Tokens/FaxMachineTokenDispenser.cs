using UnityEngine;

namespace NPC
{
    public class FaxMachineTokenDispenser : TokenDispenser<FaxMachineToken>
    {
        public override AiToken RequestToken()
        {
            var spot = GetArchiveAvailableSpot();
            if (spot) return FaxMachineToken.MakeToken(spot);
        
            return MakeRequestToken();
        }
    }
}