using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace WorldTime
{
    public class Day_Cycle : MonoBehaviour
    {
        private Light2D _light;

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

            _light.color = gradient.Evaluate(PercentOfDay(newTime));
        }

        private float PercentOfDay(TimeSpan timeSpan)
        {
            return(float)timeSpan.TotalMinutes % Time_Constants.MinutesInDay / Time_Constants.MinutesInDay;
        }
    }
}

