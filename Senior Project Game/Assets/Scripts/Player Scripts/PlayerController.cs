using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyrcadian
{

public class PlayerController : MonoBehaviour
{
    [SerializeReference] public GameStateManager gameStateManager;
    [SerializeField] Camera _Camera;
    [SerializeField] private Inventory_UI UI_inventory;
    private Collect_World_Item collect_item;

    public Rigidbody2D rb;
    public float movespeed = 15f;
    public PlayerControls playerControls;

    private Inventory inventory;

    private InputAction pause;
    private InputAction inventoryOpen;
    private InputAction look;
    private InputAction move;
    private InputAction grab;
    private InputAction useItem;

    public bool isGrabbing = false;

    Vector3 movedirection = Vector3.zero;
    public Vector2 lookdirection = Vector2.zero;
    
    private void Awake()
    {
        gameStateManager = GameObject.FindAnyObjectByType<GameStateManager>();
        collect_item = GetComponentInChildren<Collect_World_Item>();
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        inventory = new Inventory();
        UI_inventory.SetInventory(inventory);
        collect_item.SetInventory(inventory);
    }

    void OnEnable()
    {
        move = playerControls.Player.Move;
        grab = playerControls.Player.Grab;
        look = playerControls.Player.Look;
        pause = playerControls.Player.Pause;
        useItem = playerControls.Player.Item;
        inventoryOpen = playerControls.Player.Inventory;
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }



    // Update is called once per frame
    void Update()
    {
        /**************************************************************************************************
        
            Movement:

                -Reads the values given and put them into a variable for movement
                -Variable is used in the FixedUpdate below for physics purposes
        */
        movedirection = move.ReadValue<Vector2>();

        /**************************************************************************************************
        
            Look:

                -Reads the values given and put them into a variable for camera control
                -
        */
        lookdirection = _Camera.ScreenToWorldPoint(look.ReadValue<Vector2>());
        
        /**************************************************************************************************
        
            Grab:

                -If grab was just pressed and something is in range, you isGrabbing
                -If you let go of the button OR nothing is in range, you ain't isGrabbing      
        */
        if(grab.WasPerformedThisFrame() && this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = true;
        else if(!grab.IsInProgress() || !this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = false;


        /**************************************************************************************************
        BIG DESIGN QUESTION:   Should we NOT pause the game while looking at inventoryOpen to create more tension?
            inventoryOpen:

                -Opens Inventory UI if it isn't open
                -Closes Inventory UI if it is open already
        */
        if(inventoryOpen.WasPerformedThisFrame() && !gameStateManager.isInventory && !gameStateManager.isPaused)
        {
            gameStateManager.OpenInventory();
            gameStateManager.isInventory = true;
        }   
        else if(inventoryOpen.WasPerformedThisFrame() && gameStateManager.isInventory)
        {
            gameStateManager.CloseInventory();
            gameStateManager.isInventory = false;
        }

        /**************************************************************************************************
        
            Pause Game:

                -If pause is pressed and isn't yet paused, pause game
                -If pause is pressed and it is already paused, unpause the game
        */
        if(pause.WasPerformedThisFrame() && !gameStateManager.isPaused)
        {
            gameStateManager.PauseGame();
            gameStateManager.isPaused = true;
        }   
        else if(pause.WasPerformedThisFrame() && gameStateManager.isPaused)
        {
            gameStateManager.UnpauseGame();
            gameStateManager.isPaused = false;
        }

    
        /**************************************************************************************************
        Future Note:    Usable items will be a class or scriptable object?
            Use Item in hand:

                -...
                -...      
        */
        if(useItem.WasPerformedThisFrame())
        {

        }
        
    }

    void FixedUpdate()
    {
        // Moves Player by applying a force in a direction on their rigidbody.
        rb.AddForce(new Vector2(movedirection.x, movedirection.y) * movespeed * 2.5f);
    }
}
}