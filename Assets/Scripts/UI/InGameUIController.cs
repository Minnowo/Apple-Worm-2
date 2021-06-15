using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{

    public const string HomeScene = "MainMenu";
    public float DelayBetweenElements = 0.75f;

    public GameObject victoryScreen;

    public GameObject pauseScene;
    public GameObject player;
    public GameObject comboText;
    public GameObject healthBar;

    public LevelCompleteDisplayBubble accuracyDisplay;
    public LevelCompleteDisplayBubble comboDisplay;
    public LevelCompleteDisplayBubble bonusDisplay;


    private void Start()
    {
        Conductor.songCompletedEvent += Conductor_songCompletedEvent;
    }

    private void OnDestroy()
    {
        Conductor.songCompletedEvent -= Conductor_songCompletedEvent;
    }

    private void Conductor_songCompletedEvent(bool victory)
    {
        StartCoroutine(ShowSongFinishedScreen());
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


    private IEnumerator ShowSongFinishedScreen()
    {
        player.SetActive(false);
        comboText.SetActive(false);
        healthBar.SetActive(false);
        PlayerScoreText.Instance.gameObject.SetActive(false);
        SongTimeBar.Instance.gameObject.SetActive(false);

        /*comboDisplay.value.text = Conductor.Combo.ToString();
        accuracyDisplay.value.text = OnHitManager.playerHitAccuracy.ToString();
        bonusDisplay.value.text = OnHitManager.extraNotesHit.ToString();*/

        victoryScreen.SetActive(true);

        yield return new WaitForSeconds(DelayBetweenElements);

        comboDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(DelayBetweenElements);

        accuracyDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(DelayBetweenElements);

        bonusDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(DelayBetweenElements);


    }
}
