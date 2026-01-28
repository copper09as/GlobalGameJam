using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    [SerializeField]CustomToggle fullscreenToggle;
    void Start()
    {
        fullscreenToggle.toggleObject.onValueChanged.AddListener(SetFullscreen);
        fullscreenToggle.toggleObject.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.UpdateState();

    }
    public void SetResolutionByString(string resolution)
    {
        // 例："1920x1080"
        if (string.IsNullOrEmpty(resolution))
            return;

        string[] parts = resolution.Split('x');
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid resolution string: {resolution}");
            return;
        }

        if (int.TryParse(parts[0], out int width) &&
            int.TryParse(parts[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            Debug.Log($"Resolution set to {width}x{height}");
        }
        else
        {
            Debug.LogError($"Failed to parse resolution: {resolution}");
        }
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }
}
