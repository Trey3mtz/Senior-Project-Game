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
        public int stomachSize;

        public int currentHP;
        public int currentHunger;

        public int energy;
    }
}
