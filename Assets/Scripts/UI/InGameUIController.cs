using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{

    public const string HomeScene = "MainMenu";
    public float DelayBetweenElements = 0.75f;
    public float NumberCountDelay = 0.001f;
    public bool SongPassed = false;

    public GameObject victoryScreen;
    public GameObject homeButton;
    public GameObject restartButton;

    public GameObject pauseScene;
    public GameObject player;
    public GameObject comboText;
    public GameObject healthBar;

    public LevelCompleteDisplayBubble accuracyDisplay;
    public LevelCompleteDisplayBubble comboDisplay;
    public LevelCompleteDisplayBubble bonusDisplay;

    public LevelCompleteDisplayBubble songTitleText;
    public LevelCompleteDisplayBubble victoryStatusText;
    public LevelCompleteDisplayBubble scoreText;

    private bool disablePause = false;
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
        disablePause = true;
        SongPassed = victory;
        StartCoroutine(ShowSongFinishedScreen());
    }

    public void HomeButtonPressed()
    {
        SceneManager.LoadSceneAsync(HomeScene);
    }

    public void PauseButtonPressed()
    {
        if (Conductor.countingDown || disablePause)
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
        comboDisplay.gameObject.SetActive(false);
        accuracyDisplay.gameObject.SetActive(false);
        bonusDisplay.gameObject.SetActive(false);
        homeButton.SetActive(false);
        restartButton.SetActive(false);

        scoreText.value.text = string.Format("Score: {0}", PlayerScoreText.PlayerScore);
        songTitleText.value.text = Conductor.Instance.songInfo.SongName;
        victoryStatusText.value.text = string.Format("Status: {0}", SongPassed.FormatBool("Victory", "Failed"));

        victoryScreen.SetActive(true);

        yield return new WaitForSeconds(DelayBetweenElements);

        comboDisplay.gameObject.SetActive(true);

        for(int i = 0; i < Conductor.Combo + 1; i += 3)
        {
            comboDisplay.value.text = i.Clamp(0, Conductor.Combo).ToString();
            yield return new WaitForSeconds(NumberCountDelay);
        }

        comboDisplay.value.text = Conductor.Combo.ToString();

        yield return new WaitForSeconds(DelayBetweenElements);

        accuracyDisplay.gameObject.SetActive(true);

        for (int i = 0; i < OnHitManager.playerHitAccuracy.Clamp(0, 100) + 1; i += 3)
        {
            accuracyDisplay.value.text = string.Format("{0:#.00}%", ((float)i).Clamp(0, OnHitManager.playerHitAccuracy));
            yield return new WaitForSeconds(NumberCountDelay);
        }
        accuracyDisplay.value.text = string.Format("{0:#.00}%", OnHitManager.playerHitAccuracy.Clamp(0, 100));

        yield return new WaitForSeconds(DelayBetweenElements);

        bonusDisplay.gameObject.SetActive(true);

        for (int i = 0; i < OnHitManager.extraNotesHit + 1; i += 3)
        {
            bonusDisplay.value.text = i.Clamp(0, OnHitManager.extraNotesHit).ToString();
            yield return new WaitForSeconds(NumberCountDelay);
        }

        bonusDisplay.value.text = OnHitManager.extraNotesHit.ToString();

        yield return new WaitForSeconds(DelayBetweenElements);

        homeButton.SetActive(true);
        restartButton.SetActive(true);

        disablePause = false;
    }
}
