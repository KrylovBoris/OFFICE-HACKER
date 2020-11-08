﻿using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    public string switchableObjectName;
    public GameObject switchableObject;
    private string _turnOnDescription = "Turn on";
    private string _turnOffDescription = "Turn off";
    
    private bool _switchFlag;
    // Start is called before the first frame update
    public void Interact()
    {
        if (_switchFlag)
        {
            switchableObject.SetActive(false);
            _switchFlag = false;
        }
        else
        {
            switchableObject.SetActive(true);
            _switchFlag = true;
        }
        
    }

    public string InteractionDescription()
    {
        string result = switchableObjectName;
        if (_switchFlag)
        {
            return _turnOnDescription + switchableObjectName;
        }
        return _turnOffDescription + switchableObjectName;
    }
}