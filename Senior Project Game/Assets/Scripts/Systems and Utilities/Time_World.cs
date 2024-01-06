using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.WorldTime
{

    /****************************************************************************************

        Description:    This script continually adds time to a counter or clock. 
                        This is the basis for the world's time of day function.
    
                        Uses struct TimeSpan instead of a float for future functionality with tracking Days.
    */

    public class Time_World : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;
 
        [SerializeField]
        private float day_length;   //measured in seconds

        private TimeSpan current_time;
        private float minute_length => day_length/Time_Constants.MinutesInDay;

        private void Awake()
        {
            GameManager.Instance.Time_World = this;
        }
        private void Start()
        {
            StartCoroutine(AddMinute());
        }

        private IEnumerator AddMinute()
        {
            current_time += TimeSpan.FromMinutes(1);
            WorldTimeChanged?.Invoke(this, current_time);
            yield return new WaitForSeconds(minute_length);
            StartCoroutine(AddMinute());
        }

        public void Save(ref WorldTimeSaveData data)
        {
            data.TimeOfTheDay = current_time;
        }
        
        public void Load(WorldTimeSaveData data)
        {
            current_time = data.TimeOfTheDay;
            //StartingTime = m_CurrentTimeOfTheDay;
        }    

    }
    public struct WorldTimeSaveData
    {
        public TimeSpan TimeOfTheDay;
    }
}
