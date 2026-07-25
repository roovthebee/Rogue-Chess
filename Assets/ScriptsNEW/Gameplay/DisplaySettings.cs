using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private int width = 1920;

    [SerializeField] private int height = 1080;

    // Unity Messages

    private void Awake()
    {
        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }
}