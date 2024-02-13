using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    [Serializable]
    public struct Creature_Stats
    {
        public int healthPool;
        [HideInInspector] public int currentHP;

        [Space]
        public int stomachSize;
        [HideInInspector] public int currentHunger;

        [Space]
        public int staminaPool;
        [HideInInspector] public int currentStamina;

        [HideInInspector] public float proteinScore;
    }
}
