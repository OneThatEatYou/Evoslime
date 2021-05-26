using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider healthSlider;
    public Transform foodSliderParent;
    public GameObject foodSliderPrefab;

    Dictionary<NutritionType, Slider> foodSliders = new Dictionary<NutritionType, Slider>();

    public void Init(MonsterData monsterData)
    {
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
                slider.maxValue = appetite;
                image.color = FoodData.foodColorDict[food.Key];
                foodSliders.Add(food.Key, slider);
            }

            slider.value = lastVal + food.Value;
            lastVal = lastVal + food.Value;
        }
    }
}
