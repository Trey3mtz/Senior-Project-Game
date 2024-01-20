using System;
using UnityEngine;

namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class InventorySlot : MonoBehaviour
    {
        
        public int slotIndex;
        [SerializeField] private Inventory_UI parentUI;
        [SerializeField] private AudioSource slotdropSFX;
        [SerializeField] private AudioClip slotDropFX;

        private void Awake()
        {
            parentUI = GetComponentInParent<Inventory_UI>();
        }

        // Try to drop item into slot
        public void DropInSlot(DragDropItem movedItem)
        {   
            if(transform.childCount == 0)
            {
                movedItem.parentAfterDrag = transform;
                movedItem.thisIndex = slotIndex;
            }
            parentUI.DropItemIntoSlot(movedItem, slotIndex);
            PlaySound();
        }

        public void DecrementSlot()
        {
            PlaySound();
            parentUI.DecrementItemIndex(slotIndex);
        }

        private void PlaySound()
        {   AudioManager.Instance.PlaySoundFX(slotDropFX);    }
    }
}