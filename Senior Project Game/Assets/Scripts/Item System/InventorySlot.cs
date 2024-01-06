using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class InventorySlot : MonoBehaviour ,IDropHandler
    {
        public int slotIndex;

        public void OnDrop(PointerEventData eventData)
        {
            if(transform.childCount == 0)
            {
                GameObject dropped = eventData.pointerDrag;
                DragDropItem draggedItem = dropped.GetComponent<DragDropItem>();
                draggedItem.parentAfterDrag = transform;

                GetComponentInParent<Inventory_UI>().ChangedSlottedItem(draggedItem.thisIndex, slotIndex);
                draggedItem.thisIndex = slotIndex;
            }
        }
    }
}