using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitManager : MonoBehaviour
{
    public AudioSource musicSource;
    public ParticleSystem ps;

    private Dictionary<int, ParticleSystem> particleSystems;
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();

        Conductor.beatOnHitEvent += BeatHit;

        particleSystems = new Dictionary<int, ParticleSystem>();
        float xpos = Conductor.Instance.finishLineX;
        float[] yPos = Conductor.Instance.trackSpawnYPos;

        for (int i = 0; i < yPos.Length; i++)
        {
            GameObject burstParticle = Instantiate<GameObject>(UIPrefabs.BurstParticle);

            burstParticle.transform.parent = this.transform;

            burstParticle.transform.position = new Vector3(
                xpos + Conductor.Instance.perfectOffsetX,
                yPos[i],
                6);

            particleSystems[i] = burstParticle.GetComponent<ParticleSystem>();
        }
    }

    private void BeatHit(int trackNumber, Rank rank, NoteType t)
    {
        //Debug.Log(rank);
        
        if (rank == Rank.MISS)
            return;

        musicSource.Play();
        particleSystems[trackNumber].Stop();
        particleSystems[trackNumber].Play();

        if (t == NoteType.Bad)
            PlayerControler.Instance.TakeDamage(NotePool.Instance.damage);
    }
}
