using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Wander,
        Chase
    }

    [SerializeField, Expandable] MonsterData monsterData;

    float DetectionRadius { get {return monsterData.detectionRadius; } }
    float WanderCooldown { get { return monsterData.wanderCooldown; } }
    float WanderRadius { get { return monsterData.wanderRadius; } }

    float wanderCooldownElapsed;
    Vector2 targetWanderPos;
    Transform chaseTarget;
    AIState curState;
    public LayerMask monsterLayer;

    MonsterControls monsterControls;

    private void OnValidate()
    {
        if (TryGetComponent(out monsterControls))
        {
            monsterData = monsterControls.monsterData;
        }
    }

    private void Awake()
    {
        monsterControls = GetComponent<MonsterControls>();
    }

    private void Update()
    {
        switch (curState)
        {
            case AIState.Idle:
                Idle();
                DetectPlayer();
                break;
            case AIState.Wander:
                Wander();
                DetectPlayer();
                break;
            case AIState.Chase:
                Chase();
                break;
        }
    }

    void DetectPlayer()
    {
        PlayerController playerController;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, DetectionRadius, monsterLayer);

        foreach (Collider2D col in cols)
        {
            if (col.gameObject != gameObject && col.TryGetComponent(out playerController))
            {
                //player detected
                curState = AIState.Chase;
                chaseTarget = playerController.transform;
            }
        }
    }

    void Idle()
    {
        if (wanderCooldownElapsed < WanderCooldown)
        {
            wanderCooldownElapsed += Time.deltaTime;
        }
        else
        {
            wanderCooldownElapsed = 0;
            GenerateNewTargetPos();
            curState = AIState.Wander;
        }
    }

    void Wander()
    {
        if (Vector2.SqrMagnitude(targetWanderPos - (Vector2)transform.position) < 0.1f)
        {
            curState = AIState.Idle;
            monsterControls.Move(Vector2.zero);
        }
        else
        {
            monsterControls.Move(CalculateMovementInput(targetWanderPos));
        }
    }

    Vector2 CalculateMovementInput(Vector2 targetPos)
    {
        Vector2 dir = targetPos - (Vector2)transform.position;
        dir.Normalize();

        return dir;
    }

    void GenerateNewTargetPos()
    {
        targetWanderPos = Random.insideUnitCircle * WanderRadius * 0.8f;
        targetWanderPos += (targetWanderPos - (Vector2)transform.position) * 0.2f;
    }

    void Chase()
    {
        if (Vector2.SqrMagnitude((Vector2)chaseTarget.position - (Vector2)transform.position) < 3f)
        {
            monsterControls.Move(CalculateMovementInput(chaseTarget.position));
        }
        else
        {
            // stop moving
            monsterControls.Move(Vector2.zero);
            curState = AIState.Idle;
            chaseTarget = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (monsterData == null)
        { return; }

        // draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        // draw wander radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, WanderRadius);
    }
}
