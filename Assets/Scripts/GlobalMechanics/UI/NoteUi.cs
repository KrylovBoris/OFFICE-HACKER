using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class NoteUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noteText;

        public void Display(string text)
        {
            noteText.text = text;
        }
    
    }
}
