// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using NPC.Tokens;
using UnityEngine;

namespace NPC
{
    public class Department : MonoBehaviour
    {
        public ArchiveTokenDispenser[] archiveZones;
        [SerializeField]
        private FaxMachineTokenDispenser[] faxMachines;
        public ArchiveTokenDispenser GetRandomArchiveNavMeshDestination()
        {
            return archiveZones[Random.Range(0, archiveZones.Length - 1)];
        }

        public FaxMachineTokenDispenser GetRandomFaxMachineDestination()
        {
            return faxMachines[Random.Range(0, faxMachines.Length - 1)];
        }
    }
}
