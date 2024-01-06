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
        [SerializeField] public Inventory_UI parentUI;
        [HideInInspector] public Transform parentAfterDrag;
        [HideInInspector] public Item item;
        [HideInInspector] public int thisIndex;


        [SerializeField] private float dampeningSpeed = .05f;
        private RectTransform draggingObjectRectTransform;
        private Vector3 veloctiy = Vector3.zero;
        private Image image;
        private int amountStacked;


        public void InitializeItem(Item newItem, int newIndex, int amount)
        {
            image = GetComponent<Image>();
            parentUI = GetComponentInParent<Inventory_UI>();
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
           }
           else
           {
                Transform playerPosition = GameObject.Find("Player").transform;
                World_Item.SpawnWorldItem(playerPosition.position, item, amountStacked);
                parentUI.RemovedItemIndex(thisIndex);

               Destroy(gameObject);          
           }
       }
    }

}
