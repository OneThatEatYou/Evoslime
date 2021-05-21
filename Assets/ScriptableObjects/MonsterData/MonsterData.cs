using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores the basic data of the monster
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;

    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 100;
    [Tooltip("Max value displayable in food bar")] public int appetite = 1000;
    public float digestionSpeed = 3;
    public int damage;

    [Header("Behaviour")]
    public List<FoodType> diet = new List<FoodType>();
}
