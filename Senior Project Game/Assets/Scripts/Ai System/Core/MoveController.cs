using System.Collections;
using NavMeshPlus.Extensions;
using TMPro;
using TreeEditor;
using Unity.Burst;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

namespace Cyrcadian.UtilityAI
{
    [BurstCompile]
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
        private NavMeshPath path;
        private int stepIndex = 0;

        // all agents can set move speed in inspector
        [SerializeField] float ACCELERATION = 15f; // Adjust as needed
        public Vector3 moveDirection{get; private set;}
        [HideInInspector]public bool canMove = true;
        
        
        // method to return move speed provides a central place
        //  to implement movement speed modifiers
        public float GetAcceleration() { return ACCELERATION; }
        public AnimationCurve responseCurve;

        // Not used to navigate, but for information and for certain actions 
        [HideInInspector] public Rigidbody2D rb;
        
      
        [Tooltip("This indicates what will block your vision")] 
        [SerializeField] LayerMask layerMask;

        private float originalSpeed, originalAcceleration;
        

        void Awake()
        {
            moveDirection = new();
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            rb = GetComponent<Rigidbody2D>();            
        }
        
        void Start()
        {
            originalSpeed = agent.speed;
            originalAcceleration = agent.acceleration;   

            path = new NavMeshPath();
            StartCoroutine(slowdebuglog());
        }
   
    IEnumerator slowdebuglog()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(.25f);
            Debug.Log("Path length : " + path.corners.Length);
        }
    }
    
        void FixedUpdate()
        {    // Using Euler's number for fun. Not significant.                                           
            rb.AddForce(moveDirection * ACCELERATION * 2.718f);
        }
        
        void Update()
        {
            if(canMove)
            {
                // If no path or path is done, clear move direction.
                if(!UpdatePath())
                    moveDirection = new();
            }
            else
                moveDirection = new();
        }

        public bool UpdatePath()
        {
            // no path or path is finished
            if(stepIndex < path.corners.Length)
            {
                Vector2 nextWaypoint = path.corners[stepIndex];
                moveDirection = (nextWaypoint - (Vector2)transform.position).normalized;             
                


                // draw path in scene for debugging
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
                    

                if(QuickMafs.DistanceLength(transform.position, nextWaypoint) < 0.1f)
                    stepIndex++;
                
                
                // always succeeds if we have a path
                return true;
            }
            
            return false;
        }

        public bool UpdatePath(Vector3 destination)
        {
            // if we have no path or it is a new endpoint, calculate a new path to it
            if (path.corners.Length == 0 || (Vector2)path.corners[path.corners.Length-1] != (Vector2)destination)
            {
                if (!UnityEngine.AI.NavMesh.CalculatePath((Vector2)transform.position, destination, UnityEngine.AI.NavMesh.AllAreas, path)) {
                    // no path found
                    return false;
                }
                // path corner[0] is the starting point, first waypoint is corner[1]
                stepIndex = 1;
            }

            return UpdatePath();
        }

        // called to clear current path and stop ongoing movement
        public void StopPathMove() { path.ClearCorners(); }



      
        public bool IsMoving()
        {
            return moveDirection != new Vector3();   
        }



    

            // Speed/Acceleration changes value by a factor of itself, from a max of double it's original values or making them zero.
            // Will only accept values 0 through 1, and clamping the rest.
            public void IncreaseMoveSpeed(float targetSpeed)
            {
                targetSpeed = originalSpeed * (1 + Mathf.Clamp01(targetSpeed));
                StartCoroutine(ChangeSpeed(targetSpeed, 0.5f));
            }

            IEnumerator ChangeSpeed(float targetSpeed, float timeDuration)
            {
                float elapsedTime = timeDuration;
                float startingSpeed = agent.speed;

                // Smoothly change speed based on floats and ratios
                while(elapsedTime > 0)
                {
                    yield return new WaitForEndOfFrame();

                    float ratio = Mathf.Clamp01(1 - (elapsedTime / timeDuration));
                    ratio = responseCurve.Evaluate(ratio);
                    
                    agent.speed = Mathf.Lerp(startingSpeed, targetSpeed, ratio);
             
                    elapsedTime -= Time.deltaTime;
                }
                // Snap remainder of the speed straight to the target.
                agent.speed = targetSpeed;
            }

            public void IncreaseAcceleration(float addedAcceleration)
            {Debug.Log(" WARNING WARNING WARNING ");
                addedAcceleration = originalAcceleration * (1 + Mathf.Clamp01(addedAcceleration));
                agent.acceleration = addedAcceleration;
            }

                public void DecreaseMoveSpeed(float targetSpeed)
                {Debug.Log(" WARNING WARNING WARNING ");
                    targetSpeed = agent.speed * (1 -  Mathf.Clamp01(targetSpeed));
                    StartCoroutine(ChangeSpeed(targetSpeed, 0.5f));
                }

                public void DecreaseAcceleration(float loweredAcceleration)
                {Debug.Log(" WARNING WARNING WARNING ");
                    loweredAcceleration = originalAcceleration * (1 - Mathf.Clamp01(loweredAcceleration));
                    agent.acceleration = loweredAcceleration;
                }
                    // The last methods of changing speed/acceleration, will reset to their original values
                    public void ResetSpeed()
                    {Debug.Log(" WARNING WARNING WARNING ");
                        agent.speed = originalSpeed;
                    }

                    public void ResetAcceleration()
                    {Debug.Log(" WARNING WARNING WARNING ");
                        agent.acceleration = originalAcceleration;
                    }
       
        public bool CanSeeTarget()
        {
            return !Physics2D.Raycast(transform.position, 
                                     (path.corners[path.corners.Length] - transform.position).normalized, 
                                      QuickMafs.DistanceLength(path.corners[path.corners.Length], transform.position), 
                                      layerMask);
        }

        public void BrieflyPauseMovement(float value)
        {    StartCoroutine(PauseMovement(value));    }

        IEnumerator PauseMovement(float value)
        {Debug.Log(" WARNING WARNING WARNING ");
            agent.isStopped = true;
            yield return new WaitForSeconds(value);
            agent.isStopped = false;
        }

        public void ResetPath()
        { Debug.Log(" WARNING WARNING WARNING ");   agent.ResetPath();    }

        [BurstCompile]
        public bool IsAtDestination()
        {Debug.Log(" WARNING WARNING WARNING ");
            return agent.remainingDistance <= agent.stoppingDistance;
        }
 
        [BurstCompile]
        public void MoveToRandomPoint(float range)
        {
            Vector3 randomPoint;

            // If I successfully find a point, move to it, else try again
            if(RandomPoint(range, out randomPoint))
            {   Debug.DrawRay(transform.position, randomPoint-transform.position, Color.green, 0.5f);
                UpdatePath(randomPoint);
            }
            else
                MoveToRandomPoint(range);
        }
        
        [BurstCompile]
        public bool RandomPoint(float range, out Vector3 result)
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

        [BurstCompile]
        // Drained stamina affects speed between 50% and 10% of your stamina
        public bool DrainingStamina(CreatureController thisCreature)
        {
            if(thisCreature.stats.currentStamina > thisCreature.stats.staminaPool * 0.5f || thisCreature.stats.currentStamina < thisCreature.stats.staminaPool * 0.1f)
                return false;
            
            DecreaseMoveSpeed(thisCreature.stats.currentStamina / (thisCreature.stats.staminaPool * 0.1f));

            return true;
        }


        //private Vector3 dashDestination;
        [SerializeField] float dashSpeedFactor = 2;
        private bool canDash = true;

        [BurstCompile]
        public void Dash(Vector3 dashDirection, float dashDistance, float dashCooldown)
        {
            if (canDash)
            {
                //dashDestination = transform.position + dashDirection.normalized * dashDistance;
                //agent.velocity = agent.velocity * dashSpeedFactor;
                 rb.AddForce(dashDirection * dashDistance *dashSpeedFactor);
                canDash = false;
                StartCoroutine(StartDashCooldown(dashCooldown));
            }
        }

        IEnumerator StartDashCooldown(float dashCooldown)
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}

