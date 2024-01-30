using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Cyrcadian.PlayerSystems.InventorySystem
{
    [Serializable]
    public class QuickSlot : MonoBehaviour 
    {
        public int slotIndex;
        // This slot is not a child of the Inventory UI, but the HUD UI. This will be set in the UI prefab
        [SerializeField] public Inventory_UI parentUI;
        [SerializeField] private AudioSource slotdropSFX;
        [SerializeField] private AudioSource selectSFX;
        [SerializeField] AudioClip slotDropFX;
        [SerializeField] AudioClip selectFX;

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
                //selectSFX.Play();
                AudioManager.Instance.PlaySoundFX(selectFX);
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
            return slotIndex;
        }


        IEnumerator WaitForInitialization()
        {
            yield return new WaitForEndOfFrame();
            doneInitializing = true;
        }

         // need to change index and parent after drag
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