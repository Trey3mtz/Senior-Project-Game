using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TL.UtilityAI
{
    public class EntityController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
        }


    }
}

