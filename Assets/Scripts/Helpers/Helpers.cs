using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class Helpers
{
    public static IEnumerator Delay(float duration)
    {
        yield return new WaitForSeconds(Mathf.Abs(duration));
    }

    public static void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
