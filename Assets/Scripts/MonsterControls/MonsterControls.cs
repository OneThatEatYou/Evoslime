using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class MonsterControls : MonoBehaviour
{
    #region Delegates and Events
    public delegate void OnValueChangedDelegate<T>(T newValue);
    public event OnValueChangedDelegate<int> onHealthChangedHandler;
    public event OnValueChangedDelegate<MonsterData> onEvolvedHandler;
    public event OnValueChangedDelegate<bool> onFoodConsumedBoolHandler;
    public delegate void OnFoodConsumedDelegate(Dictionary<NutritionType, int> foodConsumed, int appetite);
    public event OnFoodConsumedDelegate onFoodConsumedHandler;
    public delegate void OnDeathDelegate();
    public event OnDeathDelegate onDeathHandler;
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
            health = Mathf.Clamp(health, 0, MaxHealth);

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
    protected Vector2 SpriteCenterPos { get { return (Vector2)transform.position + Vector2.up * spriteHeight; } }
    protected EvolutionManager EvolutionManager { get { return GameManager.Instance.evolutionManager; } }
    #endregion

    [Expandable] public MonsterData monsterData;
    public Material flashMat;
    public GameObject evolutionParticle;
    public AudioClip evolutionSparkSFX;
    public AudioClip evolutionCompleteSFX;
    [Tooltip("The center of the sprite relative to this object's position")]
    public float spriteHeight;
    public GameObject deathParticle;
    public AudioClip deathSFX;

    protected bool isAttacking = false;
    protected bool isKnockingBack = false;
    protected int direction = -1;
    protected Dictionary<NutritionType, int> foodConsumed = new Dictionary<NutritionType, int>();

    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    bool isResting = false;
    bool isInvinsible = false;
    float movementWeight = 1;
    float restTimeElapsed = 0;
    Coroutine knockBackTimer;
    Coroutine healthRegenCR;
    Material startMat;

    protected const string animDirParam = "Direction";
    protected const string animSpeedParam = "Speed";
    protected const string animAttackParam = "Attack";
    protected const string animIdleState = "Idle";

    const float restCooldown = 10;           // number of seconds to wait before entering rest state
    const float healthRegenInterval = 3;    // number of seconds between each health regen when resting
    const float hurtFlashTime = 0.2f;
    const float knockbackDuration = 0.5f;
    const float knockbackRecoveryTime = 0.2f;
    const float knockbackForce = 18;

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
        CheckToRest();
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
            anim.SetFloat(animDirParam, input.x);
            direction = Mathf.RoundToInt(input.x);
        }

        anim.SetFloat(animSpeedParam, rb.velocity.magnitude);
    }

    IEnumerator FlashSprite(float duration)
    {
        spriteRenderer.material = flashMat;

        yield return new WaitForSecondsRealtime(duration);

        spriteRenderer.material = startMat;
    }

    #region Rest
    void CheckToRest()
    {
        if (isResting)
        { return; }

        if (restTimeElapsed < restCooldown)
        {
            restTimeElapsed += Time.deltaTime;
        }
        else
        {
            EnterRestState();
        }
    }

    void EnterRestState()
    {
        // don't do anything if the monster is already resting
        if (isResting)
        {
            Debug.LogWarning("Attempting to rest when monster is already resting", gameObject);
            return; 
        }

        isResting = true;
        healthRegenCR = StartCoroutine(RegenHealth());
    }

    void ExitRestState()
    {
        restTimeElapsed = 0;
        
        // only resets rest time elapsed if the monster is not resting
        if (!isResting)
        { return; }

        isResting = false;

        StopCoroutine(healthRegenCR);
    }

    IEnumerator RegenHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(healthRegenInterval);

            Health += monsterData.healthRegen;
        }
    }
    #endregion

    #region Combat
    public virtual void Attack(Vector2 dir)
    {
        ExitRestState();
    }

    public abstract void CancelAttack();

    // called when another monster attacks this monster
    public void TakeDamage(int damage, Vector2 knockbackDir)
    {
        if (isInvinsible)
        { return; }

        // hurt animation
        // hurt sfx

        CancelAttack();
        StartCoroutine(FlashSprite(hurtFlashTime));
        Knockback(knockbackDir, knockbackForce);
        ExitRestState();

        Health -= damage;
    }

    void Knockback(Vector2 dir, float force)
    {
        if (dir.sqrMagnitude == 0)
        { return; }

        if (isKnockingBack)
        {
            StopCoroutine(knockBackTimer);
        }

        isKnockingBack = true;
        isInvinsible = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        knockBackTimer = StartCoroutine(KnockbackRecoveryTimer(knockbackDuration, knockbackRecoveryTime));
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

    [ContextMenu("Die")]
    void Die()
    {
        AudioManager.PlayAudioAtPosition(deathSFX, transform.position, AudioManager.combatSfxMixerGroup);
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);

        onDeathHandler?.Invoke();
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
        onFoodConsumedBoolHandler?.Invoke(CanEvolve());
    }
    #endregion

    #region Evolution
    bool IsFull()
    {
        return Fullness >= Appetite;
    }

    bool CanEvolve()
    {
        if (IsFull() && EvolutionManager.GetEvolutionMonster(foodConsumed, monsterData.evolutions))
        {
            return true;
        }

        return false;
    }

    [ContextMenu("Evolve")]
    public void Evolve()
    {
        if (!CanEvolve())
        { return; }

        MonsterData newMonsterData = EvolutionManager.GetEvolutionMonster(foodConsumed, monsterData.evolutions);

        if (newMonsterData == null)
        {
            Debug.LogWarning("Attempting to evolving as null. Aborting evolution.");
            return;
        }

        StartCoroutine(EvolutionAnimation(newMonsterData));
    }

    IEnumerator EvolutionAnimation(MonsterData newMonsterData)
    {
        float totalTimeElapsed = 0;
        float maxFlashTime = 8;
        float startTimeScale = Time.timeScale;

        // create SFX audio source
        AudioSource evoSfxSource = AudioManager.PlayAudioAtPosition(null, transform.position, AudioManager.combatSfxMixerGroup, false);
        evoSfxSource.clip = evolutionSparkSFX;

        Time.timeScale = 0;

        while (totalTimeElapsed < maxFlashTime)
        {
            float flashTime = 0.1f;
            float waitTime = flashTime + 0.2f * (1 - totalTimeElapsed / maxFlashTime);

            StartCoroutine(FlashSprite(flashTime));
            Instantiate(evolutionParticle, SpriteCenterPos, Quaternion.identity);

            evoSfxSource.Play();

            totalTimeElapsed += waitTime;

            yield return new WaitForSecondsRealtime(waitTime);
        }
        
        Time.timeScale = startTimeScale;

        evoSfxSource.clip = evolutionCompleteSFX;
        evoSfxSource.Play();
        Destroy(evoSfxSource.gameObject, evolutionCompleteSFX.length);

        // switch monster
        onEvolvedHandler?.Invoke(newMonsterData);
    }
    #endregion

    #region Debug
    [ContextMenu("Print consumed food")]
    void PrintConsumedFood()
    {
        Debug.Log($"Nutrition types consumed: {foodConsumed.Count}");

        foreach (KeyValuePair<NutritionType, int> kvp in foodConsumed)
        {
            Debug.Log(kvp.Key + " = " + kvp.Value);
        }
    }
    #endregion

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(SpriteCenterPos, 0.1f);
    }
}
