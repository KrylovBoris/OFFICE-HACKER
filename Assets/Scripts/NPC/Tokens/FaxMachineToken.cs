using UnityEngine;

namespace NPC
{
    public class FaxMachineToken : AiToken
    {
        private readonly FaxMachineTokenDispenser _dispenser;
        public readonly Transform SearchingSpot; 
        
        public FaxMachineToken(Transform searchingSpot, FaxMachineTokenDispenser issuer)
        {
            this.SearchingSpot = searchingSpot;
            _dispenser = issuer;
        }

        public override void Finish()
        {
            _dispenser.FreeSpot(SearchingSpot);
            base.Finish();
        }

    }
}