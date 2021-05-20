using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores the basic data of the monster
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;

    [Header("Stats")]
    public float moveSpeed;
    public uint appetite;
    public uint digestionSpeed;

    [Header("Behaviour")]
    public List<FoodType> diet = new List<FoodType>();
}
