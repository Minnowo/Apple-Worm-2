using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Collection")]
public class SongCollection : ScriptableObject
{
    
    public SongSet[] songSets;

    [Serializable]
    public class SongSet
    {
        public SongInfo easy;
        public SongInfo normal;
        public SongInfo hard;

        public bool IsEmpty()
        {
            return easy == null && normal == null && hard == null && easy.Song == null && normal.Song == null && hard.Song == null;
        }

        public int SongCount()
        {
            int c = 3;

            if (easy.tracks.Length != 2 || easy.Song == null)
                c--;

            if (normal.tracks.Length != 2 || normal.Song == null)
                c--;

            if (hard.tracks.Length != 2 || hard.Song == null)
                c--;

            return c;
        }
    }
}
