// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace NPC.Tokens
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