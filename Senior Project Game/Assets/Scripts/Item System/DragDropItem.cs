using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {    
        [SerializeField] private Inventory_UI parentUI;
        [HideInInspector] public Transform parentAfterDrag;
        [HideInInspector] public Item item;
         public int thisIndex;
         public int amountStacked;

        [SerializeField] private float dampeningSpeed = .05f;
        private RectTransform draggingObjectRectTransform;
        private Vector3 veloctiy = Vector3.zero;
        private Image image;


        [SerializeField] private AudioSource pickupSFX;
        [SerializeField] private AudioSource removeSFX;


        public void InitializeItem(Item newItem, int newIndex, int amount)
        {
            // If in a quickslot on the HUD, inventory UI will not be its parent so we need to double check
            parentUI = GetComponentInParent<Inventory_UI>();
            if(!parentUI)
                parentUI = GetComponentInParent<QuickSlot>().parentUI;

            image = GetComponent<Image>();    
            draggingObjectRectTransform = transform as RectTransform;

            item = newItem;
            image.sprite = newItem.ItemSprite;
            thisIndex = newIndex;
            amountStacked = amount;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            pickupSFX.Play();

            Debug.Log(parentUI);
            parentUI.RemovedItemIndex(thisIndex);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingObjectRectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePosition))
            {
                draggingObjectRectTransform.position = Vector3.SmoothDamp(draggingObjectRectTransform.position, globalMousePosition, ref veloctiy, dampeningSpeed);
            }
        }


       public void OnEndDrag(PointerEventData eventData)
       {
           if(eventData.pointerCurrentRaycast.gameObject != null)
           {          
                transform.SetParent(parentAfterDrag);
                image.raycastTarget = true;
                parentUI.ItemReturnedToSlot(this, thisIndex);
           }
           else
           {    
                Transform playerPosition = GameObject.Find("Player").transform;
                World_Item.SpawnWorldItem(playerPosition.position, item, amountStacked);
                
                removeSFX.Play();

               Destroy(gameObject, removeSFX.clip.length);
           }
       }
    }

}
