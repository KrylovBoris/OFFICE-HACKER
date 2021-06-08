using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private TutorialStage[] stages;
        private int _stagesindex;
        private int _stageslength;
        private GameObject _goHint;
        private String _endtext = "Завершите уровень";


        [Serializable]
        public class TutorialStage
        {
            public string text;
            public TriggerType trigger;
            public GameObject target;
            public void AddTriggerTypeScript()
            {
                switch (trigger)
                {
                    case TriggerType.CameraLook:
                        target.AddComponent<TutorialTriggerCameraLook>();
                        break;
                    case TriggerType.ZoneEnter:
                        target.AddComponent<TutorialTriggerZoneEnter>();
                        break;
                    case TriggerType.ObjectEnabled:
                        target.AddComponent<TutorialTriggerObjectEnabled>();
                        break;/*
                case TriggerType.MessageOpened:
                    break;
                case TriggerType.ShopItemPurchased:
                    break;*/
                }
            }
        }

        [Serializable]
        public enum TriggerType
        {
            CameraLook,
            ZoneEnter,
            ObjectEnabled/*,
        MessageOpened,
        ShopItemPurchased*/
        }

        void Start()
        {
            _stagesindex = 0;
            _stageslength = stages.Length;
            _goHint = GameObject.FindGameObjectWithTag("Hint");
            if (stages[_stagesindex] != null)
                Invoke("PrepareStage", 2);
        }

        public void Triggered()
        {
            GameObject.Find("AdvicePanel").GetComponent<Image>().color = new Color(11/256f, 147/256f, 0);
            if (stages[_stagesindex].trigger == TriggerType.ObjectEnabled)
            {
                Destroy(stages[_stagesindex].target.GetComponent<TutorialTriggerObjectEnabled>());
            }
            else
            {
                Destroy(stages[_stagesindex].target);
            }
            _stagesindex++;
            Invoke("PrepareStage", 2);
        }
    
        public void PrepareStage()
        {
            GameObject.Find("AdvicePanel").GetComponent<Image>().color = new Color(120/256f, 120/256f, 120/256f);
            if (_stagesindex < _stageslength)
            {
                _goHint.GetComponent<TextMeshProUGUI>().text = stages[_stagesindex].text;
                if (stages[_stagesindex].trigger == TriggerType.ObjectEnabled)
                {
                    stages[_stagesindex].target.SetActive(false);
                }
                else
                {
                    stages[_stagesindex].target.SetActive(true);
                }
                stages[_stagesindex].AddTriggerTypeScript();
            }
            else
            {
                _goHint.GetComponent<TextMeshProUGUI>().text = _endtext;
            }
        }
    }
}
