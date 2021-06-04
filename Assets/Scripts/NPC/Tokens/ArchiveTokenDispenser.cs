using Agent;
using UnityEngine;

namespace NPC
{
    public class ArchiveTokenDispenser : TokenDispenser
    {
        public override AiToken RequestToken()
        {
            var spot = GetAvailableSpot();
            if (spot) return MakeToken(spot);
        
            return MakeRequestToken();
        }
        
        protected override AiToken MakeToken(Transform spot) =>
            new ArchiveSearchToken(spot, 
                Random.Range(0, 1) > 0.5f ? SearchArchiveMethod.StandSearch : SearchArchiveMethod.SitSearch, this);
    }
}