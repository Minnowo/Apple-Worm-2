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

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        musicSource = GetComponent<AudioSource>();

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
            PlayerControler.Instance.TakeDamage(NotePool.Instance.damage);
            return;
        }

        musicSource.Play();

        circleFlash[trackNumber].ShowFlash(
            circleIndicators[trackNumber].transform.position.x, 
            circleIndicators[trackNumber].transform.position.y, 
            circleIndicators[trackNumber].transform.position.z, 
            4);

        switch (t)
        {
            case NoteType.Heal:
                print("healed");
                PlayerControler.Instance.TakeDamage(-NotePool.Instance.damage);
                break;
            case NoteType.Invincible:
                break;
            case NoteType.Normal:
                break;
            case NoteType.Bad:
                PlayerControler.Instance.TakeDamage(NotePool.Instance.damage);
                break;
        }
    }
}
