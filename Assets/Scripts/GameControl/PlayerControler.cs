using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public delegate void PlayerInputtedEvent(PlayerAction action);
    public static event PlayerInputtedEvent PlayerInputted;

    public static PlayerControler Instance = null;
    public static int locationIndex = 1;

    public int Health = 100;

    public float speed = 5;
    public PlayerInput playerInput;

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

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX - Conductor.Instance.badOffsetX * 2, 0, 5);
        y = rigidBody.transform.position.y;
        currentTrackIndex = 1;
        UIPrefabs.HealthBar.SetMaxHealth(Health);
    }

    private void Update()
    {
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
        Health = (Health - dmg).Clamp(0, 100);
        UIPrefabs.HealthBar.SetHealth(Health);
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


  /*  private void OnTriggerEnter(Collider collider)
    {
        print("hit trigger");
        if (collider.gameObject.CompareTag("MusicNote"))
        {
            if (!collider.gameObject.activeInHierarchy)
                return;

            if(collider.tag == NoteType.Bad.ToString())
            {
                TakeDamage(NotePool.Instance.damage);
            }
        }
    }*/

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
}
