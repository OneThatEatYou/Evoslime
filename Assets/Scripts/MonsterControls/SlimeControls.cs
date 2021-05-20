using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeControls : MonsterControls
{
    [Header("Attack Animation")]
    public float chargeTime;
    public float attackTime;
    public float dashSpeed;

    public override void Move(Vector2 input)
    {
        if (isAttacking)
        { return; }

        rb.velocity = input * MoveSpeed * Time.fixedDeltaTime;

        if (input.x != 0)
        {
            anim.SetFloat("Direction", input.x);
            direction = Mathf.RoundToInt(input.x);
        }

        anim.SetFloat("Speed", rb.velocity.magnitude);
    }

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
            rb.velocity = dashSpeed * dir * Mathf.Pow((1 - t / attackTime), 2);
            t += Time.deltaTime;

            yield return null;
        }

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Consumable food;

        if (collision.gameObject.TryGetComponent(out food))
        {
            if (IsEdible(food) && !IsFull())
            {
                Eat(food.Consume());
            }
        }
    }
}
