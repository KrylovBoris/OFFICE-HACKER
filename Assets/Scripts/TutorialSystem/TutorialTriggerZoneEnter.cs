using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerZoneEnter : MonoBehaviour
{
    private TutorialSystem system;

    void Start()
    {
        system = transform.GetComponentInParent<TutorialSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            system.Triggered();
    }

}
