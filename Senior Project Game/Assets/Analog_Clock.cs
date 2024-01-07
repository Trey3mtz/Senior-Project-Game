using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyrcadian.WorldTime
{
    public class Analog_Clock : MonoBehaviour
    {
        private const float minutesToDegrees = 360f / 1440f;
        
        [SerializeField]
        private Time_World world_time;

        [SerializeField] private RectTransform hand;
        [SerializeField] private Image nightBackground;
        
        // 75 degrees = 5 AM on a 24 hour clock
        float startRotation = 75;

        private void Awake()
        {   
            world_time.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            world_time.WorldTimeChanged -= OnWorldTimeChanged;
        }


            // For future use, incase we want night/day times to change given the course of seasons

            //nightBackground.fillAmount = world_time.nightDuration;
            //startRotation = world_time.sunriseHour * minutesToDegrees;
        

        // Update is called once per frame
        void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {   
            hand.rotation = Quaternion.Euler(0,0,startRotation-(newTime.Hours*60 + newTime.Minutes)*minutesToDegrees);
        }
    }
}

