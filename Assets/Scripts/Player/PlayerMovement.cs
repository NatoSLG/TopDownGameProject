using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 5f;

    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public Vector2 lastMovedVector;

    private Rigidbody2D rb;
    PlayerStats player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); //sets the default direction on where to shoot projectiles on start
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManager()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        // Get horizontal and vertical inputs
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate the movement direction
        movement = new Vector2(horizontalInput, verticalInput).normalized;

        if (movement.x != 0)
        {
            lastHorizontalVector = movement.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);//stores last moved x
        }
        if (movement.y != 0)
        {
            lastVerticalVector = movement.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);//stores last moved y
        }
        if (movement.x != 0 && movement.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }

        //check if there is no inputs and stop player when no inputs are pressed
        if (horizontalInput == 0 && verticalInput == 0)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = movement * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
}
