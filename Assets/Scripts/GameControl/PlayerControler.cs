using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public delegate void PlayerInputtedEvent(PlayerAction action);
    public static event PlayerInputtedEvent PlayerInputted;

    public delegate void PlayerTookDamageEvent(int newHealth, int oldHealth);
    public static event PlayerTookDamageEvent PlayerDamaged;

    public delegate void PlayerHealedEvent(int newHealth, int oldHealth);
    public static event PlayerHealedEvent PlayerHealed;

    public static PlayerControler Instance = null;
    public static int locationIndex = 1;

    public static int defaultInvincibleTime = 8;

    public int hitCooldownTime = 3;
    public int Health = 100;
    public int fadeSpeed = 5;
    public bool isRockForm = false;
    public bool isOnDamageCooldown = false;
    public bool resetSpriteAfterFadeOut = false;

    public float speed = 5;
    public PlayerInput playerInput;

    public float lastHitTime = 0;
    public AudioSource[] dmgSounds;

    // 1 is the bottom
    private int currentTrackIndex
    {
        get
        {
            return curIndex;
        }
        set
        {
            curIndex = value;
            PlayerControler.locationIndex = value;

            y = Conductor.Instance.trackSpawnYPos[value];
        }
    }
    private int curIndex = 1;

    private float y;

    private Sprite originalSprite;

    private Rigidbody2D rigidBody;
    private RectTransform rt;
    private SpriteRenderer sr;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();
        rt = this.GetComponent<RectTransform>();

        originalSprite = sr.sprite;

        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX - Conductor.Instance.badOffsetX * 2, 0, 5);
        y = rigidBody.transform.position.y;
        currentTrackIndex = 1;
        UIPrefabs.HealthBar.SetMaxHealth(Health);
    }

    private void Update()
    {
        if (isOnDamageCooldown)
        {
            if (isRockForm)
            {
                sr.color = new Color(1, 1, 1, 1);
            }
            else if (sr.color.a > 0.3)
            {
                sr.color = new Color(1, 1, 1, (sr.color.a - Time.deltaTime * fadeSpeed).Clamp(0, 1));
            }

            if (Time.time - hitCooldownTime >= lastHitTime)
            {
                isOnDamageCooldown = false;
            }
        }
        else if (sr.color.a < 1 || isRockForm)
        {
            sr.color = new Color(1, 1, 1, (sr.color.a + Time.deltaTime * fadeSpeed).Clamp(0, 1));

            if (resetSpriteAfterFadeOut)
            {
                sr.sprite = originalSprite;
                rt.localScale = new Vector3(0.5f, 0.5f, 1f);
                isRockForm = false;
                resetSpriteAfterFadeOut = false;
            }
        }


        this.rigidBody.transform.position = Vector3.MoveTowards(
                rigidBody.transform.position,
                new Vector3(
                    this.rigidBody.transform.position.x,
                    this.y + Mathf.Sin(2 + Time.time) * 0.35f,
                    this.rigidBody.transform.position.z),
                50 * Time.deltaTime);

        if (this.rigidBody.rotation > 5)
        {
            this.rigidBody.rotation = rigidBody.rotation + -1 * (3 * Time.deltaTime);
        }
        else if (this.rigidBody.rotation < -5)
        {
            this.rigidBody.rotation = rigidBody.rotation + 3 * Time.deltaTime;
        }
    }

    public void HealPlayer(int giveHp)
    {
        TakeDamage(-giveHp);
    }

    public void TakeDamage(int dmg)
    {
        if (dmg < 0)
        {
            OnPlayerHealed((Health - dmg).Clamp(0, 100), Health);

            Health = (Health - dmg).Clamp(0, 100);
            UIPrefabs.HealthBar.SetHealth(Health);

            dmgSounds[2].Play();
            return;
        }

        if (isOnDamageCooldown)
            return;

        PlayerTookDamage((Health - dmg).Clamp(0, 100), Health);

        Health = (Health - dmg).Clamp(0, 100);
        UIPrefabs.HealthBar.SetHealth(Health);

        if (dmg == NotePool.spikeDamage)
            dmgSounds[1].Play();
        else
            dmgSounds[0].Play();

        lastHitTime = Time.time;
        isOnDamageCooldown = true;
    }

    public void AttackUp(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        currentTrackIndex = 0;
        PlayerInput(PlayerAction.AttackUp);
        OnHitManager.Instance.circleIndicators[0].PressDownEffect();
        //print($"track: {0}, beat: {Conductor.Instance.T}");
    }
    public void AttackDown(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        currentTrackIndex = 1;
        PlayerInput(PlayerAction.AttackDown);
        OnHitManager.Instance.circleIndicators[1].PressDownEffect();

        //print($"track: {1}, beat: {Conductor.Instance.T}");
    }

    public void GiveIFrames(float time)
    {
        this.lastHitTime = Time.time + time;
        this.isOnDamageCooldown = true;
        resetSpriteAfterFadeOut = true;
        isRockForm = true;

        sr.sprite = UIPrefabs.AppleWormRockSkin;
        rt.localScale = new Vector3(0.75f, 0.75f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.activeInHierarchy)
            return;

        if (collider.gameObject.CompareTag("MusicNoteNORMAL"))
        {
            TakeDamage(NotePool.generalDamage);
        }

        if (collider.gameObject.CompareTag("MusicNoteBAD"))
        {
            TakeDamage(NotePool.spikeDamage);
            if (isRockForm)
            {
                MusicNode n = collider.gameObject.GetComponent<MusicNode>();
                n.hitPlayer = true;
                n.trackNumber = currentTrackIndex;
            }
        }

        if (collider.gameObject.CompareTag("MusicNoteINVINCIBLE"))
        {
            GiveIFrames(defaultInvincibleTime);
        }
    }

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
    private void PlayerTookDamage(int newHealth, int oldHealth, NoteType t = NoteType.Normal)
    {
        if (PlayerDamaged != null)
            PlayerDamaged(newHealth, oldHealth);
    }

    private void OnPlayerHealed(int newHealth, int oldHealth)
    {
        if (PlayerHealed != null)
            PlayerHealed(newHealth, oldHealth);
    }

}
