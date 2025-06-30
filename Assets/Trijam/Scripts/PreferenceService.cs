using System;

using UnityEngine;

public class PreferenceService : IPreferenceService
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    private const string SOUND_VOLUME_KEY = "SoundVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string HIGH_SCORE_KEY = "HighScore";

    public int CurrentLevel
    {
        get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
        set
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value);
            PlayerPrefs.Save();
            OnPreferenceChanged?.Invoke(CURRENT_LEVEL_KEY);
        }
    }

    public float SoundVolume
    {
        get => PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1f);
        set
        {
            PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, Mathf.Clamp01(value));
            PlayerPrefs.Save();
            OnPreferenceChanged?.Invoke(SOUND_VOLUME_KEY);
        }
    }

    public float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        set
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, Mathf.Clamp01(value));
            PlayerPrefs.Save();
            OnPreferenceChanged?.Invoke(MUSIC_VOLUME_KEY);
        }
    }

    public int HighScore
    {
        get => PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        set
        {
            if (value > HighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, value);
                PlayerPrefs.Save();
                OnPreferenceChanged?.Invoke(HIGH_SCORE_KEY);
            }
        }
    }

    public event Action<string> OnPreferenceChanged;

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        OnPreferenceChanged?.Invoke("ALL");
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        OnPreferenceChanged?.Invoke(key);
    }
}

public interface IPreferenceService
{
    int CurrentLevel { get; set; }
    float SoundVolume { get; set; }
    float MusicVolume { get; set; }
    int HighScore { get; set; }
    event Action<string> OnPreferenceChanged;
    void ResetAll();
    void DeleteKey(string key);
}