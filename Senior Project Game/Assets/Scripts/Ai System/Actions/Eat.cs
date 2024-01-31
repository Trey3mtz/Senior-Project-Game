using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
    public class Eat : Action
    {
        public int foodValue;
        public override void Execute(CreatureController thisCreature)
        {
           thisCreature.stats.currentHunger += foodValue; // Place logic to sleep here
        }
    }
}