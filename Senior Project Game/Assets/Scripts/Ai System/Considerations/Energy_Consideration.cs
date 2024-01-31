using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Energy", menuName = "UtilityAI/Consideration/Energy")]
    public class Energy_Consideration : Consideration
    {

         public override float ScoreConsideration(CreatureController thisCreature)
         {
            return 0.6f;
         }
    }
}