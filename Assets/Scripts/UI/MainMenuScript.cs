using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public string firstPlayableScene = "OfficeGame";
    
    public void StartGame()
    {
        LoadingSceneScript.SceneToLoad = firstPlayableScene;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
