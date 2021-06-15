using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
	public void LoadSceneByName(string sceneName)
	{
		Helpers.LoadSceneByName(sceneName);
	}

	public void QuiteGameButton()
	{
		Application.Quit();
	}
}
