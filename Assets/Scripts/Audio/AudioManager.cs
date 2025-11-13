using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Global audio controller that manages pooled SFX sources and a dedicated music source.
/// Drop one instance into your bootstrap scene and mark it DontDestroyOnLoad.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class NamedClip
    {
        public string id;
        public AudioClip clip;
    }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float musicFadeSeconds = 0.35f;

    [Header("SFX Pool")]
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private int initialPoolSize = 6;
    [SerializeField] private AudioMixerGroup sfxMixer;

    [Header("Library")]
    [SerializeField] private List<NamedClip> clips = new List<NamedClip>();

    private readonly Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private readonly List<AudioSource> activeSources = new List<AudioSource>();
    private readonly Dictionary<string, AudioClip> clipLookup = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookup();
        PreparePool();
        EnsureMusicSource();
    }

    void Update()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = activeSources[i];
            if (!source.isPlaying)
            {
                activeSources.RemoveAt(i);
                availableSources.Enqueue(source);
            }
        }
    }

    public void PlaySFX(string id, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (!clipLookup.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"AudioManager missing clip with id '{id}'.", this);
            return;
        }

        PlaySFX(clip, position, volume, pitch);
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource source = GetPooledSource();
        source.transform.position = position;
        source.volume = volume;
        source.pitch = pitch;
        source.clip = clip;
        source.Play();

        activeSources.Add(source);
    }

    public void PlaySFX(string id, float volume = 1f, float pitch = 1f)
    {
        PlaySFX(id, Vector3.zero, volume, pitch);
    }

    public AudioClip GetClip(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        clipLookup.TryGetValue(id, out AudioClip clip);
        return clip;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || musicSource.clip == clip)
        {
            return;
        }

        StartCoroutine(FadeMusicRoutine(clip, loop));
    }

    public void SetGlobalVolume(float normalizedVolume)
    {
        AudioListener.volume = Mathf.Clamp01(normalizedVolume);
    }

    private void BuildLookup()
    {
        clipLookup.Clear();
        foreach (NamedClip entry in clips)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.id) || entry.clip == null)
            {
                continue;
            }

            clipLookup[entry.id] = entry.clip;
        }
    }

    private void PreparePool()
    {
        if (sfxSourcePrefab == null)
        {
            sfxSourcePrefab = new GameObject("SFX_AudioSource").AddComponent<AudioSource>();
            sfxSourcePrefab.playOnAwake = false;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            AudioSource instance = Instantiate(sfxSourcePrefab, transform);
            ConfigureSource(instance);
            availableSources.Enqueue(instance);
        }
    }

    private void EnsureMusicSource()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Audio Source");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
    }

    private AudioSource GetPooledSource()
    {
        if (availableSources.Count == 0)
        {
            AudioSource extra = Instantiate(sfxSourcePrefab, transform);
            ConfigureSource(extra);
            availableSources.Enqueue(extra);
        }

        AudioSource source = availableSources.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    private void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.outputAudioMixerGroup = sfxMixer != null ? sfxMixer : source.outputAudioMixerGroup;
    }

    private System.Collections.IEnumerator FadeMusicRoutine(AudioClip newClip, bool loop)
    {
        float elapsed = 0f;
        float initialVolume = musicSource.volume;

        while (elapsed < musicFadeSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(initialVolume, 0f, elapsed / musicFadeSeconds);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();

        elapsed = 0f;
        while (elapsed < musicFadeSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, initialVolume, elapsed / musicFadeSeconds);
            yield return null;
        }

        musicSource.volume = initialVolume;
    }
}
