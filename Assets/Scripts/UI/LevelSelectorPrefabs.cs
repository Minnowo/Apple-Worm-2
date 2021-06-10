using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorPrefabs : MonoBehaviour
{
    public static Font BubbleFont
    {
        get
        {
            return Instance.bubbleFont;
        }
    }
    public Font bubbleFont;




    public static LevelSelectorPrefabs Instance = null;
    private void Awake()
    {
        Instance = this;
    }
}
