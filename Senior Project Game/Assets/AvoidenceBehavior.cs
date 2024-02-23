using System.Collections;
using System.Collections.Generic;
using Cyrcadian.UtilityAI;
using UnityEngine;

public class AvoidanceBehavior : MonoBehaviour
{
    public LayerMask creatureLayer;
    public float avoidanceDistance = .4f;
    public float sidestepDistance = 1f;

    private MoveController aiMover;
    private Transform targetCreature;

    void Start()
    {
        aiMover = GetComponent<MoveController>();
    }

    void Update()
    {
        CheckForCloseCreatures();
        AvoidCloseCreature();
    }

    void CheckForCloseCreatures()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, avoidanceDistance, creatureLayer);

        if (hitColliders.Length > 0)
        {
            foreach (Collider2D collider in hitColliders)
            {
                if (collider.transform.root != transform) // Check if the collider is not part of the same GameObject
                {
                    targetCreature = collider.transform;
                    break;
                }
            }
        }
        else
        {
            targetCreature = null;
        }
    }

    float sidestepAngleRange = 45f;
    void AvoidCloseCreature()
    {
        if (targetCreature != null)
        {
            Vector2 avoidanceDirection = (Vector2)transform.position - (Vector2)targetCreature.position;
            float randomAngle = Random.Range(-sidestepAngleRange, sidestepAngleRange);
            Vector2 sidestepDirection = Quaternion.Euler(0, 0, randomAngle) * avoidanceDirection.normalized;
            Vector2 newDestination = (Vector2)transform.position + sidestepDirection * sidestepDistance;
            Debug.Log("Moving away" );
            aiMover.MoveTo(newDestination);
            targetCreature = null;
        }
    }
}