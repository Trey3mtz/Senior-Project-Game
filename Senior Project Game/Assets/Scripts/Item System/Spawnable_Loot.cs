using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian.Items
{
    [Serializable]
    public struct SpawnableLoot
    {
        public Item item;
        public int lowestDropAmount;
        public int highestDropAmount;
        public float chance;
    }

    public class Spawnable_Loot : MonoBehaviour
    {

        [Header("Loot Table")]
        [Tooltip("values of 0 means it won't spawn, and 1 or greater means 100% chance to spawn")]
        [SerializeField] public SpawnableLoot[] spawnableLoot;

        // We will randomly generate a position, a force amount, and direction to push the loot
        // This will add to player satisfaction seeing it flop about as they earned the loot
        private Vector3 pushDirection;
        private float pushForce;
        private Vector3 spawningPosition;

        // These will hold a randomly generated value to compare to the chance of an item spawning, and how many of that item
        private float generatedChance;
        private int generatedAmount;

        [SerializeField] float minSpawnVelocity = 100f;
        [SerializeField] float maxSpawnVelocity = 125f;

        public void SpawnLoot()
        {       
            for(int i = 0; i < spawnableLoot.Length; i++)
            {   
                // First find out how many of this item from the loot table we want to spawn
                generatedAmount = UnityEngine.Random.Range(spawnableLoot[i].lowestDropAmount, spawnableLoot[i].highestDropAmount + 1);
                // Unlucky
                if(generatedAmount <= 0)
                    break;

                // Then, for each item we want to spawn check if we will spawn it.
                for(int j = 0; j < generatedAmount; j++)
                {
                    generatedChance = UnityEngine.Random.Range(0f, 1f);
                    if(generatedChance <= 0)
                        continue;

                    pushForce = UnityEngine.Random.Range(minSpawnVelocity, maxSpawnVelocity);
                    pushDirection = new Vector3(UnityEngine.Random.Range( -1f, 1f), UnityEngine.Random.Range( -1f, 1f)).normalized * pushForce;
                    spawningPosition = new Vector3(UnityEngine.Random.Range( -.2f, .2f), UnityEngine.Random.Range( -.2f, .2f)) + transform.position;

                    if(generatedChance <= spawnableLoot[i].chance)
                        World_Item.SpawnWorldItem(spawningPosition, spawnableLoot[i].item, pushDirection);                    
                }
            }
        }
    }
}
