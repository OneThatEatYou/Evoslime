using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager
{
    public AudioMixer mainMixer;
    public static AudioMixerGroup bgmMixerGroup;
    public static AudioMixerGroup sfxMixerGroup;
    public static AudioMixerGroup combatSfxMixerGroup;
    public static AudioMixerGroup environmentSfxMixerGroup;

    public static AudioClip buttonYesSFX;
    public static AudioClip buttonNoSFX;

    public AudioManager()
    {
        Init();
    }

    public void Init()
    {
        mainMixer = Resources.Load<AudioMixer>("MainMixer");
        bgmMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
        combatSfxMixerGroup = mainMixer.FindMatchingGroups("SFX/Combat")[0];
        environmentSfxMixerGroup = mainMixer.FindMatchingGroups("SFX/Environment")[0];

        buttonYesSFX = Resources.Load<AudioClip>("Button_yes");
        buttonNoSFX = Resources.Load<AudioClip>("Button_no");
    }

    public static AudioSource PlayAudioAtPosition(AudioClip audioClip, Vector2 position, AudioMixerGroup mixerGroup, bool autoDestroy = true, float spatialBlend = 0.5f)
    {
        GameObject obj = new GameObject("OneShotAudio");
        obj.transform.position = position;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.spatialBlend = spatialBlend;
        source.dopplerLevel = 0;
        source.spread = 45;
        source.maxDistance = 20;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        if (autoDestroy)
        {
            GameObject.Destroy(obj, audioClip.length);
        }

        return source;
    }

    //convert vol percent to value in mixer group
    public float VolumeToAtten(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
    }

    //convert value in mixer group to vol percent
    public float AttenToVol(float atten)
    {
        return Mathf.Pow(10, atten / 20);
    }
}
