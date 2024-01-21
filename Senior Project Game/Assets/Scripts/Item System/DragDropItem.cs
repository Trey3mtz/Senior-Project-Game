using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using System.Threading;
using Unity.Transforms;


namespace Cyrcadian.PlayerSystems.InventorySystem
{
    
    public class DragDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {    
        [SerializeField] private Inventory_UI parentUI;
        [SerializeField] private GraphicRaycaster g_Raycaster;
        [SerializeField] private Tooltip_Trigger tooltip;
        [HideInInspector] public Transform parentAfterDrag;
        [HideInInspector] public Item item;
        public int thisIndex;
        public int amountStacked;

        [SerializeField] private float dampeningSpeed = .01f;
        private RectTransform draggingObjectRectTransform;
        private Vector3 veloctiy = Vector3.zero;
        private Image image;

        [SerializeField] private TextMeshProUGUI childText;
        [SerializeField] private AudioClip pickupFX;
        [SerializeField] private AudioClip removeFX;


        // Checks if we are holding something, and a grace period of pressing down on an item
        private bool currentlyHoldingItem = false;
        private bool justPressedDown = false;

        public void InitializeItem(Item newItem, int newIndex, int amount)
        {
            // If in a quickslot on the HUD, inventory UI will not be its parent so we need to double check
            parentUI = GetComponentInParent<Inventory_UI>();
            if(!parentUI)
                parentUI = GetComponentInParent<QuickSlot>().parentUI;
            g_Raycaster = GetComponentInParent<GraphicRaycaster>();

            image = GetComponent<Image>();    
            draggingObjectRectTransform = transform as RectTransform;

            item = newItem;
            amountStacked = amount;
            image.sprite = newItem.ItemSprite;
            tooltip.header = newItem.Tooltip_header;
            tooltip.content = newItem.Tooltip_content;

            thisIndex = newIndex;
        }

        // Left click is the entire stack. Right clicks will pick up only 1.
        public void OnPointerDown(PointerEventData eventData)
        {
            Tooltip_System.Instance.ToggleVisibilityOff();
            if (eventData.button == PointerEventData.InputButton.Left && !currentlyHoldingItem)             // LEFT CLICK DOWN  ------------------------- (empty)
            {   
                eventData.pointerClick = transform.gameObject;
                parentAfterDrag = transform.parent;
                transform.SetParent(transform.root);
                transform.SetAsLastSibling();
                parentUI.RemovedItemIndex(thisIndex);   

                justPressedDown = true;
                currentlyHoldingItem = true;

                StartCoroutine(FollowMousePosition(eventData));   
                StartCoroutine(PressGracePeriod());  
                AudioManager.Instance.PlaySoundFX(pickupFX);
            }
            else if (eventData.button == PointerEventData.InputButton.Right && !currentlyHoldingItem)       // RIGHT CLICK DOWN ------------------------- (empty)
            {   // If the stack is already at 1, it should just work like the Left Click Down
               if(amountStacked == 1)
               { 
                    parentAfterDrag = transform.parent;
                    transform.SetParent(transform.root);
                    transform.SetAsLastSibling();
                    parentUI.RemovedItemIndex(thisIndex);   

                    currentlyHoldingItem = true;

                    StartCoroutine(FollowMousePosition(eventData));
                    AudioManager.Instance.PlaySoundFX(pickupFX);
               }
               else // If there is more than 1 in the stack, unparent and create leftOver stack
               {    // Since this section only happens when not holding an item, stack = 1 and leave the rest
                    parentAfterDrag = transform.parent;
                    transform.SetParent(transform.root);
                    transform.SetAsLastSibling();
                    parentUI.RemovedItemIndex(thisIndex);

                    // If it would be less than zero, don't add an item of size 0
                    // This would duplicate items weirdly
                    // Create a new object to leave behind
                            var newObj = new GameObject("InventoryItem", typeof(DragDropItem));
                            newObj.GetComponent<DragDropItem>().item = item;
                            newObj.GetComponent<DragDropItem>().amountStacked = amountStacked-1;
                        
                            if(parentAfterDrag.GetComponent<InventorySlot>())
                                parentAfterDrag.GetComponent<InventorySlot>().DropInSlot(newObj.GetComponent<DragDropItem>());
                            else
                                parentAfterDrag.GetComponent<QuickSlot>().DropInSlot(newObj.GetComponent<DragDropItem>());
                            newObj.transform.SetParent(parentAfterDrag);
                    
                    amountStacked = 1;
                    currentlyHoldingItem = true;

                    StartCoroutine(FollowMousePosition(eventData));
                    AudioManager.Instance.PlaySoundFX(pickupFX);
               }
            }
            else if(eventData.button == PointerEventData.InputButton.Right && currentlyHoldingItem)         // RIGHT CLICK DOWN ------------------------- (holding item)
            {   // If holding an item, check if what you are hovering is the same to pick up more of it.
                // Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();
                g_Raycaster.Raycast(eventData, results);  

                bool isOverSomeSlot = false;
                GameObject slotObj = null;
                InventorySlot inventorySlotComponent = null;
                QuickSlot quickSlotComponent = null;

                foreach(RaycastResult result in results)
                {
                    if(result.gameObject.GetComponent<InventorySlot>() || result.gameObject.GetComponent<QuickSlot>())
                        { isOverSomeSlot = true;    slotObj = result.gameObject; }

                    if(isOverSomeSlot)
                    {
                        if(!slotObj.gameObject.GetComponent<QuickSlot>())
                            inventorySlotComponent = slotObj.GetComponent<InventorySlot>();
                        else
                            quickSlotComponent = slotObj.GetComponent<QuickSlot>();  
                        break;                     
                    }
                } 
                // Has an inventoryslot, or has quickslot, or isn't not over a slot
                if(inventorySlotComponent)
                {   // Over an empty slot, just drop 1 item on right click
                    if(slotObj.transform.childCount == 0)
                    { 
                        // If it would be less than zero, don't add an item of size 0
                        // This would duplicate items weirdly
                        if(amountStacked > 1)
                        {   // Create a new object to leave behind
                            var newObj = new GameObject("InventoryItem", typeof(DragDropItem));
                            newObj.GetComponent<DragDropItem>().item = item;
                            newObj.GetComponent<DragDropItem>().amountStacked = 1;
                            // Parent it accordingly
                            newObj.transform.SetParent(slotObj.transform);
                            inventorySlotComponent.DropInSlot(newObj.GetComponent<DragDropItem>());
                            amountStacked--;
                        }
                        else
                        {
                            inventorySlotComponent.DropInSlot(this);
                            transform.SetParent(slotObj.transform);
                        }                        
                    }
                    // If matching items, decrement that slot and increment our holding item amount
                    else if(slotObj.GetComponentInChildren<DragDropItem>().item == item)
                    {
                        inventorySlotComponent.DecrementSlot();
                        amountStacked++;
                    }
                }
                else if(quickSlotComponent)
                {
                    if(slotObj.transform.childCount == 0)
                    {
                        // If it would be less than zero, don't add an item of size 0
                        // This would duplicate items weirdly
                        if(amountStacked > 1)
                        {   // Create a new object to leave behind
                            var newObj = new GameObject("InventoryItem", typeof(DragDropItem));
                            newObj.GetComponent<DragDropItem>().item = item;
                            newObj.GetComponent<DragDropItem>().amountStacked = 1;
                            // Parent it accordingly
                            newObj.transform.SetParent(slotObj.transform);
                            quickSlotComponent.DropInSlot(newObj.GetComponent<DragDropItem>());
                            amountStacked--;
                        }
                        else
                        {
                            quickSlotComponent.DropInSlot(this);
                            transform.SetParent(slotObj.transform);
                        }
        
                    }
                    // If matching items, decrement that slot and increment our holding item amount
                    else if(slotObj.GetComponentInChildren<DragDropItem>().item == item)
                    {
                        quickSlotComponent.DecrementSlot();
                        amountStacked++;
                    }
                }
                else   // If not over UI, Decrement our stack size, and spawn a world item
                {     
                        amountStacked--;
                        Transform playerPosition = GameObject.Find("Player").transform;
                        World_Item.SpawnWorldItem(playerPosition.position, item, 1);
                        AudioManager.Instance.PlaySoundFX(removeFX);
                }

                if(amountStacked < 1)
                {
                    Destroy(gameObject);
                }   
            }

            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                // Middle mouse button currently has no planned function for items in inventory
            }
            RefreshStackAmount();
        }


        // Waits for grace period to pass, as to not immediately drop an item as soon as you click it
        public void OnPointerUp(PointerEventData eventData)
        {   
            if(justPressedDown)
                return;

            if(eventData.button == PointerEventData.InputButton.Left && currentlyHoldingItem)
            {
                // Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();
                g_Raycaster.Raycast(eventData, results);

                bool isOverSomeSlot = false;
                bool isItemOverUI = false;
                GameObject slotObj = null;
                // Go through list. Ignoring this gameobject, are there other UI behind it?
                foreach(RaycastResult result in results)
                {
                    if(result.gameObject != this.gameObject)
                        isItemOverUI = true;
                    if(result.gameObject.GetComponent<InventorySlot>() || result.gameObject.GetComponent<QuickSlot>())
                        { isOverSomeSlot = true;    slotObj = result.gameObject; }
                }
          
                // If clicked, and the only UI gameobjects hover are the UI canvas and the item, "Drop" the item into the world
                // Also checks if NOT hovered over an inventory slotObj
                if(isItemOverUI && !isOverSomeSlot)
                {       
                        transform.SetParent(parentAfterDrag);
                        image.raycastTarget = true;
                        parentUI.ItemReturnedToSlot(this, thisIndex);
                        currentlyHoldingItem = false;
                }
                else if(!isItemOverUI)
                {          
                        Transform playerPosition = GameObject.Find("Player").transform;
                        World_Item.SpawnWorldItem(playerPosition.position, item, amountStacked);
                        AudioManager.Instance.PlaySoundFX(removeFX);
                        Tooltip_System.Instance.ToggleVisibilityOn();
                    Destroy(gameObject);
                }
                else if(isOverSomeSlot)
                {   
                    DragDropItem newDragDropItem = this;

                    if(!slotObj.GetComponent<QuickSlot>())
                    {                
                        if(slotObj.transform.childCount == 0)
                        { 
                            newDragDropItem.thisIndex = slotObj.GetComponent<InventorySlot>().slotIndex;
                            newDragDropItem.transform.SetParent(slotObj.transform);
                            slotObj.GetComponent<InventorySlot>().DropInSlot(newDragDropItem);
                        }   
                        else
                            slotObj.GetComponent<InventorySlot>().DropInSlot(newDragDropItem);    
                    }
                    else
                    {    
                        if(slotObj.transform.childCount == 0)
                        { 
                            newDragDropItem.thisIndex = slotObj.GetComponent<QuickSlot>().slotIndex;
                            newDragDropItem.transform.SetParent(slotObj.transform);
                            slotObj.GetComponent<QuickSlot>().DropInSlot(newDragDropItem);
                        }   
                        else
                            slotObj.GetComponent<QuickSlot>().DropInSlot(newDragDropItem);    
                    }

                    Destroy(this.gameObject);
                }
            }
        }

        // Will keep following mouse until you click somewhere else again
        private IEnumerator FollowMousePosition(PointerEventData eventData)
        {    
            if(RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingObjectRectTransform, Mouse.current.position.ReadValue(), eventData.pressEventCamera, out var globalMousePosition))
            {
                draggingObjectRectTransform.position = Vector3.SmoothDamp(draggingObjectRectTransform.position, globalMousePosition, ref veloctiy, dampeningSpeed);
            }
            yield return new WaitForEndOfFrame();
            if(currentlyHoldingItem)
                StartCoroutine(FollowMousePosition(eventData));
        }

        private IEnumerator PressGracePeriod()
        {     
            yield return new WaitForSeconds(.15f);
            justPressedDown = false;
        }

        private void RefreshStackAmount()
        {
            if(amountStacked > 1)
                childText.SetText("x" + amountStacked.ToString());
            else
               childText.SetText("");            
        }
    }
}
