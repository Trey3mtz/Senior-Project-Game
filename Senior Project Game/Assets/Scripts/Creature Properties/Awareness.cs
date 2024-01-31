using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    [RequireComponent(typeof(Collider2D))]
    public class Awareness : MonoBehaviour
    {
        // keep track of single target
        private Transform m_target;
        public Transform Target
        {
            // C# public property access
            get { return m_target; }
            set { m_target = value; }
        }

        private List<Transform> VisibleCreatures;
        private List<Transform> VisibleWorldItems;
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
            // Keep track of all visible creatures!
            if(collider.gameObject.layer == 10 )
                    VisibleCreatures.Add(collider.transform);
            else if(collider.gameObject.layer == 15)
                    VisibleWorldItems.Add(collider.transform);
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            // If we lost track of a creature that was visible,  wait a moment to remove them from the stack
            if (collider.gameObject.layer == 10 && VisibleCreatures.Contains(collider.transform)) {
                if((collider.transform.position - this.gameObject.transform.position).magnitude > losingDistance)     
                    StartCoroutine(CreatureOutOfVision(collider.transform));   
            }
            else if(collider.gameObject.layer == 15 && VisibleWorldItems.Contains(collider.transform))
                {   VisibleWorldItems.Remove(collider.transform);       VisibleWorldItems.TrimExcess();}
        }

        public List<Transform> GetVisibleCreatures()
        {
            return VisibleCreatures;
        }

        public List<Transform> GetVisibleItems()
        {
            return VisibleWorldItems;
        }

        // if out of vision for too long, target is lost
        IEnumerator CreatureOutOfVision(Transform targetTransform)
        {
            yield return new WaitForSeconds(.5f);
            VisibleCreatures.Remove(targetTransform);
            VisibleCreatures.TrimExcess();
        }
    }
}
