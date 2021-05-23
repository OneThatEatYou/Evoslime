using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider healthSlider;
    public Slider foodSlider;

    public void Init(MonsterData monsterData)
    {
        healthSlider.maxValue = monsterData.maxHealth;
        foodSlider.maxValue = monsterData.appetite;
    }

    public void SetHealthSlider(int newValue)
    {
        healthSlider.value = newValue;
    }

    public void SetFoodSlider(int newValue)
    {
        foodSlider.value = newValue;
    }
}
