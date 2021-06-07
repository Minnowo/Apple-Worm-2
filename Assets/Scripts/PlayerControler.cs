using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAction { AttackUp, AttackDown }

public class PlayerControler : MonoBehaviour
{

    //song completion
    public delegate void PlayerInputtedEvent(PlayerAction action);
    public static event PlayerInputtedEvent PlayerInputted;

    public bool IsAttacking { get; set; }
    public float AttackSpeed { get; set; }
    public int SwordLength { get; set; }

    private float horizontal;
    private float vertical;

    public float speed = 5;
    public PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AttackUp(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        PlayerInput(PlayerAction.AttackUp);
    }
    public void AttackDown(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        PlayerInput(PlayerAction.AttackDown);
    }

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
}
