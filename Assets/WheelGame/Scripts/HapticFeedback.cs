using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public static class HapticFeedback
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _TriggerImpactFeedback(int style);

    [DllImport("__Internal")]
    private static extern void _TriggerNotificationFeedback(int type);

    [DllImport("__Internal")]
    private static extern void _TriggerSelectionFeedback();
#endif

    public static void Light()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerImpactFeedback(0);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Medium()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerImpactFeedback(1);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Heavy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerImpactFeedback(2);
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void Success()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerNotificationFeedback(0);
#endif
    }

    public static void Warning()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerNotificationFeedback(1);
#endif
    }

    public static void Error()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerNotificationFeedback(2);
#endif
    }

    public static void Selection()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerSelectionFeedback();
#endif
    }
}
