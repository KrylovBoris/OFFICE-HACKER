// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace NPC
{
    public interface ISightController
    {
        Vector3 GetSightTarget(BaseAgent forAgent);
    }
}