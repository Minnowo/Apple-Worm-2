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
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // get player's rigidbody
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        //horizontal = Input.GetAxisRaw("Horizontal");
        //vertical = Input.GetAxisRaw("Vertical");

        //rigidbody2D.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    public void AttackUp(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        PlayerInput(PlayerAction.AttackUp);
        Debug.Log("player attacked up");
    }
    public void AttackDown(InputAction.CallbackContext value)
    {
        if (!value.started)
            return;

        PlayerInput(PlayerAction.AttackDown);
        Debug.Log("player attacked dowm");
    }

    private void PlayerInput(PlayerAction inp)
    {
        if (PlayerInputted != null)
            PlayerInputted(inp);
    }
}
