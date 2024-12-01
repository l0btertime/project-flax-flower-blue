using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
    public bool fadeOnStop = false;
    public bool StopOnLoad = true;
    public AudioSource source;
    [HideInInspector]
    public Coroutine fade;

    public Sound()
    {

    }
    public Sound(AudioClip ac, AudioSource aso, float v, float p, bool l)
    {
        clip = ac;
        source = aso;
        volume = v;
        pitch = p;
        loop = l;
    }

    public void AddSoundComponent(GameObject go)
    {
        source = go.gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager sharedInstance;
    [Range(0, 1)]
    public float masterVolume = 1f;
    public List<Sound> Sounds = new List<Sound>();
    // Start is called before the first frame update
    private void Awake()
    {
        if(sharedInstance == null)
        {
            sharedInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        /*
        foreach(Sound s in Sounds)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        */
    }
    void Start()
    {
        //Play("Music");
    }

    public static void SetMasterVolume(float vol)
    {
        foreach (AudioSource source in sharedInstance.GetComponents<AudioSource>())
        {
            source.volume = (source.volume / sharedInstance.masterVolume) * Mathf.Clamp(vol, 0f, 1f);
        }
        sharedInstance.masterVolume = Mathf.Clamp(vol, 0f, 1f);
    }
    public static AudioSource GetSource(string name)
    {
        Sound s = sharedInstance.Sounds.Find(sound => sound.name == name);
        return s.source;
    }
    public static void Play(string name)
    {
        Sound s = sharedInstance.Sounds.Find(sound => sound.name == name);
        if (s.source == null)
        {
            s.AddSoundComponent(sharedInstance.gameObject);
            s.source.volume = s.source.volume * sharedInstance.masterVolume;
            s.source.Play();
            if (!s.loop) Destroy(s.source, s.clip.length);
        }
        else
        {
            s.source.Play();
        }
        s.source.Play();
    }

    // pausing has been temporarily removed as it really doesn't work with the new system
    /* 
    public static void Pause(string name)
    {
        Sound s = sharedInstance.Sounds.Find(sound => sound.name == name);
        if (s.source == null) return;
        s.source.Stop();
    }
    */

    public static void Stop(string name) {
        Sound s = sharedInstance.Sounds.Find(sound => sound.name == name);
        if (s.source == null) return;
        if (!s.fadeOnStop)
        {
            Destroy(s.source);
            s.source = null;
        }
        else if (s.fade == null)
        {
            s.fade = sharedInstance.StartCoroutine("FadeOut", s);
        }
    }

    public static void StopAllSounds()
    {
        foreach(Sound s in sharedInstance.Sounds)
        {
            s.source.Stop();
        }
    }
    /*
    public static void PauseAllSounds()
    {
        foreach (Sound s in sharedInstance.Sounds)
        {
            s.source.Pause();
        }
    }
    */


    IEnumerator FadeOut(Sound s)
    {
        while (s.source.volume > 0.1f)
        {
            s.source.volume -= 0.05f * s.volume;
            yield return new WaitForSeconds(0.05f);
        }
    
        s.fade = null;
        Destroy(s.source);
        s.source = null;
        yield break;
    }

    private void OnLevelWasLoaded(int level)
    {
        foreach(Sound s in Sounds)
        {
            if (s.StopOnLoad && s.source != null) s.source.Stop();
        }
    }

}
