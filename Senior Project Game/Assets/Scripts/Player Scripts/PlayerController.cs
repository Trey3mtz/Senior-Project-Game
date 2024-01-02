using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

using Cyrcadian.PlayerSystems.InventorySystem;


namespace Cyrcadian.PlayerSystems
{
public class PlayerController : MonoBehaviour
{
    [SerializeReference] public GameStateManager gameStateManager;

    [SerializeField] Camera _Camera;


    public Rigidbody2D rb;
    public float movespeed = 15f;
    public PlayerControls playerControls;

    private PlayerData playerData;

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
        gameStateManager = FindAnyObjectByType<GameStateManager>();

        playerData = GetComponent<PlayerData>();
        playerControls = new PlayerControls();

        move            = playerControls.FindAction("Move");
        grab            = playerControls.FindAction("Grab");
        look            = playerControls.FindAction("Look");
        pause           = playerControls.FindAction("Pause");
        useItem         = playerControls.FindAction("Item");
        inventoryOpen   = playerControls.FindAction("Inventory");
    }



    void OnEnable()
    {
        move.performed += OnMove;
        move.canceled += OnMoveSTOP;
        grab.performed += OnGrab;
        grab.canceled += OnGrabSTOP;
        look.performed += OnLook;
        pause.performed += OnPause;
        useItem.performed += OnItem;
        inventoryOpen.performed += OnInventory;
        playerControls.Enable();
    }

    void OnDisable()
    {
        move.performed -= OnMove;
        move.canceled -= OnMoveSTOP;
        grab.performed -= OnGrab;
        grab.canceled -= OnGrabSTOP;
        look.performed -= OnLook;
        pause.performed -= OnPause;
        useItem.performed -= OnItem;
        inventoryOpen.performed -= OnInventory;
        playerControls.Disable();
    }

    /**************************************************************************************************
        
            Movement:

                -Reads the values given and put them into a variable for movement
                -Variable is used in the FixedUpdate below for physics purposes
    */    
    private void OnMove(InputAction.CallbackContext context)
    {
        movedirection = context.ReadValue<Vector2>();
    }
    private void OnMoveSTOP(InputAction.CallbackContext context)
    {
        movedirection = new Vector2();
    }

    /**************************************************************************************************

            Grab:

                -If grab was just pressed and something is in range, you isGrabbing
                -If you let go of the button OR nothing is in range, you ain't isGrabbing      
    */
    private void OnGrab(InputAction.CallbackContext context)
    {
        if(this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = true;
    }
    private void OnGrabSTOP(InputAction.CallbackContext context)
    {
        isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = false;
    }

    /**************************************************************************************************
        
            Look:

                -Reads the values given and put them into a variable for camera control
                -
    */
    private void OnLook(InputAction.CallbackContext context)
    {
        lookdirection = _Camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    /**************************************************************************************************
        Future Note:    Usable items will be a class or scriptable object?
            Use Item in hand:

                -...
                -...      
    */
    private void OnItem(InputAction.CallbackContext context)
    {

    }

    /**************************************************************************************************
        
            Pause Game:

                -If pause is pressed and isn't yet paused, pause game
                -If pause is pressed and it is already paused, unpause the game
    */
    private void OnPause(InputAction.CallbackContext context)
    {
        if(!gameStateManager.isPaused)
        {
            gameStateManager.PauseGame();
            gameStateManager.isPaused = true;
        }   
        else if(gameStateManager.isPaused)
        {
            gameStateManager.UnpauseGame();
            gameStateManager.isPaused = false;
        }
    }

    /**************************************************************************************************
        BIG DESIGN QUESTION:   Should we NOT pause the game while looking at inventoryOpen to create more tension?
            inventoryOpen:

                -Opens Inventory UI if it isn't open
                -Closes Inventory UI if it is open already
    */
    private void OnInventory(InputAction.CallbackContext context)
    {
        if(!gameStateManager.isInventory && !gameStateManager.isPaused)
        {
            gameStateManager.OpenInventory();
            gameStateManager.isInventory = true;
        }   
        else if(gameStateManager.isInventory)
        {
            gameStateManager.CloseInventory();
            gameStateManager.isInventory = false;
        }
    }



    

    void FixedUpdate()
    {
        // Moves Player by applying a force in a direction on their rigidbody.
        rb.AddForce(new Vector2(movedirection.x, movedirection.y) * movespeed * 2.5f);

        // Checks if object it was grabbing left grab range
        if(!this.gameObject.GetComponent<DistanceJoint2D>().connectedBody)
            isGrabbing = this.gameObject.GetComponent<DistanceJoint2D>().enabled = false;
    }
}




}