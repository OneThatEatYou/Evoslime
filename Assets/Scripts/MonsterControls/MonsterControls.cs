using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class MonsterControls : MonoBehaviour
{
    [Expandable] public MonsterData monsterData;

    protected float MoveSpeed { get { return monsterData.moveSpeed; } }
    protected List<FoodType> Diet { get { return monsterData.diet; } }
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

    public bool IsEdible(Consumable food)
    {
        return Diet.Contains(food.ConsumableFoodType);
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
}
