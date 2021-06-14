using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class Conductor : MonoBehaviour
{
    public static Conductor Instance = null;

    public delegate void BeatOnHitAction(int trackNumber, Rank rank, NoteType type);
    public static event BeatOnHitAction beatOnHitEvent;

    public delegate void SongCompletedAction();
    public static event SongCompletedAction songCompletedEvent;

    public static bool Paused = false;

    public int startCountDown = 3;

    public bool pauseNoteAssist = true;
    public int pauseNoteAssistFrames = 25;

    public float badOffsetX = 1f;
    public float goodOffsetX = 0.5f;
    public float perfectOffsetX = 0.2f;

    public float[] trackSpawnYPos = new float[] { 4f, -4f };
    public float startLineX = 8;
    public float finishLineX = -8;
    public float removeLineX = -10;
    public float dequeueX = -9;
    public float noteZPosition = 1;

    //How many seconds have passed since the song started
    public float dspSongTime;

    public float songBpm;                           //Song beats per minute
    public float songPositionInBeats;               //Current song position, in beats    

    public static float songPosition;               //Current song position, in seconds
    public static float pauseTimeStamp = -1f;
    public static float secondsPerBeat;      // 
    public static float beatsShownInAdvance = 1f;      // number of notes shown on screen

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource { get { return GetComponent<AudioSource>(); } }
    public SongInfo songInfo;

    public Text countDownText;
    public Text comboText;
    public PlayerScoreText playerScore;
    public RankText rankText;

    public int combo;

    public int scoreComboMultiplier
    {
        get
        {
            return combo / 5;
        }
    }

    private float[] notes;

    private float pausedTime = 0f;
    private float songLength;

    private bool songStarted = false;

    // the number of horizontal tracks
    //
    //         ===o==o====o========= <- track 1
    // player
    //         =====o=o=o===o==o==o= <- track 2
    //
    // number of tracks = 2
    public int numberOfTracks { get; private set; }

    // keep track of by increasing a value at an index
    // you play the next note
    private int[] trackNoteIndices;

    // keeping track of which notes are on the screen
    private Queue<MusicNode>[] trackQueues;

    // keep track of notes that were on screen but no longer are
    private MusicNode[] previousNodes;

    // keep a referance to the tracks for the song
    private Track[] tracks;

    private void Awake()
    {
        Instance = this;
        PlayerControler.PlayerInputted += PlayerControler_PlayerInputted;
        PlayerControler.PlayerDamaged += PlayerDamaged;
    }

    void Start()
    {
        Paused = true;
        pauseTimeStamp = -1f;
        combo = 0;

        songInfo = SongMessenger.Instance.CurrentSong;
        secondsPerBeat = 60f / songInfo.bpm;
        songLength = songInfo.Song.length;
        songBpm = SongMessenger.Instance.CurrentSong.bpm;
        beatsShownInAdvance = songInfo.beatsShowInAdvance;

        numberOfTracks = trackSpawnYPos.Length;
        trackNoteIndices = new int[numberOfTracks];
        trackQueues = new Queue<MusicNode>[numberOfTracks];
        previousNodes = new MusicNode[numberOfTracks];

        for (int i = 0; i < numberOfTracks; i++)
        {
            // start at 0 (first note) this will be increased later
            trackNoteIndices[i] = 0;

            // set up queues for each track
            trackQueues[i] = new Queue<MusicNode>();

            // place holder to fill the array
            previousNodes[i] = null;
        }

        tracks = songInfo.tracks;

        //PlayerControler.PlayerInputted += PlayerInputted;

        musicSource.clip = SongMessenger.Instance.CurrentSong.Song;

        StartCoroutine(CountDown());
    }

    public IEnumerator DelayPlay()
    {
        Paused = false;
        songStarted = true;

        yield return new WaitForSeconds(songInfo.firstBeatOffset);
        musicSource.Play();
    }

    public void StartSong()
    {
        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        if (songInfo.delayMusicWithFirstBeatOffset)
        {
            StartCoroutine(DelayPlay());
            return;
        }
        //Start the music
        musicSource.Play();

        Paused = false;
        songStarted = true;
    }



    // Update is called once per frame
    void Update()
    {
        if (!songStarted)
            return;

        if (Paused)
        {
            if (pauseTimeStamp < 0f)
            {
                pauseTimeStamp = (float)AudioSettings.dspTime;
                musicSource.Pause();
            }
            return;
        }

        // was paused but now has been unpaused
        if (pauseTimeStamp > 0f)
        {
            pausedTime += (float)AudioSettings.dspTime - pauseTimeStamp;

            musicSource.Play();

            pauseTimeStamp = -1f;
        }

        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - pausedTime - songInfo.firstBeatOffset);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secondsPerBeat;

        float beatToShow = songPosition / secondsPerBeat + beatsShownInAdvance;

        for (int i = 0; i < numberOfTracks; i++)
        {
            int nextNoteIndex = trackNoteIndices[i];
            Track curTrack = tracks[i];

            if (nextNoteIndex >= curTrack.notes.Length)
                continue;

            if (curTrack.notes[nextNoteIndex].note >= beatToShow)
                continue;

            Note currNote = curTrack.notes[nextNoteIndex];

            MusicNode musicNode = NotePool.Instance.GetNode(
                startLineX,
                trackSpawnYPos[i],
                finishLineX,
                removeLineX,
                noteZPosition,
                currNote.note,
                currNote.type);

            //enqueue
            trackQueues[i].Enqueue(musicNode);

            //update the next index
            trackNoteIndices[i]++;
        }

        for (int i = 0; i < numberOfTracks; i++)
        {
            // empty queue
            if (trackQueues[i].Count < 1)
                continue;

            MusicNode curNode = trackQueues[i].Peek();

            if (pauseNoteAssist)
            {
                if ((curNode.transform.position.x - finishLineX).InRange(-perfectOffsetX, perfectOffsetX) && curNode.paused == false)
                {
                    curNode.paused = true;
                }
                else if (curNode.paused)
                {
                    if (curNode.pausedCounter > pauseNoteAssistFrames)
                    {
                        curNode.paused = false;
                        curNode.pausedCounter = 0;
                    }
                    else
                    {
                        curNode.pausedCounter++;
                    }
                }
            }

            if (curNode.type == NoteType.Bad && PlayerControler.Instance.isRockForm)
            {
                if (curNode.transform.position.x < finishLineX + goodOffsetX)
                {
                    if (i == PlayerControler.locationIndex)
                    {
                        curNode.trackNumber = i;
                        curNode.hitPlayer = true;

                        trackQueues[i].Dequeue();

                        // influence the beat hit to be good instead of the spikes
                        BeatHit(i, Rank.PERFECT, NoteType.Normal);
                        continue;
                    }
                }
            }

            if (curNode.transform.position.x < dequeueX)
            {
                // can remove the note because its going to be disabled by the BeatHit 
                // function
                trackQueues[i].Dequeue();

                // don't punish the player for missing the spikes, heals, or invincible power
                if (curNode.type == NoteType.Normal)
                    BeatHit(i, Rank.MISS, NoteType.Normal);
            }
        }

        if (songPosition >= songLength)
        {
            songStarted = false;
            SongFinished();
        }
    }

    private void PlayerControler_PlayerInputted(PlayerAction action)
    {
        int trackNumber = (int)action;

        if (trackQueues.Length < 1)
            return;

        if (trackQueues[trackNumber].Count < 1)
            return;

        MusicNode frontNode = trackQueues[trackNumber].Peek();

        float offsetX = frontNode.gameObject.transform.position.x - finishLineX;
        //print($"offsetX: {offsetX}, perfectHit: {perfectOffsetX}");

        if (offsetX.InRange(-perfectOffsetX, perfectOffsetX))
        {
            HitBeat(frontNode, Rank.PERFECT, trackNumber);
            return;
        }

        if (offsetX.InRange(-goodOffsetX, goodOffsetX)) //good hit
        {
            HitBeat(frontNode, Rank.GOOD, trackNumber);
            return;
        }

        if (offsetX.InRange(-badOffsetX, badOffsetX)) //bad hit
        {
            HitBeat(frontNode, Rank.BAD, trackNumber);
            return;
        }
    }

    private void HitBeat(MusicNode n, Rank r, int tracknumber)
    {
        //Debug.Log(r);
        switch (r)
        {
            case Rank.BAD:
                n.BadHit();
                PlayerScoreText.PlayerScore += 10 + scoreComboMultiplier;
                break;
            case Rank.GOOD:
                n.GoodHit();
                PlayerScoreText.PlayerScore += 20 + scoreComboMultiplier;
                break;
            case Rank.PERFECT:
                n.PerfectHit();
                PlayerScoreText.PlayerScore += 30 + scoreComboMultiplier;
                break;
        }

        if (PlayerControler.Instance.isRockForm && n.type == NoteType.Bad)
        {
            n.PerfectHit();

            //dispatch beat on hit event
            BeatHit(tracknumber, Rank.PERFECT, NoteType.Normal);

            // remove the note from queue
            trackQueues[tracknumber].Dequeue();
            return;
        }

        //dispatch beat on hit event
        BeatHit(tracknumber, r, n.type);

        // remove the note from queue
        trackQueues[tracknumber].Dequeue();
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        for (int i = startCountDown; i >= 1; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countDownText.gameObject.SetActive(false);

        StartSong();
    }

    public void UpdateComboText(bool keepCombo)
    {

        if (keepCombo)
        {
            combo++;
            comboText.text = "x" + combo.ToString();
            comboText.fontSize = 56 + ((int)Math.Round(combo * .6)).Clamp(0, 30);

            if (!comboText.gameObject.activeInHierarchy)
                comboText.gameObject.SetActive(true);

            // why tf does unity use 0-1 for colors its awful 
            // anyway combo / 255 to make it 0-1
            // but i'm using combo / 102 because combo * 2.5 / 255 is more work
            comboText.color = new Color(1f, (1f - (combo / 102f)).ClampMin(0), (1f - (combo / 102f)).ClampMin(0), 1);
            return;
        }

        combo = 0;
        comboText.text = "x" + combo.ToString();
        comboText.gameObject.SetActive(false);
    }

    private void PlayerDamaged(int newHealth, int oldHealth)
    {
        switch(newHealth - oldHealth)
        {
            // if they take damage from spike lose combo
            // otherwise they're fine
            case NotePool.spikeDamage:
                UpdateComboText(false);
                PlayerScoreText.PlayerScore -= oldHealth - newHealth;
                break;
            case NotePool.generalDamage:
                break;
        }
    }

    void OnDestroy()
    {
        PlayerControler.PlayerInputted -= PlayerControler_PlayerInputted;
        PlayerControler.PlayerDamaged -= PlayerDamaged;
    }

    private void SongFinished()
    {
        if (songCompletedEvent != null)
            songCompletedEvent();
    }

    private void BeatHit(int trackNumber, Rank rank, NoteType t)
    {
        rankText.ShowRank(rank, 2);

        if (rank == Rank.MISS)
        {
            UpdateComboText(false);
        }
        else
        {
            UpdateComboText(true);
        }

        if (beatOnHitEvent != null)
        {
            beatOnHitEvent(trackNumber, rank, t);
        }
    }
}
