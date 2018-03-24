using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // To play sound
    // FindObjectOfType<AudioManager>().Play("SoundName");


    [UnityEngine.Serialization.FormerlySerializedAs("mute")]
    public bool Mute = false;
    [UnityEngine.Serialization.FormerlySerializedAs("sounds")]
    public Sound[] Sounds;

    void Awake()
    {
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Theme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null || Mute == true)
        {
            return;
        }
        s.source.Play();
        //Debug.Log("Sound played: " + name);
    }

}
