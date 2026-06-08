using UnityEngine;

public static class PlayerPrefsManager
{
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat("MusicVolume", 0.25f);
        set => PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public static float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat("MouseSensitivity", 0.25f);
        set => PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public static PlayerSkin PlayerSkin
    {
        get => (PlayerSkin)PlayerPrefs.GetInt("PlayerSkin", 0);
        set => PlayerPrefs.SetInt("PlayerSkin", (int)value);
    }

    public static Vector2 PatternOffsetSpeed
    {
        get => new(PlayerPrefs.GetFloat("PatternOffsetSpeedX", 0.5f), PlayerPrefs.GetFloat("PatternOffsetSpeedY", -0.5f));
        set
        {
            PlayerPrefs.SetFloat("PatternOffsetSpeedX", value.x);
            PlayerPrefs.SetFloat("PatternOffsetSpeedY", value.y);
        }
    }

    public static float PatternTransparency
    {
        get => PlayerPrefs.GetFloat("PatternTransparency", 0.8f);
        set => PlayerPrefs.SetFloat("PatternTransparency", value);
    }

    public static float PlayerSkinRotationSpeed
    {
        get => PlayerPrefs.GetFloat("PlayerSkinRotationSpeed", 270);
        set => PlayerPrefs.SetFloat("PlayerSkinRotationSpeed", value);
    }

    public static int LevelsCompleted
    {
        get => PlayerPrefs.GetInt("LevelsCompleted", 0);
        set => PlayerPrefs.SetInt("LevelsCompleted", value);
    }

    public static int SelectedLevel
    {
        get => PlayerPrefs.GetInt("SelectedLevel", 0);
        set => PlayerPrefs.SetInt("SelectedLevel", value);
    }
}
