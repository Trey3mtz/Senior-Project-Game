using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
    public class Sleep : Action
    {
        public override void Execute(EntityController thisEntity)
        {
            thisEntity.DoSleep(3);// Place logic to sleep here
        }
    }
}

