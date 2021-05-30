using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager
{
    public EvolutionManager()
    {
        Init();
    }

    void Init()
    {
        
    }

    /// <summary>
    /// Returns true if there is sufficient nutrition to evolve as the input evolution data
    /// </summary>
    bool CanEvolveAs(Dictionary<NutritionType, int> curNutrition, EvolutionData evolutionData)
    {
        foreach (NutritionStruct requiredNutrition in evolutionData.minimumNutrition)
        {
            int curNutritionAmount;

            if (curNutrition.TryGetValue(requiredNutrition.nutritionType, out curNutritionAmount) && curNutritionAmount > requiredNutrition.amount)
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns the monster data of the monster that will be evolved as
    /// </summary>
    public MonsterData GetEvolutionMonster(Dictionary<NutritionType, int> curNutrition, EvolutionData[] evolutions)
    {
        foreach (EvolutionData evolutionData in evolutions)
        {
            if (CanEvolveAs(curNutrition, evolutionData))
            {
                Debug.Log($"Can evolve as {evolutionData.monsterData.monsterName}");
                return evolutionData.monsterData;
            }
        }

        return null;
    }
}
