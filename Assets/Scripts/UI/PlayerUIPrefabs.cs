using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIPrefabs : MonoBehaviour
{
    public static PlayerUIPrefabs Instance = null;

    public static GameObject CircleIndicatorPrefab
    {
        get
        {
            return PlayerUIPrefabs.Instance.circleIndicatorPrefab;
        }
    }
    public GameObject circleIndicatorPrefab;


    public static GameObject BurstParticle
    {
        get
        {
            return PlayerUIPrefabs.Instance.burstParticle;
        }
    }
    public GameObject burstParticle;
    private void Awake()
    {
        Instance = this;
    }
}
