using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public  class Gun : ScriptableObject
{
    [Header("基础参数")]
    public float fireCooldown = 0.2f;   // 射击冷却
    public float bulletSpeed = 10f;     // 子弹速度
     public int Price;   
    public string GunName;
    public int Id;
    public int bulletCount = 5;        // 弹夹容量
    public float reloadTime = 1.6f;     // 换弹时间

    [Header("资源")]
    public GameObject bulletPrefab;
    public AudioClip fireSfx;

    /// <summary>
    /// 开火接口（不同枪实现不同逻辑）
    /// </summary>
    public virtual void Fire(Player owner, Vector3 targetPos)
    {
        
    }
}

