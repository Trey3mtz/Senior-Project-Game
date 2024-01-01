using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
    public class Eat : Action
    {
        public override void Execute(EntityController thisEntity)
        {
           // thisEntity.stats.hunger -= 1;// Place logic to sleep here
        }
    }
}