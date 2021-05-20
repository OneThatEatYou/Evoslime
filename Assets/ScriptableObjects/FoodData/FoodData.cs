using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFoodData", menuName = "ScriptableObjects/FoodData")]
public class FoodData : ScriptableObject
{
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
    Meat
}

// the type of experience held by the food
public enum NutritionType
{
    Spore,
    Cellulose,
    Compost,
    Rock,
    Metal,
    Gem,
    Bone,
    Grim
}

[System.Serializable]
public class NutritionStruct
{
    public NutritionType nutritionType;
    public int amount;
}
