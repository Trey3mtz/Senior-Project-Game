using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{

    public class Testing_Inventory_Script : MonoBehaviour
    {
        [Tooltip("Scriptable object item goes here")]
        [SerializeReference] Item item;

        [Tooltip("If needed, will make this script be flexible for testing in the future")]
        //public List<Vector3> positions;
        // Start is called before the first frame update
        void Start()
        {
            World_Item.SpawnWorldItem(new Vector3(-10, 5), item);
            World_Item.SpawnWorldItem(new Vector3(-5, 15), item);
            World_Item.SpawnWorldItem(new Vector3(-10, 15), item);
        }

    }
}