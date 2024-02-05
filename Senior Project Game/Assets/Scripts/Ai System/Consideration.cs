using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        public string Name;

        private float _score;

        //                          A score of 0 means it has the lowest priority.
        //                          A score of 1 means it has the highest priority.
        public float score
        {
            get { return _score;}
            set
            {
                this._score = Mathf.Clamp01(value);
            }
        }

        public virtual void Awake()
        {
            score = 0;
        }

        public abstract float ScoreConsideration(CreatureController entityController);
        
    }
}

