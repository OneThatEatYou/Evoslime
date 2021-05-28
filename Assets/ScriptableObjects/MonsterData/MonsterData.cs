using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores the basic data of the monster
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public GameObject monsterPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 100;
    [Tooltip("Max value displayable in food bar")] public int appetite = 1000;
    public int damage;
    public GameObject deathParticle;

    [Space(10)]
    public List<FoodType> diet = new List<FoodType>();

    [Header("AI behaviour")]
    public float detectionRadius = 2;
    public float attackRadius = 0.7f;
    public float wanderCooldown = 10;
    public float wanderRadius = 4;
    public float maxWanderTime = 5;
    public float maxChaseTime = 10;

    [Header("Evolution")]
    public EvolutionData[] evolutions;
}

[System.Serializable]
public struct EvolutionData
{
    public MonsterData monsterData;
    public NutritionStruct[] minimumNutrition;
}
