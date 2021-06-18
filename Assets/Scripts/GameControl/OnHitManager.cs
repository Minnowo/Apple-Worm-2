using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitManager : MonoBehaviour
{
    public static OnHitManager Instance = null;
    public AudioSource[] musicSource;

    public CircleFlash[] circleFlash;
    public CircleIndicator[] circleIndicators;

    public static float playerHitAccuracy = 0f;
    public static int notesHit = 0;
    public static int extraNotesHit = 0;

    public int currentPerfection = 0;

    private void Awake()
    {
        Instance = this;

        playerHitAccuracy = 0f;
        notesHit = 0;
        extraNotesHit = 0;
        currentPerfection = 0;
    }

    void Start()
    {
        Conductor.beatOnHitEvent += BeatHit;
        
        float xpos = Conductor.Instance.finishLineX;
        float[] yPos = Conductor.Instance.trackSpawnYPos;

        circleIndicators = new CircleIndicator[yPos.Length];
        circleFlash = new CircleFlash[yPos.Length];

        for (int i = 0; i < yPos.Length; i++)
        {
            GameObject circleIndicator = Instantiate<GameObject>(UIPrefabs.CircleIndicatorPrefab);
            SpriteRenderer sr = circleIndicator.GetComponent<SpriteRenderer>();

            sr.sortingLayerName = "Player";
            sr.sortingOrder = 5;

            circleIndicator.transform.parent = this.transform;

            circleIndicator.transform.position = new Vector3(
                xpos,
                yPos[i],
                5);

            circleIndicators[i] = circleIndicator.GetComponent< CircleIndicator>();

            CircleFlash circleFlash = Instantiate<CircleFlash>(UIPrefabs.FlashCirclePrefab);
            this.circleFlash[i] = circleFlash;
        }
    }

    private void OnDestroy()
    {
        Conductor.beatOnHitEvent -= BeatHit;
    }

    private void BeatHit(int trackNumber, Rank rank, NoteType t)
    {
        if (rank == Rank.MISS)
        {
            PlayerScoreText.PlayerScore -= NotePool.spikeDamage;
            return;
        }

        switch (t)
        {
            case NoteType.Heal:
                notesHit++;
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                PlayerControler.Instance.HealPlayer(NotePool.generalDamage);
                UpdateAccuracy(rank);
                break;
            case NoteType.Invincible:
                notesHit++;
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                PlayerControler.Instance.GiveIFrames(PlayerControler.defaultInvincibleTime);
                UpdateAccuracy(rank);
                break;
            case NoteType.Normal:
                notesHit++;
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                UpdateAccuracy(rank);
                break;
            case NoteType.Bad:
                PlayHitSound(trackNumber);
                PlayerControler.Instance.TakeDamage(NotePool.generalDamage);
                break;
        }
    }

    private void UpdateAccuracy(Rank rank)
    {
        switch (rank)
        {
            // since a bonus ranked note is a notetype.Normal
            // we need to reset the number of notes hit to before we hit it
            // and then increase the bonusCounter to avoid the accuracy counter being wrong
            case Rank.BONUS:
                notesHit--;
                extraNotesHit++;
                break;
            case Rank.PERFECT:
                UpdateAccuracy(3);
                break;
            case Rank.GOOD:
                UpdateAccuracy(2);
                break;
            case Rank.BAD:
                UpdateAccuracy(1);
                break;
            case Rank.MISS:
                break;
        }
    }

    private void UpdateAccuracy(int increaseBy)
    {
        currentPerfection += increaseBy;
        playerHitAccuracy = (float)currentPerfection / (float)(Conductor.totalNoteCountWithoutSpikes * 3f) * 100f;
    }

    private void PlayHitSound(int track)
    {
        musicSource[track].Play();
    }

    private void ShowFlash(int trackNumber)
    {
        circleFlash[trackNumber].ShowFlash(
            circleIndicators[trackNumber].transform.position.x,
            circleIndicators[trackNumber].transform.position.y,
            circleIndicators[trackNumber].transform.position.z,
            4);
    }
}
