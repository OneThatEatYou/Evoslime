using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public FoodType ConsumableFoodType { get { return foodData.foodType; } }
    public AudioClip[] consumeSFX;

    [Expandable, SerializeField] FoodData foodData;

    public FoodData Consume()
    {
        PlayRandomSFX();
        Destroy(gameObject);

        return foodData;
    }

    void PlayRandomSFX()
    {
        int i = Random.Range(0, consumeSFX.Length);

        AudioManager.PlayAudioAtPosition(consumeSFX[i], transform.position, AudioManager.environmentSfxMixerGroup);
    }
}
