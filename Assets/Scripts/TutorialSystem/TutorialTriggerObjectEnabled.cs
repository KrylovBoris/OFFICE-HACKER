using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerObjectEnabled : MonoBehaviour
{
    private TutorialSystem system;

    void Start()
    {
        system = GameObject.Find("TutorialObjects").GetComponent<TutorialSystem>();
        system.Triggered();
    }
}
