using UnityEngine;

public class Department : MonoBehaviour
{
    public Transform[] archiveNavMeshDestinations;
    public Transform GetRandomArchiveNavMeshDestination()
    {
        return archiveNavMeshDestinations[Random.Range(0, archiveNavMeshDestinations.Length - 1)];
    }
}
