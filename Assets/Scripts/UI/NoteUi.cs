using TMPro;
using UnityEngine;

public class NoteUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI noteText;

    public void Display(string text)
    {
        noteText.text = text;
    }
    
}
