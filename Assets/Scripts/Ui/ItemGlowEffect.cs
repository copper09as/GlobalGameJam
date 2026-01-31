using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGlowEffect : MonoBehaviour
{
    /// <summary>
    /// 物品光晕效果脚本
    /// </summary>
    public Material glowMaterial;
    /// <summary>
    /// 最大光晕强度
    /// </summary>
    public float maxGlowIntensity = 5f;
    /// <summary>
    /// 光晕变化速度
    /// </summary>
    public float glowSpeed = 2f;
    private float currentGlowIntensity = 0f;
    void Start()
    {
        glowMaterial.SetFloat("_GlowIntensity", currentGlowIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        // 让光晕强度在最大值之间来回变化
        currentGlowIntensity = Mathf.PingPong(Time.time * glowSpeed, maxGlowIntensity);
        glowMaterial.SetFloat("_GlowIntensity", currentGlowIntensity);
    }
}
