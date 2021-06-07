using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPickingScript : MonoBehaviour
{
    public SongInfo currentSong;
    public SongCollection[] songCollections;
    public static SongPickingScript Instance;

    private void Awake()
    {
        Instance = this;
    }
}
