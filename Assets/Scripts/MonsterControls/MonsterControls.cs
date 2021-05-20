using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class MonsterControls : MonoBehaviour
{
    [Expandable] public MonsterData monsterData;

    protected float MoveSpeed { get { return monsterData.moveSpeed; } }
    protected List<FoodType> Diet { get { return monsterData.diet; } }
    protected float Appetite { get { return monsterData.appetite; } }
    protected float DigestionSpeed { get { return monsterData.digestionSpeed; } }
    protected int Fullness
    { 
        get 
        {
            int sum = 0;

            foreach (var item in stomach)
            {
                sum += item.Value;
            }

            return sum;
        } 
    }

    protected bool isAttacking = false;
    protected int direction = -1;
    protected Dictionary<NutritionType, int> stomach = new Dictionary<NutritionType, int>();
    protected Dictionary<NutritionType, int> absorbedNutrients = new Dictionary<NutritionType, int>();

    protected Rigidbody2D rb;
    protected Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        InvokeRepeating("Digest", 0, 1);
    }

    private void Update()
    {
        
    }

    public abstract void Move(Vector2 input);

    public abstract void Attack(Vector2 dir);

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
            if (stomach.ContainsKey(nutririonStruct.nutritionType))
            {
                stomach[nutririonStruct.nutritionType] += nutririonStruct.amount;
            }
            else
            {
                stomach.Add(nutririonStruct.nutritionType, nutririonStruct.amount);
            }
        }
    }

    protected void Digest()
    {
        int fullness = Fullness;

        if (fullness == 0)
        { return; }

        List<NutritionType> keys = new List<NutritionType>(stomach.Keys);

        foreach (NutritionType key in keys)
        {
            int digestedAmount = Mathf.CeilToInt((float)stomach[key] / fullness * DigestionSpeed);

            // moved digested amount from the stomach dict to absorbed dict
            stomach[key] -= digestedAmount;
            AbsorbNutrients(key, digestedAmount);

            if (stomach[key] <= 0)
            {
                stomach.Remove(key);
            }
        }
    }

    void AbsorbNutrients(NutritionType nutritionType, int amount)
    {
        // check if it is the first time absorbing this nutrition
        if (absorbedNutrients.ContainsKey(nutritionType))
        {
            absorbedNutrients[nutritionType] += amount;
        }
        else
        {
            absorbedNutrients.Add(nutritionType, amount);
        }
    }

    [ContextMenu("Print stomach contents")]
    void PrintStomach()
    {
        Debug.Log($"Nutrition types consumed: {stomach.Count}");

        foreach (var kvp in stomach)
        {
            Debug.Log(kvp.Key + " = " + kvp.Value);
        }
    }

    [ContextMenu("Print absorbed nutrition")]
    void PrintAbsorbedNutrition()
    {
        Debug.Log($"Nutrition types absorbed: {absorbedNutrients.Count}");

        foreach (var kvp in absorbedNutrients)
        {
            Debug.Log(kvp.Key + " = " + kvp.Value);
        }
    }
}
