using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rank { PERFECT, GOOD, BAD, MISS };


public class Conductor : MonoBehaviour
{

    public delegate void BeatOnHitAction(int trackNumber, Rank rank);
    public static event BeatOnHitAction beatOnHitEvent;

    //song completion
    public delegate void SongCompletedAction();
    public static event SongCompletedAction songCompletedEvent;

    public bool Paused = false;

    public float badOffsetX;
    public float goodOffsetX;
    public float perfectOffsetX;

    public float[] trackSpawnYPos;
    public float startLineX;
    public float finishLineX;
    public float removeLineX;
    public float dequeueX;
    public float noteZPosition;

    //How many seconds have passed since the song started
    public float dspSongTime;

    public float songBpm;                           //Song beats per minute
    public float songPositionInBeats;               //Current song position, in beats    

    public static float songPosition;               //Current song position, in seconds
    public static float pauseTimeStamp = -1f;
    public static float secondsPerBeat;      // 
    public static int beatsShownInAdvance = 1;      // number of notes shown on screen

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource { get { return GetComponent<AudioSource>(); } }
    public SongInfo songInfo;

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
    private int numberOfTracks;

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
        PlayerControler.PlayerInputted += PlayerControler_PlayerInputted;
    }

    void Start()
    {
        Paused = true;
        pauseTimeStamp = -1f;

        songInfo = SongMessenger.Instance.CurrentSong;
        secondsPerBeat = 60f / songInfo.bpm;
        songLength = songInfo.Song.length;
        songBpm = SongMessenger.Instance.CurrentSong.bpm;

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

        PlayerControler.PlayerInputted += PlayerInputted;

        musicSource.clip = SongMessenger.Instance.CurrentSong.Song;
        StartSong();
    }

    public void StartSong()
    {
        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;


        //Start the music
        musicSource.Play();

        Paused = false;
        songStarted = true;
    }

    void PlayerInputted(PlayerAction action)
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (!songStarted)
            return;

        if (Paused)
        {
            if(pauseTimeStamp < 0f)
            {
                pauseTimeStamp = (float)AudioSettings.dspTime;
                musicSource.Pause();
            }
            return;
        }

        // was paused but now has been unpaused
        if(pauseTimeStamp > 0f)
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


        for(int i = 0; i < numberOfTracks; i++)
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
                currNote.note);

            //enqueue
            trackQueues[i].Enqueue(musicNode);

            //update the next index
            trackNoteIndices[i]++;
        }

        for(int i = 0; i < numberOfTracks; i++)
        {
            // empty queue
            if (trackQueues[i].Count < 1)
                continue;

            MusicNode curNode = trackQueues[i].Peek();

            if(curNode.transform.position.x < dequeueX)
            {
                // can remove the note because its going to be disabled by the BeatHit 
                // function
                trackQueues[i].Dequeue();

                BeatHit(i, Rank.MISS);
            }
        }

        if(songPosition >= songLength)
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
        if (offsetX < perfectOffsetX) //perfect hit
        {
            Debug.Log("perfect hit");
            frontNode.PerfectHit();

            //dispatch beat on hit event
            BeatHit(trackNumber, Rank.PERFECT);

            // remove the note from queue
            trackQueues[trackNumber].Dequeue();
            return;
        }
        
        if (offsetX < goodOffsetX) //good hit
        {
            Debug.Log("good hit");
            frontNode.GoodHit();

            //dispatch beat on hit event
            BeatHit(trackNumber, Rank.GOOD);

            // remove the note from queue
            trackQueues[trackNumber].Dequeue();
            return;
        }
        
        if (offsetX < badOffsetX) //bad hit
        {
            Debug.Log("bad hit");
            frontNode.BadHit();

            //dispatch beat on hit event
            BeatHit(trackNumber, Rank.BAD);

            // remove the note from queue
            trackQueues[trackNumber].Dequeue();
            return;
        }
    }

    void OnDestroy()
    {
        PlayerControler.PlayerInputted -= PlayerInputted;
    }

    private void SongFinished()
    {
        if (songCompletedEvent != null)
            songCompletedEvent();
    }

    private void BeatHit(int trackNumber, Rank rank)
    {
        if (beatOnHitEvent != null) 
            beatOnHitEvent(trackNumber, rank);
    }
}
