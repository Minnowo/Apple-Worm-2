using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Info")]
public class SongInfo : ScriptableObject
{
    public AudioClip Song;

    public string SongName;

    public float bpm;
    public float firstBeatOffset = 0;
    public bool delayMusicWithFirstBeatOffset = true;

    public Track[] tracks;
}

	[System.Serializable]
public class Note
{
    public float note;
    
    public NoteType type = NoteType.Normal;
}

	[System.Serializable]
public class Track
{
    public Note[] notes;
}