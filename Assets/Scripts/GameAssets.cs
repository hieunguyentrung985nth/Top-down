using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));

            return _i;
        }
    }

    [Serializable]
    public class BGMSoundAudioClip
    {
        public SoundManager.BGMSound soundType;

        public AudioClip clip;
    }

    [Serializable]
    public class SFXSoundAudioClip
    {
        public SoundManager.SFXSound soundType;

        public AudioClip clip;
    }

    public BGMSoundAudioClip[] bgmSoundAudioArray;

    public SFXSoundAudioClip[] sfxSoundAudioArray;

    public GameObject PopUp;
}
