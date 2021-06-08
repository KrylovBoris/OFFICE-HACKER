using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Скрипт, который отвечает за загрузку 
namespace GlobalMechanics.UI
{
    public class LoadingSceneScript : MonoBehaviour
    {
        public static string SceneToLoad;

        [SerializeField]
        private float _progress;
        public GameObject progressBar;

        private Image[] _bars;

        public void Start()
        {
            _progress = 0;
            _bars = progressBar.GetComponentsInChildren<Image>();
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            var sceneLoaderAsync = SceneManager.LoadSceneAsync(SceneToLoad);
            while (!sceneLoaderAsync.isDone)
            {
                UpdateProgress(sceneLoaderAsync.progress);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator TimeTest()
        {
            while (_progress < 1f)
            {
                _progress += Time.deltaTime;
                UpdateProgress(_progress);
                yield return new WaitForEndOfFrame();
            }
        }

        private void UpdateProgress(float progress)
        {
            var progressPerBar = 1f / _bars.Length;
            var barIndex = (int) (progress / progressPerBar);
            for (int i = 0; i < _bars.Length; i++)
            {
                if (i == barIndex)
                {
                    _bars[i].fillAmount = (progress - barIndex * progressPerBar) * _bars.Length;
                    continue;
                }
                _bars[i].fillAmount = (i < barIndex) ? 1f : 0f;
            }
        }
    }
}
