using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TL.UtilityAI
{
    public class EntityController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
        }

        #region Coroutine

        public void DoSleep(float time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        IEnumerator SleepCoroutine(float time)
        {
            float counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1f);
                counter--;
            }
        }

        #endregion
    }
}

