using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;

namespace Cyrcadian.UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        public string Name;
        private float _score;

        public float score
        {
            get { return _score;}
            set
            {
                this._score = Mathf.Clamp01(value);
            }
        }

        public Consideration[] considerations;

         public virtual void Awake()
         {
            score = 0;
         }

        public abstract void Execute(CreatureController thisCreature);     

    public void AssertConsiderations(CreatureController myCreature)
    {
        float minValue = 0f; // Define the minimum acceptable value
        float maxValue = 1f; // Define the maximum acceptable value

        foreach (Consideration consideration in considerations)
        {
            float result = consideration.ScoreConsideration(myCreature);

            // Assert that the returned value falls within the specified range
            Debug.Assert(result >= minValue && result <= maxValue, $"Consideration {consideration.GetType().Name} returned value out of range: {result}");
        }
    }

    }
}

