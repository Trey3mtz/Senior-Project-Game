using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian.PlayerSystems.InventorySystem
{

    public class Collect_World_Item : MonoBehaviour
    {
        [SerializeField]
        private Inventory inventory;
        [SerializeField] AudioSource itemSFX; 
        [SerializeField] PointEffector2D itemPull;

        // Player Controller sets the inventory and calls this to set it,
        //      so that the items collected go to the player's inventory
        public void SetInventory(Inventory inventory)
        {
            this.inventory = inventory;
            if(!itemPull)
                itemPull = GetComponent<PointEffector2D>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            
            World_Item item = collider.GetComponent<World_Item>();
            if(item != null)
            {
                itemSFX.Play();
                if(inventory.AddItem(item.GetItem(), item.GetAmount()))
                    {item.DestroySelf();} 
                else
                {
                   Debug.Log("Inventory is too full for this item");
                   collider.enabled = false;
                   StartCoroutine(WaitToTryAgain(collider));
                }
            }
        }

        private IEnumerator WaitToTryAgain(Collider2D collider)
        {
            yield return new WaitForSeconds(2);
            collider.enabled = true;
        }
    }
}