using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeControls : MonsterControls
{
    [Header("Attack")]
    public float attackAreaRadius;
    public float dashSpeed = 10;
    public LayerMask attackLayerMask;

    [Header("Attack Animation")]
    public float chargeTime;
    public float attackTime;

    [Header("Attack SFX")]
    public AudioClip tackleSFX;
    public AudioClip hitSFX;

    List<MonsterControls> damagedMonsters = new List<MonsterControls>();
    Coroutine tackleCR;

    public override void Attack(Vector2 dir)
    {
        base.Attack(dir);

        if (isAttacking || isKnockingBack)
        { return; }

        anim.SetTrigger(animAttackParam);

        tackleCR = StartCoroutine(Tackle(dir));
    }

    public override void CancelAttack()
    {
        if (!isAttacking)
        { return; }

        StopCoroutine(tackleCR);
        damagedMonsters.Clear();
        anim.Play(animIdleState, 0);

        isAttacking = false;
        rb.velocity = Vector2.zero;
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

        AudioManager.PlayAudioAtPosition(tackleSFX, transform.position, AudioManager.combatSfxMixerGroup);

        while (t < attackTime)
        {
            rb.velocity = dashSpeed * dir * Mathf.Pow(1 - t / attackTime, 2);
            t += Time.deltaTime;

            Collider2D[] cols = Physics2D.OverlapCircleAll(SpriteCenterPos, attackAreaRadius, attackLayerMask);
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
                AudioManager.PlayAudioAtPosition(hitSFX, transform.position, AudioManager.combatSfxMixerGroup);
            }
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SpriteCenterPos, attackAreaRadius);
    }
}
