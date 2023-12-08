using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public float movespeed = 15f;
    public PlayerControls playerControls;

    private InputAction pause;
    private InputAction inventory;
    private InputAction move;
    private InputAction grab;

    public bool isGrabbing = false;
    private bool canGrab = false;


    Vector3 movedirection = Vector3.zero;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        move = playerControls.Player.Move;
        grab = playerControls.Player.Grab;
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Read the direction values for movement
        movedirection = move.ReadValue<Vector2>();

        // If grab was just pressed and something is in range, you isGrabbing
        // If you let go of the button OR nothing is in range, you ain't isGrabbing
        if(grab.WasPerformedThisFrame() && this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = true;
        else if(!grab.IsInProgress() || !this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = false;

        
        
    }

    void FixedUpdate()
    {
        // Moves Player by applying a force in a direction on their rigidbody.
        rb.AddForce(new Vector2(movedirection.x, movedirection.y) * movespeed * 2.5f);
    }
}
