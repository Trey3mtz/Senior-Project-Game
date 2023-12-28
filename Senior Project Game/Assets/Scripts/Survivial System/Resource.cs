using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.UtilityAI
{
    // These are the resource types inside of the game, remove or add more as needed.

    // Resources are the RAW materials of this game's world. This is different from Items.
    public enum resource_type
    {
        Wood,
        Stone,
        Iron,
        Grass,
        RawMeat,
        Fish
    }


    public class Resource : ScriptableObject
    {
        public string Name;

        public resource_type type;

        public SpriteRenderer sprite;
    }
}

