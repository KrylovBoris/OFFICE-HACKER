// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using FileSystem;
using UnityEngine;
using Directory = FileSystem.Directory;

namespace GlobalMechanics.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Profile", menuName = "ScriptableObjects/Profile", order = 3)]
    public class Profile : ScriptableObject
    {
        public string personName;
        public string logInID;
        public string password;
        public HardDrive workspaceConfig;
    
        private Directory _workSpace;
        private EmailBox _emailbox;

        public void ChangePassword(string newPassword)
        {
            password = newPassword;
        }
    
        public Directory WorkSpace => _workSpace ??= workspaceConfig.Catalogue;

        public EmailBox EmailBox
        {
            get
            {
                if (_emailbox == null)
                {
                    GameManager.gm.Commutator.EstablishConnection(logInID, out _emailbox);
                }

                return _emailbox;
            }
        }
    }
}
