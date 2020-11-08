using UnityEngine;
using Directory = FileSystemTree.Directory;

[CreateAssetMenu(fileName = "Profile", menuName = "ScriptableObjects/Profile", order = 3)]
public class Profile : ScriptableObject
{
    public string personName;
    public string logInID;
    public string password;
    public TextAsset workspaceConfig;
    
    private FileSystemTree.Directory workSpace;
    public void ChangePassword(string newPassword)
    {
        password = newPassword;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Directory WorkSpace
    {
        get
        {
            if (workSpace == null)
            {
                workSpace = GameFileSystem.ConsturctDirectoriesFromJson(workspaceConfig);
            }
            return workSpace;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
