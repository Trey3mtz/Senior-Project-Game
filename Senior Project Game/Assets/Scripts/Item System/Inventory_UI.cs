using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEngine.AI;

namespace Cyrcadian.PlayerSystems.InventorySystem
{

public class Inventory_UI : MonoBehaviour
{ 
    private Inventory inventory;
    [SerializeField] public Transform itemSlotContainer;
    [SerializeField] public Transform itemSlotTemplate;

    private void Awake()
    {
            if (GameManager.Instance == null)
                Debug.Log("[Inv_UI(Awake)] Instance of gamemanager was null ");          
    }

    private void Start()
    {
        //RefreshInventoryItems();
    }

    // onInventoryChanged from the "Inventory" script subscribes to the event
    public void SetInventory(Inventory inventory)
    {          
        this.inventory = inventory;

        inventory.onInventoryChanged += Inventory_onInventoryChanged;
        RefreshInventoryItems();
    }

    // An event to be called each time an inventory update happens
    private void Inventory_onInventoryChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }


    // X and Y refer to the rows and column numbers of items in our inventory. 0,0 is the top left.
    // If X is greater than the specified number, it starts again on a new row.
    private void RefreshInventoryItems()
    {   
            foreach(Transform child in itemSlotContainer)
            {
                if(child == itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }

            int x = 0;
            int y = 0;
            float itemSlotCellSize = 137f;
  
            foreach(Inventory.InventoryEntry entry in inventory.GetInventory())
            {
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize * -1);

                Image image = itemSlotRectTransform.Find("Item Icon").GetComponent<Image>();
                image.sprite = entry.item.ItemSprite;

                TextMeshProUGUI uiText = itemSlotRectTransform.Find("Item Amount").GetComponent<TextMeshProUGUI>();
                if(entry.stackSize > 1)
                    uiText.SetText("x" + entry.stackSize.ToString());
                else
                    uiText.SetText("");

                x++;

                if(x > 3){ x = 0; y++; }
            }
    }


}
}