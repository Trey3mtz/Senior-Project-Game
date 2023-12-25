using System.Collections;
using System.Collections.Generic;
using TL.UtilityAI;
using UnityEngine;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Hunger", menuName = "UtilityAI/Consideration/Hunger")]
    public class Hunger_Consideration : Consideration
    {

         public override float ScoreConsideration()
         {
            return 0.2f;
         }
    }
}
