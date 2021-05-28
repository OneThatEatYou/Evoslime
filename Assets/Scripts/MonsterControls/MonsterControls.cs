using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class MonsterControls : MonoBehaviour
{
    [Expandable] public MonsterData monsterData;

    #region Delegates and Events
    public delegate void OnIntChangedHandler(int newValue);
    public event OnIntChangedHandler onHealthChangedHandler;
    public delegate void OnFoodConsumedHandler(Dictionary<NutritionType, int> foodConsumed, int appetite);
    public event OnFoodConsumedHandler onFoodConsumedHandler;
    #endregion

    #region Properties
    protected int MaxHealth { get { return monsterData.maxHealth; } }
    int health;
    protected int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            onHealthChangedHandler?.Invoke(health);

            if (Health <= 0)
            {
                Die();
            }
        }
    }
    protected float MoveSpeed { get { return monsterData.moveSpeed; } }
    protected List<FoodType> Diet { get { return monsterData.diet; } }
    protected int Appetite { get { return monsterData.appetite; } }
    protected int Fullness
    { 
        get 
        {
            int sum = 0;

            foreach (var item in foodConsumed)
            {
                sum += item.Value;
            }

            return sum;
        } 
    } //sum of all nutrient values in the stomach
    protected GameObject DeathParticle { get { return monsterData.deathParticle; } }
    #endregion

    public Material flashMat;

    protected bool isAttacking = false;
    bool isKnockingBack = false;
    bool isInvinsible = false;
    float movementWeight = 1;
    Coroutine knockBackTimer;
    protected int direction = -1;
    protected Dictionary<NutritionType, int> foodConsumed = new Dictionary<NutritionType, int>();
    protected Material startMat;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startMat = spriteRenderer.material;
    }

    private void Start()
    {
        Health = MaxHealth;
    }

    private void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // attempt to consume the food that this monster collides with
        Consumable food;

        if (collision.gameObject.TryGetComponent(out food))
        {
            if (IsEdible(food) && !IsFull())
            {
                Eat(food.Consume());
            }
        }
    }

    public virtual void Move(Vector2 input)
    {
        if (isAttacking || isKnockingBack)
        { return; }

        rb.velocity = input * MoveSpeed * movementWeight * Time.fixedDeltaTime;

        if (input.x != 0)
        {
            anim.SetFloat("Direction", input.x);
            direction = Mathf.RoundToInt(input.x);
        }

        anim.SetFloat("Speed", rb.velocity.magnitude);
    }

    #region Combat
    public abstract void Attack(Vector2 dir);

    // called when another monster attacks this monster
    public void TakeDamage(int damage, Vector2 knockbackDir)
    {
        if (isInvinsible)
        { return; }

        // hurt animation
        // hurt sfx
        StartCoroutine(FlashSprite(0.2f));

        Health -= damage;
        Knockback(knockbackDir, 18);

        Debug.Log($"{name} took {damage} damage", gameObject);
    }

    void Knockback(Vector2 dir, float force)
    {
        if (dir.x == 0 && dir.y == 0)
        { return; }

        if (isKnockingBack)
        {
            StopCoroutine(knockBackTimer);
        }

        isKnockingBack = true;
        isInvinsible = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        knockBackTimer = StartCoroutine(KnockbackRecoveryTimer(0.5f, 0.2f));
    }

    IEnumerator KnockbackRecoveryTimer(float knockbackTime, float recoveryTime)
    {
        float t = 0;

        yield return new WaitForSeconds(knockbackTime);

        movementWeight = 0;
        isKnockingBack = false;
        isInvinsible = false;

        while (t < recoveryTime)
        {
            t += Time.deltaTime;
            movementWeight = Mathf.Lerp(0, 1, Mathf.Pow(t / recoveryTime, 2));
            yield return null;
        }

        movementWeight = 1;
    }

    IEnumerator FlashSprite(float duration)
    {
        spriteRenderer.material = flashMat;

        yield return new WaitForSeconds(duration);

        spriteRenderer.material = startMat;
    }

    void Die()
    {
        Debug.Log($"{name} has died", gameObject);
        Instantiate(DeathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    #endregion

    #region Food System
    public bool IsEdible(Consumable food)
    {
        return Diet.Contains(food.ConsumableFoodType);
    }

    public bool IsFull()
    {
        return Fullness >= Appetite;
    }

    public void Eat(FoodData foodData)
    {
        foreach (NutritionStruct nutririonStruct in foodData.nutritions)
        {
            // check if it is the first time consuming this nutrition
            if (foodConsumed.ContainsKey(nutririonStruct.nutritionType))
            {
                foodConsumed[nutririonStruct.nutritionType] += nutririonStruct.amount;
            }
            else
            {
                foodConsumed.Add(nutririonStruct.nutritionType, nutririonStruct.amount);
            }
        }

        onFoodConsumedHandler?.Invoke(foodConsumed, Appetite);
    }
    #endregion

    #region Debug
    [ContextMenu("Print consumed food")]
    void PrintConsumedFood()
    {
        Debug.Log($"Nutrition types consumed: {foodConsumed.Count}");

        foreach (var kvp in foodConsumed)
        {
            Debug.Log(kvp.Key + " = " + kvp.Value);
        }
    }
    #endregion
}
