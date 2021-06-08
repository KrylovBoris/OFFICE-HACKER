using UnityEngine;

namespace NPC
{
    public interface ISightController
    {
        Vector3 GetSightTarget(BaseAgent forAgent);
    }
}