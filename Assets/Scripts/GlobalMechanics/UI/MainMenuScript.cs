// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlobalMechanics.UI
{
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
}
