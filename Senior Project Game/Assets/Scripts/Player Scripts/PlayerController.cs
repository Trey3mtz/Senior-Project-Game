using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

using Cyrcadian.PlayerSystems.InventorySystem;
using Unity.VisualScripting;


namespace Cyrcadian.PlayerSystems
{
public class PlayerController : MonoBehaviour
{
        /// <summary>
        ///
        ///     This script is ONLY for taking in inputs from Controllers/Keyboard. If you want to create a new control or
        ///     ability that uses some button press, you must first go to the PlayerInput asset under "/Asset/Input Controls".
        ///     If you want to learn more about Unity's Input System watch "https://youtu.be/m5WsmlEOFiA?si=eBXWSUFplcAVH1U_".
        ///     
        ///     Setting up a new input in this script follows this format:
        ///                 - Create a private local InputAction, all lowercase for uniformity.      
        ///                 - In Awake() set that variable to what ever you named it in the PlayerInput asset
        ///                 - Follow the OnEnble / OnDisable format. The += means to "Subscribe" using Unity's Event System
        ///                 - What follows the += symbol the name of the new method you'll make to house the logic
        ///                 - Make the method private with the return type void. Names of methods must start with "On" for uniformity
        ///                 - Put (InputAction.CallbackContext context) as the only parameter of this method. FINISHED. 
        /// 
        /// </summary>  
        
    [SerializeReference] public GameStateManager gameStateManager;
    [SerializeField] Inventory_UI _inventoryUI;
    [SerializeField] Camera _Camera;


    public Rigidbody2D rb;
    public float movespeed = 15f;
    public PlayerControls playerControls;

    private PlayerData playerData;

    // Each InputAction must have its own local variable placed here
    private InputAction pause;
    private InputAction inventoryOpen;
    private InputAction look;
    private InputAction move;
    private InputAction grab;
    private InputAction useItem;
    private InputAction mouseScroll;
    private InputAction keyboardNum;

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
        mouseScroll     = playerControls.FindAction("MouseScroll");
        keyboardNum     = playerControls.FindAction("KeyboardNumber");
    }

    // Have all inputs subscribe to a method, so that it is called every time the InputAction is performed
    void OnEnable()
    {
        move.performed              += OnMove;
        move.canceled               += OnMoveSTOP;
        grab.performed              += OnGrab;
        grab.canceled               += OnGrabSTOP;
        look.performed              += OnLook;
        pause.performed             += OnPause;
        useItem.performed           += OnItem;
        inventoryOpen.performed     += OnInventory;
        mouseScroll.performed       += OnScroll;
        keyboardNum.performed       += OnKeyboardNumber;
        playerControls.Enable();
    }

    // Unsubscribe once you want to disable controls
    void OnDisable()
    {
        move.performed              -= OnMove;
        move.canceled               -= OnMoveSTOP;
        grab.performed              -= OnGrab;
        grab.canceled               -= OnGrabSTOP;
        look.performed              -= OnLook;
        pause.performed             -= OnPause;
        useItem.performed           -= OnItem;
        inventoryOpen.performed     -= OnInventory;
        mouseScroll.performed       -= OnScroll;
        keyboardNum.performed       -= OnKeyboardNumber;
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
            Use Item Selected:

                -Calls the inventoryUI script to use the item we have currently selected in Hotbar
                -The players position, and gameobject is passed
                -We don't know what the item is yet nor what it may do here, so we give it general information    
    */
    private void OnItem(InputAction.CallbackContext context)
    {
        if(!pointerOverUI && !isGrabbing)
            _inventoryUI.UseSelectedItem(gameObject.transform.position, gameObject);         
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
        DESIGN QUESTION:   Should we NOT pause the game while looking at inventoryOpen to create more tension?
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
        else if(gameStateManager.isInventory && !gameStateManager.isPaused)
        {
            gameStateManager.CloseInventory();
            gameStateManager.isInventory = false;
        }
    }

    /**************************************************************************************************
            Scroll through Hotbar:

                -Uses the mouse wheel to scroll through the hotbar
    */
    private void OnScroll(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() < 0)
            _inventoryUI.ChangeSelectedSlot(-1);
        else if(context.ReadValue<float>() > 0)
            _inventoryUI.ChangeSelectedSlot(1);
    }

    /**************************************************************************************************
            Select a specific Hotbar Slot:

                -Keyboard numbers select a specific Hotbar Slot
                -I thought it was nice to have both scrolling and traditional numbers as hotbar slots
    */
    private void OnKeyboardNumber(InputAction.CallbackContext context)
    {
        int newValue = context.ReadValue<float>().ConvertTo<int>() -1;
        
        _inventoryUI.SelectedSpecificSlot(newValue);
    }


// This finds out if mouse is over UI. If yes, do not try to use the hotbar slot selected
bool pointerOverUI = false;
void Update()
{
    pointerOverUI = EventSystem.current.IsPointerOverGameObject();
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