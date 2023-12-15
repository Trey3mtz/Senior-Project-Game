using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TL.Core
{
    public class AIBrain : MonoBehaviour
    {
        public Action bestAction { get; set;}
        private EntityController npc;

        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<EntityController>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ChooseBestAction(Action[] actionsAvailable)
        {
            
        }

        public void ScoreAction(Action action)
        {

        }
    }
}
