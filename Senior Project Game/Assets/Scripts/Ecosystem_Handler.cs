using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    public class Ecosystem_Handler : ScriptableObject
    {
        [Serializable]
        public struct CreatureSavedData
        {
            public Creature_Stats stats;
            public Vector3 position;
        }

        private List<Creature> AllCreatures = new List<Creature>(); 

        // Subscribe to the events of a spawning/despawning creature 
        private void OnEnable()
        {
            Initialize_Creature.OnSpawn    += AddCreatureToList;
            Initialize_Creature.OnDespawn  += RemoveCreatureFromList;
        }

        private void OnDisable()
        {
            Initialize_Creature.OnSpawn    -= AddCreatureToList;
            Initialize_Creature.OnDespawn  -= RemoveCreatureFromList;
        }

        public List<Creature> GetListOfAllCreatures()
        {
            return AllCreatures;
        }

        private void AddCreatureToList(object sender, Creature creature)
        {
            AllCreatures.Add(creature);
        }

        private void RemoveCreatureFromList(object sender, Creature creature)
        {
            AllCreatures.Remove(creature);
        }
    }
}
