using UnityEngine;
using NPC;

public interface ISightController
{
    Vector3 GetSightTarget(BaseAgent forAgent);
}