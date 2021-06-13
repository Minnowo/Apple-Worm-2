using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreText : MonoBehaviour
{
    public static PlayerScoreText Instance = null;
    
    public static int PlayerScore
    {
        get
        {
            return Instance.score;
        }
        set
        {
            Instance.score = 5 * Mathf.RoundToInt(value / 5f);
            Instance.t.text = "Score: " + Instance.score;
        }
    }
    private int score = 0;

    private Text t;

    private void Awake()
    {
        t = GetComponent<Text>();
        Instance = this;
        PlayerScore = 0;
    }
}
