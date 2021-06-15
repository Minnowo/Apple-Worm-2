using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitManager : MonoBehaviour
{
    public static OnHitManager Instance = null;
    public AudioSource[] musicSource;

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
        //musicSource = GetComponent<AudioSource>();
        //defaultPitch = musicSource.pitch;

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
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                PlayerControler.Instance.HealPlayer(NotePool.generalDamage);
                break;
            case NoteType.Invincible:
                PlayerControler.Instance.GiveIFrames(PlayerControler.defaultInvincibleTime);
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                break;
            case NoteType.Normal:
                PlayHitSound(trackNumber);
                ShowFlash(trackNumber);
                break;
            case NoteType.Bad:
                PlayHitSound(trackNumber);
                PlayerControler.Instance.TakeDamage(NotePool.generalDamage);
                break;
        }
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
