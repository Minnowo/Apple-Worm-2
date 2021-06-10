using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
    public float startX;
    public float endX;
    public float removeLineX;
    public float beat;
	public NoteType notetype;

	public NoteType type
    {
        get
        {
			return t;
        }
        set
        {
			notetype = value;
			t = value;

			if (sr == null)
				return;
            switch (t)
            {
				case NoteType.Normal:
					sr.sprite = UIPrefabs.DefaultMusicNodeSprite;
					break;
				case NoteType.Bad:
					sr.sprite = UIPrefabs.BadMusicNodeSprite;
					break;
				case NoteType.Special:
					sr.sprite = UIPrefabs.SpecialMusicNodeSprite;
					break;
            }
        }
    }
	private NoteType t;

    public bool paused;
	private SpriteRenderer sr;
    private void Awake()
    {
		sr = GetComponent<SpriteRenderer>();
		sr.sortingLayerName = "MusicNote";
		//sr.sprite
    }

    public void Initialize(float startX, float posY, float endX, float removeLineX, float posZ, float targetBeat, NoteType nt = NoteType.Normal)
	{
		this.startX = startX;
		this.endX = endX;
		this.beat = targetBeat;
		this.removeLineX = removeLineX;
		this.type = nt;

		paused = false;

		//set position
		transform.position = new Vector3(startX, posY, posZ);
	}

    // Update is called once per frame
    void Update()
    {
		if (Conductor.pauseTimeStamp > 0f) 
			return; //resume not managed

		if (paused) 
			return; 

		transform.position = new Vector3(
			startX + (endX - startX) * (1f - (beat - Conductor.songPosition / Conductor.secondsPerBeat) / Conductor.beatsShownInAdvance), 
			transform.position.y, 
			transform.position.z);

		//remove itself when out of the screen (remove line)
		if (transform.position.x < removeLineX)
		{
			gameObject.SetActive(false);
		}
	}

	public void PerfectHit()
	{
		gameObject.SetActive(false);
	}

	public void GoodHit()
	{
		gameObject.SetActive(false);
	}

	public void BadHit()
	{
		gameObject.SetActive(false);
	}
}
