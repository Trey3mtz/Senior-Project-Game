using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    [RequireComponent(typeof(Collider2D))]
    public class Awareness : MonoBehaviour
    {
        // Target a specific thing in our list of visible stuff
        private Transform m_target;
        public Transform Target
        {
            // C# public property access
            get { return m_target; }
            set { m_target = value; }
        }

        [HideInInspector] public List<Transform> VisibleCreatures { get; private set; }
        [HideInInspector] public List<Transform> VisibleWorldItems { get; private set; }

        [SerializeField] float losingDistance = 5f; 
        
        void Awake()
        {
            VisibleCreatures = new List<Transform>();
            VisibleWorldItems = new List<Transform>();
        }

        //                                          IMPORTANT NOTE:     layer 10 is "Creature", so we are checking if they are creatures
        //                                                              layer 15 is dropped items

        void OnTriggerEnter2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;
            // Keep track of all visible creatures!
            if(collider.gameObject.layer == 10 )
                    VisibleCreatures.Add(collider.transform);
            else if(collider.gameObject.layer == 15)
                    VisibleWorldItems.Add(collider.transform);
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;

            // If we lost track of a creature that was visible,  wait a moment to remove them from the stack
            if (collider.gameObject.layer == 10 && VisibleCreatures.Contains(collider.transform)) {
                if((collider.transform.position - this.gameObject.transform.position).magnitude > losingDistance)     
                    StartCoroutine(CreatureOutOfVision(collider.transform));   
            }
            else if(collider.gameObject.layer == 15 && VisibleWorldItems.Contains(collider.transform))
                {   VisibleWorldItems.Remove(collider.transform);       VisibleWorldItems.TrimExcess();}


            if(collider.transform.root == Target)
                StartCoroutine(TargetOutOfVision());
        }
        // If Target stepped out of vision, wait for 1 seconds and check if they are in the list still. 
        // If Target is in list of visible creatures, try again Until they leave.
        IEnumerator TargetOutOfVision()
        { 
            yield return new WaitForSeconds(1f);
            if(!VisibleCreatures.Contains(Target))
                Target = null;
            else if(Mathf.Abs((Target.position - transform.position).sqrMagnitude) > losingDistance*2)
            {   
                VisibleCreatures.Remove(Target);
                Target = null;
            }
            else
                StartCoroutine(TargetOutOfVision());
        }
        // If creature is out of vision for too long, no longer can see them
        IEnumerator CreatureOutOfVision(Transform targetTransform)
        {
            yield return new WaitForSeconds(.5f);
            VisibleCreatures.Remove(targetTransform);
            VisibleCreatures.TrimExcess();
        }


        public bool IsTargetInVision()
        {

            if(Target)
                return true;
            else
                return false;
        }
    }
}
