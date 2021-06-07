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
    }
}
