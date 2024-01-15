using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{
    public class Mineable_Resource : MonoBehaviour
    {
        [SerializeField] Item item;
        [SerializeField] Transform spawnpoint;
        [SerializeField] string correctTool;

        void Awake()
        {
            spawnpoint = gameObject.transform;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.gameObject.tag == correctTool)
                World_Item.SpawnWorldItem(spawnpoint.position, item);
        }
    }
}
