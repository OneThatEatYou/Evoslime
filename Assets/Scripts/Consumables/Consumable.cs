using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public FoodType ConsumableFoodType { get { return foodData.foodType; } }

    [Expandable, SerializeField] FoodData foodData;

    public FoodData Consume()
    {
        Destroy(gameObject);
        return foodData;
    }
}
