using System.Collections;
using System.Collections.Generic;
using Cyrcadian.Creatures;
using UnityEngine;


namespace Cyrcadian.UtilityAI
{
    public class CreatureController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;
        public Creature_Stats stats;

        // Keeps track of nearby Creatures, and Items so far.
        private Awareness awareness;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
            awareness = GetComponentInChildren<Awareness>();
        }


        // If the brain had finished choosing a best action, Execute that action.
        // Feeds "this" specific entity into the execute method.
        void Update()
        {
        if(aiBrain.isFinishedDeciding)
        {
            aiBrain.isFinishedDeciding = false;
            aiBrain.bestAction.Execute(this);
        }
        }

            // Upon completing an Action, choose the next best action from all available actions
            public void UponCompletedAction()
            {
                aiBrain.ChooseBestAction(actionsAvailable);
            }



        #region Coroutine
        /*****************************************************************************************************************
            This region contains the code and logic for all actions that will take Real Time to complete. 
            This gives room for animations to play.
            This also makes sense from a realism perspective.
        */

        public void DoSleep(float time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        IEnumerator SleepCoroutine(float time)
        {
            float counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1f);
                counter--;
            }

            UponCompletedAction();
        }

        #endregion
    }
}

