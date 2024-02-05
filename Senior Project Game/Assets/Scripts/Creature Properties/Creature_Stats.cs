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
        public int currentHP;

        [Space]
        public int stomachSize;
        public int currentHunger;

        [Space]
        public int staminaPool;
        public int currentStamina;
    }
}
