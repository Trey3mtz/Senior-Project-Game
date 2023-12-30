using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI
{
    public class Entity_Stats : ScriptableObject
    {
        /***********************************************************************************
            This is a list of stats/attributes used accross all entities:
                - Species name
                - Hunger stat
                - Energy stat
                - Level of aggression

                - An array of Resources listed as a viable food source for this specific species

        */
        public string Species_Name;

        public int Hunger_stat { get; set;}
        public int Energy_stat { get; set;}
        public int AggressionLevel;

        public Resource[] foodSources;
    }
}

