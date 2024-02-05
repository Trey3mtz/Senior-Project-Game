using System.Collections;
using DG.Tweening;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Cyrcadian.UtilityAI
{
    public class MoveController : MonoBehaviour
    {
    
    /// <summary>
    ///                     This script will contain any logic regarding the manipulation of a creature's NavMeshAgent.
    ///                     
    ///                     Awareness, a script and collider which manages detection of things in a radius, is set
    ///                     in the creature's controller script, and located on the prefab of a creature.
    ///                     
    ///                     The decision to manipulate a NavMeshAgent is logically handled in the CreatureController script, where
    ///                     it has access to both this MoveController script and the Awareness script.
    ///                     The decision to move towards a specific creature it can see in it's awareness range is an example of what
    ///                     is handled in the CreatureController. Once a decision is made by the brain, it will call this script.                  
    /// </summary>
    
    
        public NavMeshAgent agent { get; private set; }
        
      
        [Tooltip("This indicates what will block your vision")] 
        [SerializeField] LayerMask layerMask;

        private float originalSpeed, originalAcceleration;


        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            originalSpeed = agent.speed;
            originalAcceleration = agent.acceleration;
            
        }

        // Update is called once per frame
        void Update()
        {
            if(agent.isPathStale)
                agent.path = null;
        }

        public void MoveTo(Vector2 position)
        {
            agent.destination = position;
        }

            // Speed/Acceleration changes value by a factor of itself, from a max of double it's original values or making them zero.
            // Will only accept values 0 through 1, and clamping the rest.
            public void IncreaseMoveSpeed(float addedSpeed)
            {
                addedSpeed = originalSpeed * (1 + Mathf.Clamp01(addedSpeed));
                DOTween.To(() => agent.speed, x => agent.speed = x, addedSpeed, 1f);
            }

            public void IncreaseAcceleration(float addedAcceleration)
            {
                addedAcceleration = originalAcceleration * (1 + Mathf.Clamp01(addedAcceleration));
                DOTween.To(() => agent.acceleration, x => agent.acceleration = x, addedAcceleration, 1f);
            }

                public void DecreaseMoveSpeed(float loweredSpeed)
                {
                    loweredSpeed = originalSpeed * (1 -  Mathf.Clamp01(loweredSpeed));
                    DOTween.To(() => agent.speed, x => agent.speed = x, loweredSpeed, 1f);
                }

                public void DecreaseAcceleration(float loweredAcceleration)
                {
                    loweredAcceleration = originalAcceleration * (1 - Mathf.Clamp01(loweredAcceleration));
                    DOTween.To(() => agent.acceleration, x => agent.acceleration = x, loweredAcceleration, 1f);
                }
                    // The last methods of changing speed/acceleration, will reset to their original values
                    public void ResetSpeed()
                    {
                        DOTween.To(() => agent.speed, x => agent.speed = x, originalSpeed, 1f);
                    }

                    public void ResetAcceleration()
                    {
                        DOTween.To(() => agent.acceleration, x => agent.acceleration = x, originalAcceleration, 1f);
                    }

        public bool CanSeeTarget()
        {
            return !Physics2D.Raycast(transform.position, (agent.destination - transform.position).normalized, agent.remainingDistance, layerMask);
        }

        public void BrieflyPauseMove(float value)
        {    StartCoroutine(PauseMovement(value));    }

        IEnumerator PauseMovement(float value)
        {
            agent.isStopped = true;
            yield return new WaitForSeconds(value);
            agent.isStopped = false;
        }

        public void StopMoving()
        {    agent.ResetPath();    }


        public void MoveToRandomPoint(float range)
        {
            Vector3 point;

            // If I successfully find a point, move to it, else try again
            if(RandomPoint(range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }
            else
                MoveToRandomPoint(range);
        }
        
        bool RandomPoint(float range, out Vector3 result)
        {
            
            Vector3 center = transform.position;
            Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
            { 
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

    }
}

