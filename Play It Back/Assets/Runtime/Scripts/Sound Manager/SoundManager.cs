using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Sounds
{
    public string ClipName;
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume;
    [Range(0f, 1f)]
    public float Pitch;
    [Range(0f, 1f)]
    public float SpatialBlend;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance => s_instance;
    private static SoundManager s_instance;

    [SerializeField]
    private bool _dontDestroyOnLoad = false;
    [SerializeField]
    private List<Sounds> _sounds = new List<Sounds>();


    private Dictionary<string, AudioSource> _audioSources = new Dictionary<string, AudioSource>();
    private GlobalEventSystem _eventSystem = null;

    private void Awake()
    {
        if (Instance == null)
        {
            s_instance = this;
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        loadUpAudioSources();
        _eventSystem = GameObject.FindGameObjectWithTag("Event System").GetComponent<GlobalEventSystem>();
    }

    private void loadUpAudioSources()
    {
        foreach (var sound in _sounds)
        {
            var gO = new GameObject($"AS:{sound.ClipName}", typeof(AudioSource));
            var aS = gO.GetComponent<AudioSource>();

            gO.transform.SetParent(transform, true);

            aS.clip = sound.Clip;
            aS.playOnAwake = false;
            aS.volume = sound.Volume;
            aS.pitch = sound.Pitch;
            aS.spatialBlend = sound.SpatialBlend;

            _audioSources.Add(sound.ClipName, aS);
        }
    }

    private bool isValid(string clipName)
    {
        if (!string.IsNullOrEmpty(clipName) && _audioSources.ContainsKey(clipName))
            return true;

        print($"Audio Clip : {clipName}, not found");
        return false;
    }

    public void PlaySound(string clipName)
    {
        if (isValid(clipName))
            _audioSources[clipName].Play();
    }

    public void PlaySoundDelayed(string clipName, float delay)
    {
        if (isValid(clipName))
            _audioSources[clipName].PlayDelayed(delay);
    }
}
