using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Flee", menuName = "UtilityAI/Actions/Flee")]
    public class Flee : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
           // thisCreature.stats.hunger -= 1;// Place logic to sleep here
        }
    }
}