using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{

    public const string HomeScene = "MainMenu";

    public GameObject pauseScene;

    private void Start()
    {
        Conductor.songCompletedEvent += Conductor_songCompletedEvent;
    }

    private void OnDestroy()
    {
        Conductor.songCompletedEvent -= Conductor_songCompletedEvent;
    }

    private void Conductor_songCompletedEvent()
    {
        
    }

    public void HomeButtonPressed()
    {
        SceneManager.LoadSceneAsync(HomeScene);
    }

    public void PauseButtonPressed()
    {
        if (Conductor.countingDown)
            return;

        if (Conductor.Paused)
        {
            pauseScene.SetActive(false);

            Conductor.Paused = false;
            return;
        }

        pauseScene.SetActive(true);

        Conductor.Paused = true;
    }

    public void PauseButtonPressed(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;
        PauseButtonPressed();
    }

    public void RestartButtonPressed()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
