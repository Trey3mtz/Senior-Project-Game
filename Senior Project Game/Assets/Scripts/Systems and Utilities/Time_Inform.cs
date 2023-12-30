using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cyrcadian.WorldTime
{
    /*****************************************************************************************************
    
    Description:    This script is for activating Events at specific times of day. 
                    Through the Event System of Unity, you should be able to tell it
                    to do specific actions at specific times.

    */

    public class Time_Inform : MonoBehaviour
    {
        [SerializeField]
        private Time_World time_world;

        [SerializeField]
        private List<Schedule> schedule;

        private void Start()
        {
            time_world.WorldTimeChanged += CheckSchedule;
        }

        private void OnDestroy()
        {
            time_world.WorldTimeChanged -= CheckSchedule;
        }

        private void CheckSchedule(object sender, TimeSpan newTime)
        {
            var _schedule = 
                    schedule.FirstOrDefault(s =>
                        s.Hour == newTime.Hours &&
                        s.Minute == newTime.Minutes);

            _schedule?.action?.Invoke();    
        }

        
        [Serializable]
        private class Schedule
        {
            public int Hour;
            public int Minute;
            public UnityEvent action;
        }
    }
}

