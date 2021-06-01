using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class ProfileDatabase : MonoBehaviour
    {
        public Profile[] profiles;
        private Dictionary<string, Profile> _idToProfile;

        private void Awake()
        {
            _idToProfile = new Dictionary<string, Profile>();
            foreach (var p in profiles)
            {
                _idToProfile.Add(p.logInID, p);
            }
        }

        public string GetLogIn(string profileName)
        {
            for (int i = 0; i < profiles.Length; i++)
            {
                if (profiles[i].personName == profileName)
                {
                    return profiles[i].logInID;
                }
            }

            return "";
        }
    
        public Profile GetProfile(string ID)
        {
            return _idToProfile[ID];
        }

        public bool ContainsProfile(string ID)
        {
            return _idToProfile.ContainsKey(ID);
        }
    
    
    }
}
