using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    public bool Finished => _finished;
    private bool _finished = false;
    
    public string mission;
    public string file;
    public int size;
    public string description;
    public uint payment;
    
    public TextMeshProUGUI missionName;
    public TextMeshProUGUI fileName;
    public TextMeshProUGUI fileSize;
    public TextMeshProUGUI descriptionText;
    public Button submitFileButton;
    
    // Start is called before the first frame update
    void Start()
    {
        missionName.text = mission + ((payment > 0) ? ". Reward: " + payment +"$" : "");
        fileName.text = file;
        fileSize.text = size + " Mb";
        descriptionText.text = description;
        BlockButton();
    }

    public void BlockButton()
    {
        submitFileButton.interactable = false;
    }
    
    public void UnBlockButton()
    {
        _finished = true;
        submitFileButton.interactable = true;
    }

    public void ObjectiveComplete()
    {
        GameManager.gm.player.GetComponent<PlayerInventory>().GetPaid(payment);
        this.gameObject.SetActive(false);
    }
    
    
}
