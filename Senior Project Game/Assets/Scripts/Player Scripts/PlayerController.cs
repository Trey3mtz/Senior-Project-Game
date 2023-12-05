using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public float movespeed = 15f;
    public PlayerControls playerControls;

    private InputAction move;

    Vector3 movedirection = Vector3.zero;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        move = playerControls.Player.Move;
        playerControls.Enable();
    }
    void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {

        // Read the direction values
        movedirection = move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Moves Player by applying a force in a direction on their rigidbody.
        rb.AddForce(new Vector2(movedirection.x, movedirection.y) * movespeed * 2.5f);
    }
}
