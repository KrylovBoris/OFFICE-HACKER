using System;
using System.Collections.Generic;
using NPC;
using UnityEngine;

namespace Player
{
    public class PlayerKnowledge
    {
        private string _name;
        private string _knownPassword;
        private string _knownLogin;

        public string Name => _name;
        public string Password => _knownPassword;
        public string Login => _knownLogin;
        private Dictionary<PersonalityTrait.TraitType, float> _knownTraitsAndTheirIntensity;
        //TODO: Fact container;
    
        // Start is called before the first frame update
        public PlayerKnowledge(string newName)
        {
            _name = newName;
            _knownLogin = string.Empty;
            _knownPassword = string.Empty;
            _knownTraitsAndTheirIntensity = new Dictionary<PersonalityTrait.TraitType, float>();
        }

        public void LearnNewPassword(string learntPassword)
        {
            _knownPassword = learntPassword;
        }

        public void LearnLogin(string learntLogin)
        {
            _knownLogin = learntLogin;
        }
    
        public void LearnPersonalityTrait(PersonalityTrait.TraitType trait)
        {
            _knownTraitsAndTheirIntensity.Add(trait, float.PositiveInfinity);
        }

        public void LearnTraitIntensity(PersonalityTrait.TraitType trait, float intensity)
        {
            if (_knownTraitsAndTheirIntensity.ContainsKey(trait))
            {
                _knownTraitsAndTheirIntensity[trait] = intensity;
            }
        }

        public static PlayerKnowledge operator +(PlayerKnowledge k1, PlayerKnowledge k2)
        {
        
            Debug.Assert(k1.Name == k2.Name);
            var k3 = new PlayerKnowledge(k1.Name);
            if (k1._knownLogin != String.Empty)
            {
                k3._knownLogin = k1._knownLogin;
            }
            else
            {
                k3._knownLogin = k2._knownLogin;
            }
        
            if (k2._knownPassword != String.Empty)
            {
                k3._knownPassword = k2._knownPassword;
            }
            else
            {
                k3._knownPassword = k1._knownPassword;
            }

            foreach (var key in k1._knownTraitsAndTheirIntensity.Keys)
            {
                k3._knownTraitsAndTheirIntensity.Add(key, k1._knownTraitsAndTheirIntensity[key]);
            }
        
            foreach (var key in k2._knownTraitsAndTheirIntensity.Keys)
            {
                if (k3._knownTraitsAndTheirIntensity.ContainsKey(key))
                    k3._knownTraitsAndTheirIntensity.Add(key, k2._knownTraitsAndTheirIntensity[key]);
            }

            return k3;
        }


        //TODO: переписать логику таким образом, чтобы оператор проверял содержит ли k2 новые данные по отношению к k1
        public static bool operator ==(PlayerKnowledge k1, PlayerKnowledge k2)
        {
            Debug.Assert(k1.Name == k2.Name);
            var result = true;
            result &= k1._knownLogin == k2._knownLogin;
            result &= k1._knownPassword == k2._knownPassword;
            foreach (var key in k2._knownTraitsAndTheirIntensity.Keys)
            {
                result &= k1._knownTraitsAndTheirIntensity.ContainsKey(key);
            }
        
            return result;
        }

        public static bool operator !=(PlayerKnowledge k1, PlayerKnowledge k2)
        {
            return !(k1 == k2);
        }
    }
}
