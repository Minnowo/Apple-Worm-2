using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public delegate void PlayerInputtedEvent(PlayerAction action);
    public static event PlayerInputtedEvent PlayerInputted;

    public static PlayerControler Instance = null;
    public bool IsAttacking { get; set; }
    public float AttackSpeed { get; set; }
    public int SwordLength { get; set; }
    public int Health = 100;

    public float speed = 5;
    public PlayerInput playerInput;

    private Rigidbody2D rigidBody;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX - Conductor.Instance.badOffsetX*2, 0, 5);

        UIPrefabs.HealthBar.SetMaxHealth(Health);
    }

    private void Update()
    {
        this.rigidBody.transform.position = new Vector3(
            this.rigidBody.transform.position.x, 
            Mathf.Sin(1 + Time.time) * 0.5f, 
            this.rigidBody.transform.position.z);
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


        PlayerInput(PlayerAction.AttackUp);
        OnHitManager.Instance.circleIndicators[0].PressDownEffect();
        //print($"track: {0}, beat: {Conductor.Instance.T}");
    }
    public void AttackDown(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        PlayerInput(PlayerAction.AttackDown);
        OnHitManager.Instance.circleIndicators[1].PressDownEffect();

        //print($"track: {1}, beat: {Conductor.Instance.T}");
    }

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
}
