using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
public class ResolutionController : MonoBehaviour
{
    [SerializeField]CustomToggle fullscreenToggle;
    [SerializeField]CustomToggle vSyncToggle; // 新增：垂直同步 Toggle
    private void Start()
    {
        // 全屏 Toggle
        fullscreenToggle.toggleObject.onValueChanged.AddListener(SetFullscreen);
        fullscreenToggle.toggleObject.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.UpdateState();

        // 垂直同步 Toggle
        vSyncToggle.toggleObject.onValueChanged.AddListener(SetVSync);
        vSyncToggle.toggleObject.isOn = PlayerPrefs.GetInt("vsync", 1) == 1;
        vSyncToggle.UpdateState();
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
   #region VSync
    public void SetVSync(bool isOn)
    {
        // 1 = 开启 VSync, 0 = 关闭
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        PlayerPrefs.SetInt("vsync", isOn ? 1 : 0);
        Debug.Log("VSync " + (isOn ? "Enabled" : "Disabled"));
    }
    #endregion

    #region Frame Rate
    /// <summary>
    /// 设置帧率上限
    /// </summary>
    /// <param name="fps">帧率整数， <=0表示不限帧率</param>
    public void SetFrameRate(int fps)
    {
        if (fps <= 0)
        {
            Application.targetFrameRate = -1; // 不限制
            Debug.Log("Frame rate unlimited");
        }
        else
        {
            Application.targetFrameRate = fps;
            Debug.Log("Frame rate set to " + fps);
        }

        PlayerPrefs.SetInt("framerate", fps);
    }
    #endregion
}
