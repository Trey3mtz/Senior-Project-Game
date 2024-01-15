using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Generic Item", menuName = "2D Survial/Items/Generic Item")]
    public class Generic_Item : Item
    {
        // This item class is for items that do not need sounds, or methods.
        // Their only purpose is to be collected as a World_Item, and stored in inventory.

        public override bool CanUse(Vector3Int target)
        {
           return false;
        }

        public override bool Use(Vector3Int target, GameObject gameObject)
        {
            return false;
        }
    }
}
