using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音效管理器 - 管理游戏中所有音频播放
/// 支持程序化音效和预加载音效
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    [Range(0f, 1f)]
    public float voiceVolume = 1f;

    private Dictionary<string, AudioClip> soundCache;
    private List<AudioSource> sfxPool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            soundCache = new Dictionary<string, AudioClip>();
            sfxPool = new List<AudioSource>();
            
            // 创建音频源
            CreateAudioSources();
            
            // 预加载音效
            PreloadSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateAudioSources()
    {
        // 背景音乐源
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.parent = transform;
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // 音效源
        GameObject sfxObj = new GameObject("SFXSource");
        sfxObj.transform.parent = transform;
        sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        // 语音源
        GameObject voiceObj = new GameObject("VoiceSource");
        voiceObj.transform.parent = transform;
        voiceSource = voiceObj.AddComponent<AudioSource>();
        voiceSource.loop = false;
        voiceSource.playOnAwake = false;

        // 创建音效池
        for (int i = 0; i < 5; i++)
        {
            GameObject poolObj = new GameObject($"SFXPool_{i}");
            poolObj.transform.parent = transform;
            AudioSource poolSource = poolObj.AddComponent<AudioSource>();
            poolSource.loop = false;
            poolSource.playOnAwake = false;
            sfxPool.Add(poolSource);
        }
    }

    void PreloadSounds()
    {
        // 预生成常用音效
        soundCache["click"] = GenerateClickSound();
        soundCache["select"] = GenerateSelectSound();
        soundCache["attack"] = GenerateAttackSound();
        soundCache["hit"] = GenerateHitSound();
        soundCache["victory"] = GenerateVictorySound();
        soundCache["defeat"] = GenerateDefeatSound();
        soundCache["coin"] = GenerateCoinSound();
        soundCache["draw"] = GenerateDrawSound();
    }

    #region 音效播放方法

    public void PlayClick()
    {
        PlaySFX("click");
    }

    public void PlaySelect()
    {
        PlaySFX("select");
    }

    public void PlayAttack()
    {
        PlaySFX("attack");
    }

    public void PlayHit()
    {
        PlaySFX("hit");
    }

    public void PlayVictory()
    {
        PlaySFX("victory");
        PlayMusic("victory");
    }

    public void PlayDefeat()
    {
        PlaySFX("defeat");
    }

    public void PlayCoin()
    {
        PlaySFX("coin");
    }

    public void PlayDraw()
    {
        PlaySFX("draw");
    }

    public void PlayCritical()
    {
        PlaySFX("critical");
    }

    public void PlaySkill()
    {
        PlaySFX("skill");
    }

    #endregion

    #region 通用播放方法

    public void PlaySFX(string soundName)
    {
        if (soundCache.ContainsKey(soundName))
        {
            // 使用音效池播放
            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.clip = soundCache[soundName];
                source.volume = sfxVolume;
                source.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = sfxVolume;
            source.Play();
        }
    }

    public void PlayMusic(string musicName)
    {
        if (soundCache.ContainsKey(musicName))
        {
            musicSource.clip = soundCache[musicName];
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    #endregion

    #region 程序化音效生成

    /// <summary>
    /// 生成点击音效
    /// </summary>
    AudioClip GenerateClickSound()
    {
        int sampleRate = 44100;
        float duration = 0.05f;
        AudioClip clip = AudioClip.Create("click", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = Mathf.Sin(2 * Mathf.PI * 800 * t) * Mathf.Exp(-t * 50);
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成选中音效
    /// </summary>
    AudioClip GenerateSelectSound()
    {
        int sampleRate = 44100;
        float duration = 0.08f;
        AudioClip clip = AudioClip.Create("select", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float freq = 400 + 200 * (1 - t / duration);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * Mathf.Exp(-t * 20);
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成攻击音效
    /// </summary>
    AudioClip GenerateAttackSound()
    {
        int sampleRate = 44100;
        float duration = 0.15f;
        AudioClip clip = AudioClip.Create("attack", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float noise = Random.Range(-1f, 1f) * 0.3f;
            float tone = Mathf.Sin(2 * Mathf.PI * 200 * t) * 0.7f;
            samples[i] = (noise + tone) * Mathf.Exp(-t * 15);
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成命中音效
    /// </summary>
    AudioClip GenerateHitSound()
    {
        int sampleRate = 44100;
        float duration = 0.1f;
        AudioClip clip = AudioClip.Create("hit", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float noise = Random.Range(-1f, 1f);
            samples[i] = noise * Mathf.Exp(-t * 30) * 0.8f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成胜利音效
    /// </summary>
    AudioClip GenerateVictorySound()
    {
        int sampleRate = 44100;
        float duration = 1f;
        AudioClip clip = AudioClip.Create("victory", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        float[] notes = { 523f, 659f, 784f, 1047f }; // C5, E5, G5, C6
        
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float noteIndex = Mathf.Floor(t * 4);
            float noteT = (t * 4) % 1f;
            
            float freq = notes[Mathf.Min((int)noteIndex, notes.Length - 1)];
            float wave = Mathf.Sin(2 * Mathf.PI * freq * t);
            
            float envelope = 1f;
            if (noteT > 0.8f) envelope = 1f - (noteT - 0.8f) / 0.2f;
            
            samples[i] = wave * envelope * 0.3f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成失败音效
    /// </summary>
    AudioClip GenerateDefeatSound()
    {
        int sampleRate = 44100;
        float duration = 0.8f;
        AudioClip clip = AudioClip.Create("defeat", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float freq = 300f * Mathf.Exp(-t * 2f);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * Mathf.Exp(-t * 3f) * 0.5f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成获得金币音效
    /// </summary>
    AudioClip GenerateCoinSound()
    {
        int sampleRate = 44100;
        float duration = 0.12f;
        AudioClip clip = AudioClip.Create("coin", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float freq = 1200f + 800f * (1 - t / duration);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * Mathf.Exp(-t * 30);
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// 生成抽卡音效
    /// </summary>
    AudioClip GenerateDrawSound()
    {
        int sampleRate = 44100;
        float duration = 0.5f;
        AudioClip clip = AudioClip.Create("draw", (int)(sampleRate * duration), 1, sampleRate, false);
        
        float[] samples = new float[clip.samples];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float noise = Random.Range(-1f, 1f) * 0.2f;
            float sparkle = Mathf.Sin(2 * Mathf.PI * (2000f + 1000f * Mathf.Sin(t * 20f)) * t) * 0.3f;
            samples[i] = (noise + sparkle) * Mathf.Exp(-t * 3f);
        }
        
        clip.SetData(samples, 0);
        return clip;
    }

    #endregion

    AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }
        
        // 如果所有源都在播放，创建新的
        GameObject newObj = new GameObject("SFXPool_Dynamic");
        newObj.transform.parent = transform;
        AudioSource newSource = newObj.AddComponent<AudioSource>();
        newSource.loop = false;
        sfxPool.Add(newSource);
        
        return newSource;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void MuteAll(bool mute)
    {
        if (musicSource != null) musicSource.mute = mute;
        if (sfxSource != null) sfxSource.mute = mute;
        foreach (var source in sfxPool)
            source.mute = mute;
        if (voiceSource != null) voiceSource.mute = mute;
    }
}
