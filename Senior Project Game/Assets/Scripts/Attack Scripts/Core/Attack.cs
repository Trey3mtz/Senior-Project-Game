using Unity.Collections;
using Cyrcadian.AttackSystem;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.VisualScripting;
using Unity.Entities.UniversalDelegates;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Cyrcadian.UtilityAI;

namespace Cyrcadian
{
    public abstract class Attack : ScriptableObject
    {

        void OnEnable()
        {
            AttackPrefab.GetComponent<Damage_HealthBar>().damage = DamageValue;
            allowedToAttack = true;
        }




#region Internal Timer Logic
        
        
        // Called when TryAttacking realizes it is allowed to attack. (Internal only)
        // Pass in the MonoBehaviour that called this attack to have it start a coroutine for this Scriptable Object
        private void StartAttackTimer(MonoBehaviour thisMono)
        {
            allowedToAttack = false;

            thisMono.StartCoroutine(UpdateAttackTimer());
        }

        public IEnumerator UpdateAttackTimer()
        {   
            float timer = Cooldown;
            while(timer > 0f)
            {
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;              
            }

            // timer is complete
            allowedToAttack = true;
        }


#endregion


        // Names must be unique in order for creature's to correctly look up the name for their attack
        [Tooltip("Name used for the Attack must be Unique")]
        public string UniqueName = "DefaultName";

        // Once we TryAttacking, you must StartAttackTimer to know if you can do an attack.
        public bool allowedToAttack { get; private set; }
        
        // Trys to find a HealthBar to attack, damage value should be given to DamageLogic.
        private Damage_HealthBar DamageLogic;
        [Tooltip("This value will attach itself to the DamageLogic, to hurt a healthbar if found.")]
        public int DamageValue;

        [Tooltip("CanAttack will only be allowed to return true, if something is in the Range of attacking")]
        public float Range = 1.5f;

        [Tooltip("How long one should wait before being able to do this attack again")]
        public float Cooldown = 1f;


        [Tooltip("This is the attacking entity, will be set at Runtime")]
        public Transform WhoIsAttacking {get; private set;}
        [Tooltip("Who we are trying to attack, will be set at Runtime")]
        public Transform WhatIsBeingAttacked {get; private set;}

        [Tooltip("This is the prefab that will make itself the child of whomever tries attacking")]
        public GameObject AttackPrefab;


        [Tooltip("Sounds triggered when doing an attack")]
        public AudioClip[] AttackSound;
        public Vector2 volume = new Vector2(0.5f, 0.5f);
        public Vector2 pitch = new Vector2(1,1);


        
        // Will let us know if we can attack, using the job system to calculate range difference
        public virtual bool CanAttack()
        {   
           
#region logic for CanAttack()
            // Set up our JOB
            NativeArray<bool> tempBoolArray = new NativeArray<bool>(1, Allocator.TempJob); 
            CalculateRangeDifference _rangeCalculationJob = new CalculateRangeDifference(){
                attacker = WhoIsAttacking.position,
                victim = WhatIsBeingAttacked.position,
                range = Range,
                isInRange = tempBoolArray
            };

            rangeJobHandle = _rangeCalculationJob.Schedule();

            // Execute JOB on multi-thread
            rangeJobHandle.Complete();
            bool isInRange;
            isInRange = _rangeCalculationJob.isInRange[0];
            
            tempBoolArray.Dispose();
#endregion            
            return isInRange && allowedToAttack;
        }

        // This is the method to be called by something wanting to Attack another Transform
        // Returns true if they tried an attack and false if they could not attack (either not in range or attack timer not allowing it)
        public bool TryAttacking(MonoBehaviour thisMono, Transform target)
        {
            WhatIsBeingAttacked = target;
            WhoIsAttacking = thisMono.transform;
           
            if(CanAttack())
            {   
                StartAttackTimer(thisMono);
                DoAttack(thisMono, target);
                // We performed an attack!
                return true;
            }
            else
                // We did not perform an attack.
                return false;
        }

        // Houses logic for the some abstract Attack. Will be defined by a specific attack.
        public abstract void DoAttack(MonoBehaviour mono, Transform target);


        // simply checks if in range for this attack
        public virtual bool IsInAttackRange(CreatureController creatureController, Transform target)
        {   
            WhoIsAttacking = creatureController.transform; 
            WhatIsBeingAttacked = target;
#region logic for CanAttack()
            // Set up our JOB
            NativeArray<bool> tempBoolArray = new NativeArray<bool>(1, Allocator.TempJob); 
            CalculateRangeDifference _rangeCalculationJob = new CalculateRangeDifference(){
                attacker = WhoIsAttacking.position,
                victim = WhatIsBeingAttacked.position,
                range = Range,
                isInRange = tempBoolArray
            };

            rangeJobHandle = _rangeCalculationJob.Schedule();

            // Execute JOB on multi-thread
            rangeJobHandle.Complete();
            bool isInRange;
            isInRange = _rangeCalculationJob.isInRange[0];
            
            tempBoolArray.Dispose();
#endregion            
            return isInRange;
        }


        // A method to play audioclips
        public void PlaySound()
        {
            if(AttackSound.Length == 0)
            {
                Debug.LogWarning($"Missing sound clips for item {UniqueName}");
                return;
            }

            // Sends all audioclips to master audio
            foreach(AudioClip clip in AttackSound)
            {   AudioManager.Instance.PlaySoundFX(clip);   }
        }

        // A method to play audioclips, with different volume
        public void PlaySound(float volume)
        {
            if(AttackSound.Length == 0)
            {
                Debug.LogWarning($"Missing sound clips for item {UniqueName}");
                return;
            }

            // Sends all audioclips to master audio
            foreach(AudioClip clip in AttackSound)
            {   AudioManager.Instance.PlaySoundFX(clip, volume);   }
        }

        // Multi-threaded calculation of checking a range
        private JobHandle rangeJobHandle;
        [BurstCompile]
        public struct CalculateRangeDifference : IJob
        {
            public float3 attacker;
            public float3 victim;
            public float range;

            public NativeArray<bool> isInRange;

            public void Execute()
            {
                isInRange[0] = math.lengthsq(victim - attacker) <= range*range;
            }
        }






    }
}
