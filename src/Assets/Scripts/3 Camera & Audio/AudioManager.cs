using UnityEngine;
using UnityEngine.Audio;

// OBJECTIVE 35
public class AudioManager : MonoBehaviour
{
    public static AudioManager g { get; private set; }

    [SerializeField] private AudioMixerGroup Master;

    [SerializeField][Space] private Sound[] sounds;

    private void Awake()
    {
        if (g == null)
        {
            g = GetComponent<AudioManager>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = Master;
        }
    }

    private Sound FindSound(string name) // Find a Sound instance by name
    {
        Sound s = null;
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s == null)
        {
            Debug.LogWarning($"Sound {name} not found");
            return null;
        }
        return s;
    }

    public void Play(string name) // Play a sound by name
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            Debug.LogWarning("Tried to play null sound");
        }
        else if ((!s.source.isPlaying && !s.multipleSounds) || s.multipleSounds)
        {
            s.source.clip = s.clips[Random.Range(0, s.clips.Length - 1)];
            s.source.Play();
        }
    }

    public void PlayNewClip(string name) // Used when you want to play a random new clip from a Sound
    {
        Sound s = FindSound(name);

        if (s == null)
        {
            Debug.LogError("Tried to play null sound");
            return;
        }
        if (s.clips.Length == 1)
        {
            Debug.LogWarning("Likely using wrong method; use play instead");
        }
        s.source.clip = s.clips[Random.Range(0, s.clips.Length - 1)];
        s.source.Play();
    }

    public AudioClip GetClip(string name) // Get the current clip from a sound by name
    {
        return FindSound(name).source.clip;
    }

    public void Stop(string name) // Stop the current sound from playing
    {
        Sound s = FindSound(name);
        s.source.Stop();
    }

    public bool IsPlaying(string name) // Check if sound is playing
    {
        AudioSource s = FindSound(name).source;
        return s.isPlaying;
    }

    public void ChangeMasterVolume(float v) // Change master volume between 0 & 1
    { // OBJECTIVE 36
        if (v < 0f || v > 1f)
        {
            Debug.LogError($"Trying to make master volume {v}");
            return;
        }
        AudioListener.volume = v;
    }

    public float GetMasterVolume() // Gets master volume
    {
        return AudioListener.volume;
    }
}
