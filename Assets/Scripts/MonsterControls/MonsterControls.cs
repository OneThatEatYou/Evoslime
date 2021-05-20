using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class MonsterControls : MonoBehaviour
{
    [Expandable] public MonsterData monsterData;

    protected float MoveSpeed { get { return monsterData.moveSpeed; } }
    protected List<FoodType> Diet { get { return monsterData.diet; } }

    protected bool isAttacking = false;
    protected int direction = -1;

    protected Rigidbody2D rb;
    protected Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public abstract void Move(Vector2 input);

    public abstract void Attack(Vector2 dir);

    public void Eat(FoodData foodData)
    {
        if (Diet.Contains(foodData.foodType))
        {
            Debug.Log($"{name} consumed {foodData.foodName}");
        }
    }
}
