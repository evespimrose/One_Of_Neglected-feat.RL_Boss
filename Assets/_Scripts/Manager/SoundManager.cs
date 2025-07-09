using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Collections;
using Cysharp.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>
{
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }

    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private const int EFFECT_SOURCE_COUNT = 128; // 효과음을 위한 AudioSource 풀 크기
    private Queue<AudioSource> _effectSourcePool = new Queue<AudioSource>();
    private Queue<SoundEffect> _effectQueue = new Queue<SoundEffect>();
    private bool _isProcessingQueue;

    private class SoundEffect
    {
        public AudioClip Clip;
        public float Pitch;
        public float Volume;

        public SoundEffect(AudioClip clip, float pitch, float volume)
        {
            Clip = clip;
            Pitch = pitch;
            Volume = volume;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            Init();
        }
    }

    public void Init()
    {
        string[] soundNames = System.Enum.GetNames(typeof(Sound));

        GameObject bgmGo = new GameObject { name = soundNames[(int)Sound.Bgm] };
        _audioSources[(int)Sound.Bgm] = bgmGo.AddComponent<AudioSource>();
        bgmGo.transform.parent = transform;

        GameObject effectRoot = new GameObject { name = "EffectSources" };
        effectRoot.transform.parent = transform;

        for (int i = 0; i < EFFECT_SOURCE_COUNT; i++)
        {
            GameObject go = new GameObject { name = $"EffectSource_{i}" };
            go.transform.parent = effectRoot.transform;
            AudioSource source = go.AddComponent<AudioSource>();
            _effectSourcePool.Enqueue(source);
        }

        _audioSources[(int)Sound.Effect] = _effectSourcePool.Peek();

        _audioSources[(int)Sound.Bgm].loop = true;
        SetMasterVolume(0.5f * 100);
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
        foreach (var source in _effectSourcePool)
        {
            source.clip = null;
            source.Stop();
        }

        _isProcessingQueue = false;
        _effectQueue.Clear();
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        Play(path, type, pitch, true, 1.0f);
    }

    public void Play(string path, Sound type, float pitch, bool loop)
    {
        Play(path, type, pitch, loop, 1.0f);
    }

    public void Play(string path, Sound type, float pitch, bool loop, float volume)
    {
        if (string.IsNullOrEmpty(path))
            return;

        AudioClip audioClip = GetOrAddAudioClip(path, type);
        if (audioClip == null)
            return;

        if (type == Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.volume = volume * masterVolume * bgmVolume;
            audioSource.Play();
        }
        else
        {
            _effectQueue.Enqueue(new SoundEffect(audioClip, pitch, volume));

            if (!_isProcessingQueue)
            {
                ProcessEffectQueue().Forget();
            }
        }
    }

    private AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Using/Audio/") == false)
        {
            path = $"Using/Audio/{path}";
        }

        AudioClip audioClip = null;

        if (type == Sound.Bgm)
        {
            audioClip = Resources.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                if (audioClip != null)
                {
                    _audioClips.Add(path, audioClip);
                }
                else
                {
                    // AudioClip[] allClips = Resources.LoadAll<AudioClip>("Sounds");
                    // foreach (var clip in allClips)
                    // {
                    //     // Debug.Log($"- {clip.name}");
                    // }
                }
            }
        }

        return audioClip;
    }

    public void Stop(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    public void SetVolume(Sound type, float volume)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.volume = volume;
    }

    public float GetVolume(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        return audioSource.volume;
    }

    private float masterVolume = 1f;
    private float effectVolume = 1f;
    private float bgmVolume = 1f;

    public float getMasterVolume()
    {
        return masterVolume;
    }
    public float getEffectVolume()
    {
        return effectVolume;
    }
    public float getBgmVolume()
    {
        return bgmVolume;
    }

    public void SetMasterVolume(float masterAmount)
    {
        masterVolume = masterAmount / 100f;
        AudioSource effectSource = _audioSources[(int)Sound.Effect];
        AudioSource bgmSource = _audioSources[(int)Sound.Bgm];

        effectSource.volume = masterVolume * effectVolume;
        bgmSource.volume = masterVolume * bgmVolume;
    }

    public void SetEffectVolume(float effectAmount)
    {
        effectVolume = effectAmount * 100f;
        AudioSource effectSource = _audioSources[(int)Sound.Effect];
        effectSource.volume = masterVolume * effectVolume;
    }

    public void SetBgmVolume(float bgmAmount)
    {
        bgmVolume = bgmAmount / 100f;
        AudioSource bgmSource = _audioSources[(int)Sound.Bgm];
        bgmSource.volume = masterVolume * bgmVolume;
    }

    private AudioSource GetAvailableEffectSource()
    {
        foreach (var source in _effectSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource oldestSource = _effectSourcePool.Dequeue();
        _effectSourcePool.Enqueue(oldestSource);
        return oldestSource;
    }

    private async UniTaskVoid ProcessEffectQueue()
    {
        _isProcessingQueue = true;

        while (_effectQueue.Count > 0)
        {
            var effect = _effectQueue.Dequeue();

            AudioSource availableSource = GetAvailableEffectSource();
            if (availableSource != null)
            {
                availableSource.pitch = effect.Pitch;
                availableSource.volume = masterVolume * effectVolume;
                availableSource.PlayOneShot(effect.Clip, effect.Volume);

                float delay = Mathf.Max(0.05f, effect.Clip.length * 0.0001f);
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
        }

        _isProcessingQueue = false;
    }
}