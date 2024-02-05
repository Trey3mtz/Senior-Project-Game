using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Idle", menuName = "UtilityAI/Actions/Idle")]
    public class Idle : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
            thisCreature.DoIdle(5);
        }
    }
}
