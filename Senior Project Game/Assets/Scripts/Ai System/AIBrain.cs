using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cyrcadian.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        public Action bestAction { get; set;}
        public bool isFinishedDeciding { get; set;}

        private CreatureController thisCreature;

        
        // Start is called before the first frame update
        void Start()
        {
            thisCreature = GetComponent<CreatureController>();
        }

        // Choose a best action for thisCreature from it's list of available actions
        void Update()
        {
            if(thisCreature.isDying)
                return;
                
            if( bestAction is null)
            {
                ChooseBestAction(thisCreature.actionsAvailable);
            }
        }

        // Loops through all possible actions to find the highest scoring action
        public void ChooseBestAction(Action[] actionsAvailable)
        {
            float score = 0f;
            int nextBestActionIndex = 0;
            
            for(int i = 0; i < actionsAvailable.Length; i++)
            {
                if(ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }
            //Debug.Log(" next action is " + actionsAvailable[nextBestActionIndex].name + " with score : " +actionsAvailable[nextBestActionIndex].score);
            bestAction = actionsAvailable[nextBestActionIndex];
            isFinishedDeciding = true;
        }

        // Loops through all Considerations of an action
        // Scores each Consideration
        // Average the Consideration score to get overall action score
        public float ScoreAction(Action action)
        {
            float totalConsiderationScore = 1f;
            float modificationFactor = 1 -(1f/ action.considerations.Length);

            for(int i = 0; i < action.considerations.Length; i++)
            {
                // Averaging scheme of overall score (credits to Dave Mark from GDC 2010 and his book "Behavioral Mathematics for Game AI (Applied Mathematics)")
                // Rescales the float value after the compounding floats between 0 and 1
                float considerationScore = action.considerations[i].ScoreConsideration(thisCreature);
                float makeUpValue = (1 - considerationScore) * modificationFactor;
                float finalScore = considerationScore + (makeUpValue * considerationScore);

                totalConsiderationScore *= finalScore;

                // If a consideration is zero, it has no point in computing further.
                if(totalConsiderationScore == 0)
                {
                    action.score = 0;
                    return action.score; 
                }
            }
            
            action.score = totalConsiderationScore;
            
            return action.score;
            
        }
    }
}
