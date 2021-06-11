using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefabs : MonoBehaviour
{
    public static UIPrefabs Instance = null;

    public static GameObject CircleIndicatorPrefab
    {
        get
        {
            return UIPrefabs.Instance.circleIndicatorPrefab;
        }
    }
    public GameObject circleIndicatorPrefab;


    public static GameObject BurstParticle
    {
        get
        {
            return UIPrefabs.Instance.burstParticle;
        }
    }
    public GameObject burstParticle;

    public static Sprite DefaultMusicNodeSprite
    {
        get
        {
            return UIPrefabs.Instance.defaultMusicNodeSprite;
        }
    }
    public Sprite defaultMusicNodeSprite;

    public static Sprite HealMusicNodeSprite
    {
        get
        {
            return UIPrefabs.Instance.healMusicNodeSprite;
        }
    }
    public Sprite healMusicNodeSprite;

    public static Sprite BadMusicNodeSprite
    {
        get
        {
            return UIPrefabs.Instance.badMusicNodeSprite;
        }
    }
    public Sprite badMusicNodeSprite;

    public static HealthBar HealthBar
    {
        get
        {
            return Instance.healthBar;
        }
    }
    public HealthBar healthBar;

    public static CircleFlash FlashCirclePrefab
    {
        get
        {
            return Instance.flashCirclePrefab;
        }
    }
    public CircleFlash flashCirclePrefab;

    private void Awake()
    {
        Instance = this;
    }
}
