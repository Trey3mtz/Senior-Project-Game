using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class InventorySlot : MonoBehaviour ,IDropHandler
    {
        
        public int slotIndex;
        [SerializeField] private Inventory_UI parentUI;

        [SerializeField] private AudioSource slotdropSFX;
        [SerializeField] private AudioClip slotDropFX;

        private void Awake()
        {
            parentUI = GetComponentInParent<Inventory_UI>();
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

            //slotdropSFX.Play();
            AudioManager.Instance.PlaySoundFX(slotDropFX);
        }
    }
}