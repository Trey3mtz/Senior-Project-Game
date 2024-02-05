using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Become Alert", menuName = "UtilityAI/Actions/Become Alert")]
    public class BecomeAlert : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
           thisCreature.DoAlert();
        }
    }
}