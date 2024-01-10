using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Entities.UniversalDelegates;

namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class QuickSlot : MonoBehaviour, IDropHandler 
    {
        public int slotIndex;
        // This slot is not a child of the Inventory UI, but the HUD UI. This will be set in the UI prefab
        [SerializeField] public Inventory_UI parentUI;
        [SerializeField] private AudioSource slotdropSFX;
        [SerializeField] private AudioSource selectSFX;

        [SerializeField] Image slotImage;
        [SerializeField] Color selectedColor, notSelectedColor;

        private Vector3 originalScale;
        private Vector2 originalPivot;
        private bool doneInitializing = false;

        private void Awake()
        {
            originalScale = gameObject.GetComponent<RectTransform>().localScale;
            originalPivot = gameObject.GetComponent<RectTransform>().pivot;
            Deselect();
            StartCoroutine(WaitForInitialization());
        }



        public void Select()
        {
            slotImage.color = selectedColor;
            gameObject.GetComponent<RectTransform>().localScale = originalScale * 1.2f;
            gameObject.GetComponent<RectTransform>().pivot = originalPivot + new Vector2(0,-.25f);

            if(doneInitializing)
                selectSFX.Play();
        }

        public void Deselect()
        {
            slotImage.color = notSelectedColor;
            gameObject.GetComponent<RectTransform>().localScale = originalScale;
            gameObject.GetComponent<RectTransform>().pivot = originalPivot;
        }

        // Simply returns the Invetory index this slot is at. Inventory_UI script will handle the logic of using items.
        public int UseItemInSlot()
        {
            Debug.Log("You tried to use item in Hotbar Slot #" + (slotIndex-11));
            return slotIndex;
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

        IEnumerator WaitForInitialization()
        {
            yield return new WaitForEndOfFrame();
            doneInitializing = true;
        }
    }
}