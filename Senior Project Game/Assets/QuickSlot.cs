using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class QuickSlot : MonoBehaviour, IDropHandler 
    {
        public int slotIndex;

        // This slot is not a child of the Inventory UI, but the HUD UI. 
        [SerializeField] private Inventory_UI parentUI;

        public void EquipInHand(GameObject equipped)
        {
            if(transform.childCount == 0)
            {
                
    
                
               
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if(transform.childCount == 0)
            {
                GameObject dropped = eventData.pointerDrag;
                DragDropItem draggedItem = dropped.GetComponent<DragDropItem>();
                draggedItem.parentAfterDrag = transform;

                parentUI.DropItemIntoSlot(draggedItem, slotIndex);
                draggedItem.thisIndex = slotIndex;
            }
            else
            {
                
            }
        }
    }
}