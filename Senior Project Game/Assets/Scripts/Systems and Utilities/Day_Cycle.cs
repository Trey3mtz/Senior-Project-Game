using System;
using System.Collections;
using System.Diagnostics;
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

        // This is used alongside Slerp to return the Y value to be used for the sun's global light intensity. 
        private Vector3 theoreticalSunPosition;
        private Vector3 sunRise = new Vector3 (-1,0);
        private Vector3 sunSet = new Vector3 (1,0);
        private Vector3 midDay = new Vector3 (0,1);

        // i.e. Night, Day, Twilgiht
        private TimeOfDay theTimeOfDay = TimeOfDay.Night;
        
        private float dayHour = 5/24f;
        private float twilightHour = 17/24f;
        private float nightHour = 21/24f;


        [SerializeField]
        private Time_World world_time;

        [SerializeField]
        private Gradient gradient;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            world_time.WorldTimeChanged += OnWorldTimeChanged;

            theoreticalSunPosition = sunRise;
        }

        private void OnDestroy()
        {
            world_time.WorldTimeChanged -= OnWorldTimeChanged;
        }

        // This method simulates a theoretical sun's rotation around the 2D world, based on the percentage of 
        // how much time has passed in 1 day. The X-axis is the horizon, and Y-axis is the sun's height.
        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            PercentOfDay(newTime);


            if(percetageDaylight <= 0.5)
                theoreticalSunPosition = Vector3.Slerp(sunRise,midDay, PercentageBetweenValues(0, 0.5f, percetageDaylight));
            else if(percetageDaylight >= 0.5)
                theoreticalSunPosition = Vector3.Slerp(midDay,sunSet, PercentageBetweenValues(0.5f, 1, percetageDaylight));
            
            _light.intensity = theoreticalSunPosition.y;
            _light.color = gradient.Evaluate(percetageDaylight);

            
            
            if(percetageDaylight >= dayHour)
                theTimeOfDay = TimeOfDay.Day;
            if(percetageDaylight >= twilightHour)
                theTimeOfDay = TimeOfDay.Twilight;
            if(percetageDaylight > nightHour)
                theTimeOfDay = TimeOfDay.Night;

        }

        public void PercentOfDay(TimeSpan timeSpan)
        {
            percetageDaylight = (float)timeSpan.TotalMinutes % Time_Constants.MinutesInDay / Time_Constants.MinutesInDay;
        }

        private float PercentageBetweenValues(float a, float b, float relavtivePosition)
        {
            return (relavtivePosition - a)/ (b - a);
        }

        
        public float GetPercentageOfDay()
        {
            return percetageDaylight;
        }


        public int GetTimeOfDay()
        {
            return Convert.ToInt32(theTimeOfDay);
        }

        public enum TimeOfDay
        {
            Night,
            Day,
            Twilight
        }
    }
}

