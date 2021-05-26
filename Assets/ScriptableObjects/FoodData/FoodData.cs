using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodData", menuName = "ScriptableObjects/FoodData")]
public class FoodData : ScriptableObject
{
    public static Dictionary<NutritionType, Color> foodColorDict = new Dictionary<NutritionType, Color>()
    {
        { NutritionType.Spore, new Color(231, 181, 75, 255)/255f},
        { NutritionType.Cellulose, new Color(24, 164, 43, 255)/255f},
        { NutritionType.Compost, new Color(200, 123, 84, 255)/255f},
        { NutritionType.Rock, new Color(107, 149, 161, 255)/255f},
    };

    public string foodName;
    public FoodType foodType;
    public NutritionStruct[] nutritions;
}

public enum FoodType
{
    Mushroom,
    Plant,
    Compost,
    Mineral,
}

// the type of experience held by the food
public enum NutritionType
{
    Spore,
    Cellulose,
    Compost,
    Rock,
}

[System.Serializable]
public class NutritionStruct
{
    public NutritionType nutritionType;
    public int amount;
}
