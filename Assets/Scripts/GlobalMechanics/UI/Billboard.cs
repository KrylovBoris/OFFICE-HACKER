// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace GlobalMechanics.UI
{
    [AddComponentMenu("ProjectFaceless/Tools/Billboard")]
    public class Billboard : MonoBehaviour
    {
    
        Transform cameraTransform;

        [Tooltip("Flip among z axis")]
        public bool flip = false;

        void Start()
        {
            cameraTransform = GameManager.gm.player.transform.Find("Main Camera").transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.forward = flip ? cameraTransform.forward : -cameraTransform.forward;
        }
    }
}
