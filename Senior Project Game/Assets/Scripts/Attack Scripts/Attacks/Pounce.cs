using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Cyrcadian.Creatures;
using Cyrcadian.UtilityAI;
using Unity.Transforms;
using UnityEngine;

namespace Cyrcadian.AttackSystem
{
    [CreateAssetMenu(fileName = "Pounce",menuName ="2D Survival/Attacks/Pounce")]
    public class Pounce : Attack
    {

        public override void DoAttack(MonoBehaviour thisMonobehaviour, Transform target)
        {
            Vector2 attackDirection = ( target.position - thisMonobehaviour.transform.position).normalized;
            Rigidbody2D rb = WhoIsAttacking.GetComponent<Rigidbody2D>();

            rb.AddForce(attackDirection * 5);
            thisMonobehaviour.GetComponentInChildren<Creature_Animation>().Attack();
        }
    }
}
