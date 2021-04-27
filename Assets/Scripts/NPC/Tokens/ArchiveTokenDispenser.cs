using Agent;

namespace NPC
{
    public class ArchiveTokenDispenser : TokenDispenser<ArchiveSearchToken>
    {
        public override AiToken RequestToken()
        {
            var spot = GetArchiveAvailableSpot();
            if (spot) return ArchiveSearchToken.MakeToken(spot);
        
            return MakeRequestToken();
        }
    }
}