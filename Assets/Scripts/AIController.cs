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

    public LayerMask monsterLayer;

    float DetectionRadius { get {return monsterData.detectionRadius; } }
    float AttackRadius { get {return monsterData.attackRadius; } }
    float WanderCooldown { get { return monsterData.wanderCooldown; } }
    float WanderCooldownVariance { get { return monsterData.wanderCooldownVariance; } }
    float WanderRadius { get { return monsterData.wanderRadius; } }

    float curWanderCooldown;
    float wanderCooldownElapsed;
    float moveTimeElapsed;
    Vector2 targetWanderPos;
    Transform chaseTarget;
    AIState curState;
    MonsterData monsterData;
    MonsterControls monsterControls;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (TryGetComponent(out monsterControls))
        {
            monsterData = monsterControls.monsterData;
        }
    }
#endif

    private void Awake()
    {
        monsterControls = GetComponent<MonsterControls>();
        monsterData = monsterControls.monsterData;
    }

    private void Update()
    {
        switch (curState)
        {
            case AIState.Idle:
                Idle();
                DetectTarget();
                break;
            case AIState.Wander:
                Wander();
                DetectTarget();
                break;
            case AIState.Chase:
                Chase();
                break;
        }
    }

    void DetectTarget()
    {
        MonsterControls monster;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, DetectionRadius, monsterLayer);

        foreach (Collider2D col in cols)
        {
            if (col.gameObject != gameObject && col.TryGetComponent(out monster))
            {
                //player detected
                curState = AIState.Chase;
                chaseTarget = monster.transform;
            }
        }
    }

    void Idle()
    {
        if (wanderCooldownElapsed < curWanderCooldown)
        {
            wanderCooldownElapsed += Time.deltaTime;
            monsterControls.Move(Vector2.zero);
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
        if (Vector2.SqrMagnitude(targetWanderPos - (Vector2)transform.position) < 0.1f || moveTimeElapsed > monsterData.maxWanderTime)
        {
            // return to idle
            curState = AIState.Idle;
            monsterControls.Move(Vector2.zero);
            curWanderCooldown = WanderCooldown + Random.Range(-WanderCooldownVariance, WanderCooldownVariance);
            moveTimeElapsed = 0;
        }
        else
        {
            // move to target position
            monsterControls.Move(CalculateMovementInput(targetWanderPos));
            moveTimeElapsed += Time.deltaTime;
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
        if (chaseTarget != null)
        {
            float distance = Vector2.Distance(chaseTarget.position, transform.position);

            if (distance < AttackRadius)
            {
                // attack
                monsterControls.Attack(CalculateMovementInput(chaseTarget.position));
            }
            else if (moveTimeElapsed < monsterData.maxChaseTime)
            {
                // chase
                monsterControls.Move(CalculateMovementInput(chaseTarget.position));
                moveTimeElapsed += Time.deltaTime;
            }
            else
            {
                // stop chasing
                monsterControls.Move(Vector2.zero);
                curState = AIState.Idle;
                chaseTarget = null;
                curWanderCooldown = WanderCooldown + Random.Range(-WanderCooldownVariance, WanderCooldownVariance);
                moveTimeElapsed = 0;
            }
        }
        else
        {
            monsterControls.Move(Vector2.zero);
            curState = AIState.Idle;
            curWanderCooldown = WanderCooldown + Random.Range(-WanderCooldownVariance, WanderCooldownVariance);
            moveTimeElapsed = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (monsterData == null)
        { return; }

        // draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        // draw attack radius
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);

        // draw wander radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, WanderRadius);
    }
}
