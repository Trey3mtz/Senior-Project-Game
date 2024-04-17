using System.Collections;
using System.Collections.Generic;
using Cyrcadian.Creatures;
using UnityEngine;

namespace Cyrcadian
{
    public class TEST_SpawnCreature : MonoBehaviour
    {

        [SerializeField] Creature creature;

        // Start is called before the first frame update
        void OnEnable()
        {
            Initialize_Creature.SpawnCreature(transform.position, creature);
        }

    }
}
