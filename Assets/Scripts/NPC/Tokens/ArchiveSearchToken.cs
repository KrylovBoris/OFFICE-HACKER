using UnityEngine;

namespace NPC
{
    public enum SearchArchiveMethod
    {
        StandSearch,
        SitSearch
    }
    
    public class ArchiveSearchToken : AiToken
    {
        public readonly SearchArchiveMethod Method;
        public readonly Transform SearchingSpot; 
        
        public ArchiveSearchToken(Transform searchingSpot, SearchArchiveMethod method)
        {
            this.SearchingSpot = searchingSpot;
            this.Method = method;
        }

        public static ArchiveSearchToken MakeToken(Transform spot) =>
            new ArchiveSearchToken(spot, 
                Random.Range(0, 1) > 0.5f ? SearchArchiveMethod.StandSearch : SearchArchiveMethod.SitSearch);
        
    }
}