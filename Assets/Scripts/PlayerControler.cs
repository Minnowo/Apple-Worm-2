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
        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX + Conductor.Instance.goodOffsetX, 0, 5);

        UIPrefabs.HealthBar.SetMaxHealth(Health);
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
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
