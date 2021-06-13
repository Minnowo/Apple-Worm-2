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

    public static PlayerControler Instance = null;
    public static int locationIndex = 1;

    public int hitCooldownTime = 3;
    public int Health = 100;
    public int fadeSpeed = 5;
    public bool isOnDamageCooldown = false;

    public float speed = 5;
    public PlayerInput playerInput;

    public float lastHitTime = 0;

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

    private Rigidbody2D rigidBody;
    private SpriteRenderer sr;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();

        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX - Conductor.Instance.badOffsetX * 2, 0, 5);
        y = rigidBody.transform.position.y;
        currentTrackIndex = 1;
        UIPrefabs.HealthBar.SetMaxHealth(Health);
    }

    private void Update()
    {
        if (isOnDamageCooldown)
        {
            if(sr.color.a > 0.3)
            {
                sr.color = new Color(1, 1, 1, (sr.color.a - Time.deltaTime * fadeSpeed).Clamp(0, 1));
            }

            if (Time.time - hitCooldownTime >= lastHitTime)
            {
                isOnDamageCooldown = false;
            }
        }
        else if (sr.color.a < 1)
        {
            sr.color = new Color(1, 1, 1, (sr.color.a + Time.deltaTime * fadeSpeed).Clamp(0, 1));
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

    public void TakeDamage(int dmg)
    {
        if (!isOnDamageCooldown)
        {
            PlayerTookDamage((Health - dmg).Clamp(0, 100), Health);
            Health = (Health - dmg).Clamp(0, 100);
            UIPrefabs.HealthBar.SetHealth(Health);

            // i pass negative values to heal the player
            // this will not give the i frames for being healed
            if (dmg > 0)
            {
                lastHitTime = Time.time;
                isOnDamageCooldown = true;
            }
        }
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


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.activeInHierarchy)
            return;

        if (collider.gameObject.CompareTag("MusicNoteNORMAL"))
        {
            TakeDamage(NotePool.Instance.generalDamage);
        }

        if (collider.gameObject.CompareTag("MusicNoteBAD"))
        {
            TakeDamage(NotePool.Instance.spikeDamage);
        }

        if (collider.gameObject.CompareTag("MusicNoteINVINCIBLE"))
        {

        }
    }

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
    private void PlayerTookDamage(int newHealth, int oldHealth)
    {
        if (PlayerDamaged != null)
            PlayerDamaged(newHealth, oldHealth);
    }
}
