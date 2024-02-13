using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
            if(thisCreature.isDying || GameStateManager.IsPaused())
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
            {   //Debug.Log("Action '" + actionsAvailable[i].name + "' with score of : " + ScoreAction(actionsAvailable[i]));
                if(ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }
            Debug.Log(" next action is " + actionsAvailable[nextBestActionIndex].name + " with score : " +actionsAvailable[nextBestActionIndex].score + " from creature " + transform);
            bestAction = actionsAvailable[nextBestActionIndex];
            isFinishedDeciding = true;
        }

        // Below is a modified version of the commented out code block of ScoreAction() to use Unity's Job System for multithreaded performance
        // Leaving the old version behind for reference
        private List<float> tempScoreList;
        public float ScoreAction(Action action)
        {
            if(action.considerations.Length == 0)
                return 0f;
            
            tempScoreList = new List<float>();

            for(int i = 0; i < action.considerations.Length; i++)
                tempScoreList.Add(action.considerations[i].ScoreConsideration(thisCreature));

                NativeArray<float> Temp = new NativeArray<float>(tempScoreList.Count, Allocator.Persistent);
                Temp.CopyFrom(tempScoreList.ToArray());

            ScoreActionJob _scoreJob = new ScoreActionJob(){
                modificationFactor = 1 - (1.0f / action.considerations.Length),
                ConiderationScores = Temp,
                ArrayLength = action.considerations.Length,
                FinalActionScore = new NativeArray<float>(1, Allocator.Persistent)
            };
            

            ScoringJobHandle = _scoreJob.Schedule();
            ScoringJobHandle.Complete();
            
            action.score = _scoreJob.FinalActionScore[0];

            Temp.Dispose();
            
            return action.score;
        }


        /*
        // Loops through all Considerations of an action
        // Scores each Consideration
        // Average the Consideration score to get overall action score
        public float ScoreAction(Action action)
        {
            float totalConsiderationScore = 1f;
            float modificationFactor = 1 -(1.0f/ action.considerations.Length);

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
            Debug.Log("totalConsiderationScore is " + totalConsiderationScore + " for action : " + action.name);
            action.score = totalConsiderationScore;
            
            return action.score;
            
        }
        */

        JobHandle ScoringJobHandle;
        [BurstCompile]
        public struct ScoreActionJob : IJob
        {
            [ReadOnly] public float modificationFactor;
            [ReadOnly] public NativeArray<float> ConiderationScores;
            [ReadOnly] public int ArrayLength;
        
            public NativeArray<float> FinalActionScore;

            public void Execute()
            {
                float totalConsiderationScore = 1f;

                for(int i = 0; i < ArrayLength; i++)
                {

                    float makeUpValue = (1 - ConiderationScores[i]) * modificationFactor;
                    float finalScore = ConiderationScores[i] + (makeUpValue * ConiderationScores[i]);

                    totalConsiderationScore *= finalScore;

                    // If a consideration is zero, it has no point in computing further.
                    if(totalConsiderationScore == 0)
                    {
                        FinalActionScore[0] = 0;
                        break;
                    }
                }

                FinalActionScore[0] = totalConsiderationScore;
            }
        }
    }
}
