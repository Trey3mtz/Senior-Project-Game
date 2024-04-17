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
            thisMonobehaviour.GetComponent<MoveController>().Dash(attackDirection, Cooldown);
            thisMonobehaviour.GetComponentInChildren<Creature_Animation>().Attack();

            GameObject myAttack = thisMonobehaviour.transform.Find("Body").Find("Mouth").Find("Pounce").gameObject;
            
            thisMonobehaviour.StartCoroutine(AttackCoroutine(myAttack, .2f));
        }

        IEnumerator AttackCoroutine(GameObject attack, float time)
        {
            attack.SetActive(true);
            yield return new WaitForSeconds(time);
            attack.SetActive(false);
        }
    }
}
