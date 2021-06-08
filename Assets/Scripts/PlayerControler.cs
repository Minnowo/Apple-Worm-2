using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public delegate void PlayerInputtedEvent(PlayerAction action);
    public static event PlayerInputtedEvent PlayerInputted;

    public bool IsAttacking { get; set; }
    public float AttackSpeed { get; set; }
    public int SwordLength { get; set; }

    public float speed = 5;
    public PlayerInput playerInput;

    private Rigidbody2D rigidBody;

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        rigidBody.transform.position = new Vector3(Conductor.Instance.finishLineX, 0, 5);

        float xpos = Conductor.Instance.finishLineX;
        float[] yPos = Conductor.Instance.trackSpawnYPos;

        for (int i = 0; i < yPos.Length; i++)
        {
            GameObject circleIndicator = Instantiate<GameObject>(PlayerUIPrefabs.CircleIndicatorPrefab);
            SpriteRenderer sr = circleIndicator.GetComponent<SpriteRenderer>();

            sr.sortingOrder = 5;

            circleIndicator.transform.parent = this.transform;

            circleIndicator.transform.position = new Vector3(
                xpos + Conductor.Instance.perfectOffsetX, 
                yPos[i], 
                5);
        }
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
