using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Hunt", menuName = "UtilityAI/Actions/Hunt")]
    public class Hunting : Action
    {
        public override void Execute(CreatureController thisCreature)
        {

            thisCreature.DoChase(thisCreature.awareness.FindTastiestCreature(thisCreature));
        }
    }
}

