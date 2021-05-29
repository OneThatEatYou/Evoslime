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
    public delegate void OnEvolvedHandler(MonsterData newMonsterData);
    public event OnEvolvedHandler onEvolvedHandler;
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
    protected Vector2 spriteCenterPos { get { return (Vector2)transform.position + Vector2.up * spriteHeight; } }
    #endregion

    public Material flashMat;

    [Space(10)]
    [Tooltip("Where the center of the sprite should be relative to this object's position")]
    public float spriteHeight;

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

    const string evolveParam = "Evolve";

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
            AttemptEat(food);
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

    IEnumerator FlashSprite(float duration)
    {
        spriteRenderer.material = flashMat;

        yield return new WaitForSecondsRealtime(duration);

        spriteRenderer.material = startMat;
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

        //Debug.Log($"{name} took {damage} damage", gameObject);
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

    void Die()
    {
        Instantiate(DeathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    #endregion

    #region Food System
    public bool IsEdible(Consumable food)
    {
        return Diet.Contains(food.ConsumableFoodType);
    }

    public void AttemptEat(Consumable food)
    {
        if (IsEdible(food))
        {
            Eat(food.Consume());
        }
    }

    void Eat(FoodData foodData)
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

        onFoodConsumedHandler?.Invoke(foodConsumed, Mathf.Max(Appetite, Fullness));
    }
    #endregion

    #region Evolution
    bool IsFull()
    {
        return Fullness >= Appetite;
    }

    [ContextMenu("Evolve")]
    void Evolve()
    {
        MonsterData newMonsterData = GameManager.Instance.evolutionManager.GetEvolutionMonster(foodConsumed, monsterData.evolutions);

        if (newMonsterData == null)
        {
            Debug.Log("No available evolution");
            return;
        }

        Debug.Log($"Evolving as {newMonsterData.monsterName}");

        StartCoroutine(EvolutionAnimation(newMonsterData));
    }

    IEnumerator EvolutionAnimation(MonsterData newMonsterData)
    {
        float totalTimeElapsed = 0;
        float maxFlashTime = 8;
        float startTimeScale = Time.timeScale;

        Time.timeScale = 0;

        while (totalTimeElapsed < maxFlashTime)
        {
            //float flashTime = 0.05f + 0.15f * (1 - totalTimeElapsed / maxFlashTime);
            //float waitTime = 2.5f * flashTime;

            float flashTime = 0.1f;
            float waitTime = flashTime + 0.2f * (1 - totalTimeElapsed / maxFlashTime);

            StartCoroutine(FlashSprite(flashTime));
            Instantiate(GameManager.Instance.evolutionManager.evolutionParticle, spriteCenterPos, Quaternion.identity);
            totalTimeElapsed += waitTime;

            yield return new WaitForSecondsRealtime(waitTime);
        }
        
        Time.timeScale = startTimeScale;

        // switch monster
        onEvolvedHandler?.Invoke(newMonsterData);
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

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spriteCenterPos, 0.1f);
    }
}
