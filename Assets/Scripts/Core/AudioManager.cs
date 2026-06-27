using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    public List<AudioClip> battleMusic;
    public List<AudioClip> menuMusic;
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip gachaSound;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        GameSettings settings = GameManager.instance.currentSave.settings;
        
        if (musicSource != null)
        {
            musicSource.mute = !settings.music;
            musicSource.volume = settings.musicVolume;
        }
        
        if (sfxSource != null)
        {
            sfxSource.mute = !settings.sfx;
            sfxSource.volume = settings.sfxVolume;
        }
    }

    public void PlayMenuMusic()
    {
        if (menuMusic.Count > 0)
        {
            AudioClip clip = menuMusic[Random.Range(0, menuMusic.Count)];
            PlayMusic(clip);
        }
    }

    public void PlayBattleMusic()
    {
        if (battleMusic.Count > 0)
        {
            AudioClip clip = battleMusic[Random.Range(0, battleMusic.Count)];
            PlayMusic(clip);
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayAttackSound()
    {
        PlaySFX(attackSound);
    }

    public void PlayDamageSound()
    {
        PlaySFX(damageSound);
    }

    public void PlayVictorySound()
    {
        PlaySFX(victorySound);
    }

    public void PlayDefeatSound()
    {
        PlaySFX(defeatSound);
    }

    public void PlayGachaSound()
    {
        PlaySFX(gachaSound);
    }

    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
