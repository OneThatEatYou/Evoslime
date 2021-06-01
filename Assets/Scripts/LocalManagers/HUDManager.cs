using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider healthSlider;
    public Transform foodSliderParent;
    public GameObject foodSliderPrefab;
    public GameObject evolutionPrompt;

    Dictionary<NutritionType, Slider> foodSliders = new Dictionary<NutritionType, Slider>();

    public void Init(MonsterData monsterData)
    {
        ClearSliders();
        healthSlider.maxValue = monsterData.maxHealth;
    }

    public void SetHealthSlider(int newValue)
    {
        healthSlider.value = newValue;
    }

    public void SetFoodSlider(Dictionary<NutritionType, int> foodConsumed, int appetite)
    {
        int lastVal = 0; // the value offset of the slider

        foreach (KeyValuePair<NutritionType, int> food in foodConsumed)
        {
            Slider slider;

            // find the slider for this nutrition type
            if (foodSliders.ContainsKey(food.Key))
            {
                // get the existing slider for the nutrition type
                slider = foodSliders[food.Key];
            }
            else
            {
                // instantiate a slider
                GameObject obj = Instantiate(foodSliderPrefab, foodSliderParent);
                Image image = obj.GetComponentInChildren<Image>();

                obj.transform.SetSiblingIndex(1);
                slider = obj.GetComponent<Slider>();
                image.color = FoodData.foodColorDict[food.Key];
                foodSliders.Add(food.Key, slider);
            }

            slider.maxValue = appetite;
            slider.value = lastVal + food.Value;
            lastVal = lastVal + food.Value;
        }
    }

    void ClearSliders()
    {
        if (foodSliders.Count > 0)
        {
            foreach (Slider slider in foodSliders.Values)
            {
                Destroy(slider.gameObject);
            }

            foodSliders = new Dictionary<NutritionType, Slider>();
        }
    }

    public void SetEvolutionPrompt(bool canEvolve)
    {
        evolutionPrompt.SetActive(canEvolve);
    }
}
