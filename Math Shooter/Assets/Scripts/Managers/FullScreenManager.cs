using UnityEngine;
using System.Runtime.InteropServices;

public class FullscreenManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void EntrarFullscreen();

    public static FullscreenManager instance;

    void Awake()
    {
        instance = this;
    }

    public void AtivarFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        EntrarFullscreen();
#endif
    }
}