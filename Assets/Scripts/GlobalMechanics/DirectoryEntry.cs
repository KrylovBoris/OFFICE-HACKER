using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirectoryEntry : MonoBehaviour, IPointerClickHandler
{

    [Tooltip("icon that displays directory")]
    [SerializeField]
    private Image icon;
    public Image Icon => icon;
    public TMP_InputField directoryName;
    public GameFileSystem fileSystem;
    public GameObject toolPanel;
    private string _fullPath;
    public string Path => _fullPath;
    private bool _isPathSet = false;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var tools = Instantiate(toolPanel, eventData.position, fileSystem.transform.rotation, fileSystem.transform);
            fileSystem.ConnectExplorerTools(tools);
            tools.GetComponent<ExplorerTools>().EstablishConnection(fileSystem, this);
        }
        else
        {
            fileSystem.DisconnectExplorerTools();
        }
    }

    public void SetPath(string path, Sprite fileIcon, GameFileSystem gameFileSystem)
    {
        if (!_isPathSet)
        {
            SetControllingSystem(gameFileSystem);
            _fullPath = path;
            directoryName.text = path.Substring(path.LastIndexOf('\\') + 1);
            _isPathSet = true;
            Icon.sprite = fileIcon;
        }
    }

    public void SetControllingSystem(GameFileSystem system)
    {
        fileSystem = system;
    }
    
    public void Open()
    {
        fileSystem.OpenDirectory(_fullPath);
    }

    public void StartRenaming()
    {
        var text = directoryName.text;
        directoryName.interactable = true;
        directoryName.text = text;
    }

    public void FinishRenaming()
    {
        //TODO Rename
    }
    
    
}
