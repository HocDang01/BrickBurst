using UnityEngine;
public static class Vibrate
{
    /// <summary>
    /// Button Click:       VibrationManager.Play(VibrateType.Selection);
    /// Small Virate:       VibrationManager.Play(VibrateType.Light);
    /// Big Vibrate:        VibrationManager.Play(VibrateType.Medium);
    /// Very big Vibrate:   VibrationManager.Play(VibrateType.Heavy);
    /// Win:                VibrationManager.Play(VibrateType.Success);
    /// Lose:               VibrationManager.Play(VibrateType.Failure);
    /// Custom (android):   VibrationManager.Custom(200); 
    /// </summary>

    private static bool IsEnabled => UserProperty.HapticOn;

    // ===== PUBLIC API =====

    public static void Play(VibrateType type)
    {
        if (!IsEnabled) return;

#if UNITY_EDITOR
        return;
#elif UNITY_ANDROID
        PlayAndroid(type);
#elif UNITY_IOS
        PlayIOS(type);
#endif
    }

    public static void Custom(long milliseconds)
    {
        if (!IsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(milliseconds);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    // ===== ANDROID =====

#if UNITY_ANDROID
    private static void PlayAndroid(VibrateType type)
    {
        long duration = type switch
        {
            VibrateType.Light => 30,
            VibrateType.Medium => 60,
            VibrateType.Heavy => 120,
            VibrateType.Selection => 20,
            VibrateType.Success => 80,
            VibrateType.Failure => 150,
            _ => 50
        };

        VibrateAndroid(duration);
    }

    private static void VibrateAndroid(long milliseconds)
    {
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
        catch
        {
            // Ignore - device may not support vibration
        }
    }
#endif

    // ===== iOS =====

#if UNITY_IOS
    private static void PlayIOS(VibrateType type)
    {
        // Unity built-in (basic)
        Handheld.Vibrate();
    }
#endif
}

public enum VibrateType
{
    Light,          // small
    Medium,         // big
    Heavy,          // 
    Success,        // win
    Failure,        // Fail
    Selection       // Click button
}
