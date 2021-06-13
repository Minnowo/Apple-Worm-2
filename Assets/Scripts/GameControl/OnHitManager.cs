using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitManager : MonoBehaviour
{
    public static OnHitManager Instance = null;
    public AudioSource musicSource;

    public CircleFlash[] circleFlash;
    public CircleIndicator[] circleIndicators;

    private Dictionary<int, ParticleSystem> particleSystems;

    private float defaultPitch;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        defaultPitch = musicSource.pitch;

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

    private void BeatHit(int trackNumber, Rank rank, NoteType t)
    {
        if (rank == Rank.MISS)
        {
            PlayerScoreText.PlayerScore -= NotePool.Instance.spikeDamage;
            return;
        }

        switch (t)
        {
            case NoteType.Heal:
                PlayHitSound();
                ShowFlash(trackNumber);
                PlayerControler.Instance.TakeDamage(-NotePool.Instance.generalDamage);
                break;
            case NoteType.Invincible:
                PlayerControler.Instance.GiveIFrames(5);
                PlayHitSound();
                ShowFlash(trackNumber);
                break;
            case NoteType.Normal:
                PlayHitSound();
                ShowFlash(trackNumber);
                break;
            case NoteType.Bad:
                PlayHitSound();
                PlayerControler.Instance.TakeDamage(NotePool.Instance.generalDamage);
                break;
        }
    }

    private void PlayHitSound()
    {
        musicSource.Play();
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
