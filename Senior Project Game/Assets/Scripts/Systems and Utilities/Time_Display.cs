using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cyrcadian.WorldTime
{
    /**************************************************************************************
    
        Description:    This script simply can display the Hour and Minute of 
                        the day. Should be used together with a canvas to show numbers.

    */

    [RequireComponent(typeof(TMP_Text))]
    
    public class Time_Display : MonoBehaviour
    {
        [SerializeField] 
        private Time_World world_time;

        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            world_time.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            world_time.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChangedDigital(object sender, TimeSpan newTime)
        {
            text.SetText(newTime.ToString(@"hh\:mm"));
        }

        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            text.SetText("Day " + newTime.Days.ToString());
        }
    
    }
}

