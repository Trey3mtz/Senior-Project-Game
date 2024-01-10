using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class QuickSlot : MonoBehaviour, IDropHandler 
    {
        public int slotIndex;
        // This slot is not a child of the Inventory UI, but the HUD UI. This will be set in the UI prefab
        [SerializeField] public Inventory_UI parentUI;
        [SerializeField] private AudioSource slotdropSFX;

        [SerializeField] Image slotImage;
        [SerializeField] Color selectedColor, notSelectedColor;

        private void Awake()
        {
            Deselect();
        }

        public void Select()
        {
            slotImage.color = selectedColor;
        }

        public void Deselect()
        {
            slotImage.color = notSelectedColor;
        }

        public void EquipInHand(GameObject equipped)
        {
            if(transform.childCount == 0)
            {
                
                
                
               
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
                GameObject dropped = eventData.pointerDrag;
                // Get rid of the original object, we only need its data. Otherwise clones would be left.
                Destroy(eventData.pointerDrag);
                DragDropItem draggedItem = dropped.GetComponent<DragDropItem>();
            
            if(transform.childCount == 0)
            {   
                draggedItem.parentAfterDrag = transform;
                parentUI.DropItemIntoSlot(draggedItem, slotIndex);
                draggedItem.thisIndex = slotIndex;
            }
            else
            {
                parentUI.DropItemIntoSlot(draggedItem, slotIndex);
            }

            slotdropSFX.Play();
        }
    }
}