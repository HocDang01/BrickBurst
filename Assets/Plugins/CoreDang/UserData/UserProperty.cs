using UnityEngine;

public class UserProperty
{
    public static bool MusicOn;
    public static bool SfxOn;
    public static bool HapticOn;

    public static void Save()
    {
        PlayerPrefs.SetInt("MusicOn", MusicOn ? 1 : 0);
        PlayerPrefs.SetInt("SfxOn", SfxOn ? 1 : 0);
        PlayerPrefs.SetInt("HapticOn", HapticOn ? 1 : 0);

        PlayerPrefs.Save(); // force save xuống disk
    }

    public static void Load()
    {
        MusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        SfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
        HapticOn = PlayerPrefs.GetInt("HapticOn", 1) == 1;
    }
}