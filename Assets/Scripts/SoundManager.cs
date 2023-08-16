using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Serializable]
    public enum BGMSound
    {
        MainMenuTheme,
        InBattle
    }

    [Serializable]
    public enum SFXSound
    {
        Movement,
        Dead,
        Pistols,
        Shotgun,
        RocketLauncher,
        AK47,
        Laser,
        Nuclear,
        Enemy1_Atk,
        Enemy1_Die,
        Enemy2_Atk,
        Enemy2_Die,
        Ammo,
        CantBuy,
        ButtonHover,
        ButtonPress,
        ShopButtonHover,
        Heal,
        NULL
    }

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup soundBGM;

    [SerializeField] private AudioMixerGroup soundSFX;

    [SerializeField] private float bgmVolume;

    [SerializeField] private float sfxVolume;

    private Transform bgmSound;

    private Transform sfxSound;

    private SoundSettings soundSettings;

    private GameManager gameManager;

    private bool canPlayInGameMusic = false;

    private Dictionary<SFXSound, float> dictionarySound;

    private void Awake()
    {
        Instance = this;

        bgmSound = transform.Find("BGM");

        sfxSound = transform.Find("SFX");

        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnPlayInGameMusicEvent += GameManager_OnPlayInGameMusicEvent;

        EnemyCounter.Instance.OnPlayInGameMusic += Instance_OnPlayInGameMusic;
    }

    private void Instance_OnPlayInGameMusic()
    {
        canPlayInGameMusic = true;

        StartCoroutine(PlayRandomBGMSound(BGMSound.InBattle));
    }

    private void GameManager_OnPlayInGameMusicEvent()
    {
        canPlayInGameMusic = true;

        StartCoroutine(PlayRandomBGMSound(BGMSound.InBattle));
    }

    private void OnEnable()
    {
        soundSettings = SaveSystem.LoadSoundSettings();

        if(soundSettings == null)
        {
            soundSettings = new SoundSettings();

            soundSettings.bgmVolume = 0;

            soundSettings.sfxVolume = 0;
        }

        bgmVolume = soundSettings.bgmVolume;

        sfxVolume = soundSettings.sfxVolume;


    }

    private void Start()
    {
        Initialize();

        SetUpBGM();

        //PlayBGMSound(BGMSound.MainMenuTheme);

        //SaveSystem.DeleteSoundSettingsSave();
    }

    private void SetUpBGM()
    {
        AudioSource audioComponent = bgmSound.gameObject.AddComponent<AudioSource>();

        audioComponent.outputAudioMixerGroup = soundBGM;

        audioComponent.loop = true;

        audioComponent.playOnAwake = true;
    }


    public void PlayBGMSound(BGMSound soundToPlay)
    {
        List<AudioClip> clipToPlay = GetBGMAudioClip(soundToPlay);

        bgmSound.GetComponent<AudioSource>().clip = clipToPlay[0];

        bgmSound.GetComponent<AudioSource>().Play();
    }

    private IEnumerator PlayRandomBGMSound(BGMSound soundToPlay)
    {
        List<AudioClip> listToPlay = GetBGMAudioClip(soundToPlay);

        int index = UnityEngine.Random.Range(0, 2);

        while (canPlayInGameMusic && gameManager.gameState == GameManager.GameState.IS_PLAYING)
        {
            bgmSound.GetComponent<AudioSource>().clip = listToPlay[index];

            bgmSound.GetComponent<AudioSource>().Play();

            yield return new WaitForSecondsRealtime(listToPlay[index].length + 1f);

            index++;

            if(index == 2)
            {
                index = 0;
            }
        }
        
    }

    public void StopBGMSound()
    {
        bgmSound.GetComponent<AudioSource>().Stop();

        canPlayInGameMusic = false;
    }

    public void OnChangeBGMVolume(float value)
    {
        bgmVolume = Mathf.Log10(value) * 20;

        soundBGM.audioMixer.SetFloat("bgmVolume", bgmVolume);

        soundSettings.bgmVolume = bgmVolume;

        SaveSystem.SaveSoundSettings(soundSettings);
    }

    public float GetBGMVolume()
    {
        return Mathf.Pow(10, bgmVolume / 20);
    }

    private AudioSource SetUpSFX()
    {
        AudioSource audioComponent = sfxSound.gameObject.AddComponent<AudioSource>();

        audioComponent.outputAudioMixerGroup = soundSFX;

        return audioComponent;
    }

    private void Initialize()
    {
        dictionarySound = new Dictionary<SFXSound, float>();

        soundBGM.audioMixer.SetFloat("bgmVolume", bgmVolume);

        soundSFX.audioMixer.SetFloat("sfxVolume", sfxVolume);

        dictionarySound.Add(SFXSound.Movement, 0);

        foreach (GameAssets.SFXSoundAudioClip clip in GameAssets.i.sfxSoundAudioArray)
        {
            AudioSource audioComponent = SetUpSFX();

            sfxDictionary.Add(clip.soundType, audioComponent);
        }
    }

    private bool CanPlaySound(SFXSound sound)
    {
        switch (sound)
        {
            case SFXSound.Movement:
                if (dictionarySound.ContainsKey(sound))
                {
                    float lastTimePlayed = dictionarySound[sound];

                    float maxTimePlay = 0.05f;

                    if(lastTimePlayed + maxTimePlay < Time.time)
                    {
                        dictionarySound[sound] = Time.time;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case SFXSound.AK47:
                if (dictionarySound.ContainsKey(sound))
                {
                    float lastTimePlayed = dictionarySound[sound];

                    float maxTimePlay = 0.05f;

                    if (lastTimePlayed + maxTimePlay < Time.time)
                    {
                        dictionarySound[sound] = Time.time;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
        }
    }

    public void StopSFXSound(SFXSound sound)
    {
        switch (sound)
        {
            case SFXSound.Movement:
                sfxDictionary[sound].Stop();
                break;
            case SFXSound.Dead:
                sfxDictionary[sound].Stop();
                break;
            case SFXSound.Pistols:
                break;
            case SFXSound.Shotgun:
                break;
            case SFXSound.RocketLauncher:
                break;
            case SFXSound.AK47:
                sfxDictionary[sound].Stop();
                break;
            case SFXSound.Laser:
                break;
            case SFXSound.Nuclear:
                sfxDictionary[sound].Stop();
                break;
            case SFXSound.NULL:
                break;
            default:
                break;
        }
    }

    private bool IsSpecialSound(SFXSound sound)
    {
        switch (sound)
        {
            case SFXSound.AK47:
                return true;
            case SFXSound.Movement:
                return true;
            default:
                return false;
        }
    }

    private bool IsLoopSound(SFXSound sound)
    {
        switch (sound)
        {
            case SFXSound.AK47:
                return true;
            case SFXSound.Movement:
                return true;
            default:
                return false;
        }
    }

    public SFXSound GetEnemyDieSound(Enemy enemy)
    {
        if (enemy.id == 0 || enemy.id == 1 || enemy.id == 2 || enemy.id == 3)
        {
            return SFXSound.Enemy1_Die;
        }

        else
        {
            return SFXSound.Enemy2_Die;
        }
    }

    public SFXSound GetEnemyAtkSound(Enemy enemy)
    {
        if (enemy.id == 0 || enemy.id == 1 || enemy.id == 2 || enemy.id == 3)
        {
            return SFXSound.Enemy1_Atk;
        }

        else
        {
            return SFXSound.Enemy2_Atk;
        }
    }

    private Dictionary<SFXSound, AudioSource> sfxDictionary = new Dictionary<SFXSound, AudioSource>();

    private SFXSound currentSFXSound;

    private AudioClip currentClip;

    private List<GameObject> listSO = new List<GameObject>();

    private void ResetPitch()
    {
        sfxDictionary[SFXSound.Dead].pitch = 1;
    }

    private void StopDeathSound()
    {
        StopSFXSound(SFXSound.Dead);
    }

    private bool CanPlayOneShot(SFXSound sound)
    {
        if(sound == SFXSound.Enemy1_Die || sound == SFXSound.Enemy2_Die || sound == SFXSound.Heal || sound == SFXSound.Ammo)
        {
            return true;
        }

        return false;
    }

    public void PlaySFXSound(SFXSound sound)
    {
        if (CanPlaySound(sound))
        {
            List<AudioClip> clipToPlay = GetSFXAudioClip(sound);

            if (sound == SFXSound.Enemy1_Atk || sound == SFXSound.Enemy2_Atk || sound == SFXSound.Enemy1_Die || sound == SFXSound.Enemy2_Die)
            {
                if (canPlay)
                {
                    canPlay = false;

                    if (CanPlayOneShot(sound))
                    {
                        sfxDictionary[sound].PlayOneShot(clipToPlay[0]);
                    }

                    StartCoroutine(Reset());
                }
            }
            
            if (sound == SFXSound.Dead)
            {
                sfxDictionary[sound].pitch = 2;

                Invoke(nameof(ResetPitch), 0.4f);

                Invoke(nameof(StopDeathSound), 2.2f);
            }

            if (IsSpecialSound(sound))
            {
                if (IsLoopSound(sound))
                {
                    sfxDictionary[sound].loop = true;
                }

                if (!sfxDictionary[sound].isPlaying)
                {
                    sfxDictionary[sound].clip = clipToPlay[0];

                    sfxDictionary[sound].Play();
                }
            }
            else
            {
                if (CanPlayOneShot(sound))
                {
                    sfxDictionary[sound].PlayOneShot(clipToPlay[0]);
                }
                else
                {
                    sfxDictionary[sound].clip = clipToPlay[0];

                    sfxDictionary[sound].Play();
                }
            }

                  
        }      
    }

    private bool canPlay = true;

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.3f);

        canPlay = true;
    }

    public void OnChangeSFXVolume(float value)
    {
        sfxVolume = Mathf.Log10(value) * 20;

        soundSFX.audioMixer.SetFloat("sfxVolume", sfxVolume);

        soundSettings.sfxVolume = sfxVolume;

        SaveSystem.SaveSoundSettings(soundSettings);
    }

    public float GetSFXVolume()
    {
        return Mathf.Pow(10, sfxVolume / 20);
    }

    private List<AudioClip> GetBGMAudioClip(BGMSound soundToFind)
    {
        List<AudioClip> list = new List<AudioClip>();

        foreach (GameAssets.BGMSoundAudioClip soundAudioClip in GameAssets.i.bgmSoundAudioArray)
        {
            if(soundAudioClip.soundType == soundToFind)
            {
                list.Add(soundAudioClip.clip);
            }
        }

        return list;
    }

    private List<AudioClip> GetSFXAudioClip(SFXSound soundToFind)
    {
        List<AudioClip> list = new List<AudioClip>();

        foreach (GameAssets.SFXSoundAudioClip soundAudioClip in GameAssets.i.sfxSoundAudioArray)
        {
            if (soundAudioClip.soundType == soundToFind)
            {
                list.Add(soundAudioClip.clip);
            }
        }

        return list;
    }
}
