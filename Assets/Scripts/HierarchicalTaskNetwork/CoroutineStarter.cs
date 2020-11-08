using System.Collections;
using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{
    public void StartRunningCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
