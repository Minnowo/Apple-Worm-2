using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongMessenger : MonoBehaviour
{
    public static SongMessenger Instance = null;

    public SongInfo CurrentSong;
    public int songIndex;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
