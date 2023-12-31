using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{
    public class Storage
    {
        public List<Inventory.InventoryEntry> Content { get; private set; }

        public Storage()
        {
            Content = new List<Inventory.InventoryEntry>();
        }

        public void Store(Inventory.InventoryEntry entry)
        {
            //we won't have thousands of objects types stored, so there should be no performance problem on simply searching
            //for the key. But as usual : profile. If profiling show this to be massively underperformant, switch over to a
            //lookup data format like Dictionary.
            var idx = Content.FindIndex(inventoryEntry => inventoryEntry.item.Key == entry.item.Key);
            if (idx != -1)
            {
                Content[idx].stackSize += entry.stackSize;
            }
            else
            {
                Content.Add(new Inventory.InventoryEntry()
                {
                    item = entry.item,
                    stackSize = entry.stackSize
                });
            }
        }

        // Will return how much was actually retrieve (in case more than what's stored is asked)
        public int Retrieve(int contentIndex, int amount)
        {
            Debug.Assert(contentIndex < Content.Count, "Tried to retrieve a non existing entry from storage");

            int actualAmount = Mathf.Min(amount, Content[contentIndex].stackSize);

            Content[contentIndex].stackSize -= actualAmount;

            return actualAmount;
        }
    }
}
