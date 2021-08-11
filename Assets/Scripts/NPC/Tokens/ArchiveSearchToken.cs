// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace NPC.Tokens
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
        private ArchiveTokenDispenser _dispenser;

        public ArchiveSearchToken(Transform searchingSpot, SearchArchiveMethod method, ArchiveTokenDispenser dispenser)
        {
            this.SearchingSpot = searchingSpot;
            this.Method = method;
            _dispenser = dispenser;
        }
        
        public override void Finish()
        {
            _dispenser.FreeSpot(SearchingSpot);
            base.Finish();
        }
    }
}