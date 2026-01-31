using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGlowEffect : MonoBehaviour
{
   /// <summary>
   /// 物体发光材质
   /// </summary>
     public Material glowMaterial;  
    public float glowSpeed = 2f;   // 发光效果的速度
    private Renderer objectRenderer;
    private float glowIntensity = 0f;  // 发光强度

    void Start()
    {
        // 获取物体的 Renderer 组件
        objectRenderer = GetComponent<Renderer>();

        // 确保物体有发光材质
        if (glowMaterial == null && objectRenderer != null)
        {
            glowMaterial = objectRenderer.material;
        }
    }

    void Update()
    {
        // 从内到外的发光效果（增大发光强度）
        if (glowMaterial != null)
        {
            glowIntensity += glowSpeed * Time.deltaTime;

            // 让发光强度在 0 到 1 之间循环变化
            if (glowIntensity > 1f)
            {
                glowIntensity = 0f;  // 重置到起始状态
            }

            // 将发光强度应用到材质的发光属性上
            glowMaterial.SetFloat("_GlowIntensity", glowIntensity);
        }
    }
}