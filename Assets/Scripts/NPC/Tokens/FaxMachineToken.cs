using UnityEngine;

namespace NPC
{
    public class FaxMachineToken : AiToken
    {
        public readonly Transform SearchingSpot; 
        
        public FaxMachineToken(Transform searchingSpot)
        {
            this.SearchingSpot = searchingSpot;
        }

        public static FaxMachineToken MakeToken(Transform spot) =>
            new FaxMachineToken(spot);

    }
}