using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Cyrcadian.WorldTime
{
    /**************************************************************************************

    Description:    This script changes the Global Light based on the percetage of day left.
                    If something else needs to be added in a gradual manner, this would be 
                    the script to modify.
    
    */

    public class Day_Cycle : MonoBehaviour
    {
        private Light2D _light;
        private float percetageDaylight;

        [SerializeField]
        private Time_World world_time;

        [SerializeField]
        private Gradient gradient;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            world_time.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            world_time.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            _light.intensity = PercentOfDay(newTime);
            //_light.color = gradient.Evaluate(percetageDaylight);
            
        }

        public float PercentOfDay(TimeSpan timeSpan)
        {
            percetageDaylight = (float)timeSpan.TotalMinutes % Time_Constants.MinutesInDay / Time_Constants.MinutesInDay;
            return percetageDaylight;
        }

        
        public float GetPercentageOfDay()
        {
            return percetageDaylight;
        }
    }
}

