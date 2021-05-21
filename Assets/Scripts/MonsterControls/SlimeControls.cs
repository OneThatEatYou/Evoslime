using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeControls : MonsterControls
{
    [Header("Attack")]
    public Vector2 attackAreaOffset;
    public float attackAreaRadius;
    public LayerMask attackLayerMask;
    Vector2 AttackAreaOrigin { get { return (Vector2)transform.position + attackAreaOffset; } } // the world position of attack area
    List<MonsterControls> damagedMonsters = new List<MonsterControls>();

    [Header("Attack Animation")]
    public float chargeTime;
    public float attackTime;
    public float dashSpeed;

    public override void Attack(Vector2 dir)
    {
        if (isAttacking)
        { return; }

        anim.SetTrigger("Attack");

        StartCoroutine(Tackle(dir));
    }

    IEnumerator Tackle(Vector2 dir)
    {
        float t = 0;

        if (dir.sqrMagnitude == 0)
        {
            dir.x = direction;
        }

        isAttacking = true;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(chargeTime);

        while (t < attackTime)
        {
            rb.velocity = dashSpeed * dir * Mathf.Pow(1 - t / attackTime, 2);
            t += Time.deltaTime;

            Collider2D[] cols = Physics2D.OverlapCircleAll(AttackAreaOrigin, attackAreaRadius, attackLayerMask);
            ProcessTargets(cols);

            yield return null;
        }

        isAttacking = false;
        damagedMonsters.Clear();
    }

    void ProcessTargets(Collider2D[] targets)
    {
        MonsterControls targetControls;

        foreach (Collider2D target in targets)
        {
            if (target.gameObject != gameObject && target.TryGetComponent(out targetControls) && !damagedMonsters.Contains(targetControls))
            {
                targetControls.TakeDamage(monsterData.damage, target.transform.position - transform.position);
                damagedMonsters.Add(targetControls);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackAreaOrigin, attackAreaRadius);
    }
}