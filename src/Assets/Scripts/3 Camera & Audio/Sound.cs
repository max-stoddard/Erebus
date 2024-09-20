using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip[] clips; // Each sound can take one of many clips, i.e. multiple footsteps etc

    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    public bool multipleSounds;
}
