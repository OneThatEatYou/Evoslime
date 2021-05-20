using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [Expandable] public FoodData foodData;

    public FoodData Consume()
    {
        Destroy(gameObject);
        return foodData;
    }
}
