using UnityEngine;

namespace TutorialSystem
{
    public class TutorialTriggerObjectEnabled : MonoBehaviour
    {
        private TutorialSystem system;

        void Start()
        {
            system = GameObject.Find("TutorialObjects").GetComponent<TutorialSystem>();
            system.Triggered();
        }
    }
}
