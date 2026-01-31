using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static Sun instance;
    public float speed = 10f;//太阳当前运行速度
    public float normalSpeed = 10f;//太阳正常速度
    public float shakedSpeed = 50f;//震动时太阳速度
    [SerializeField]
    private SolarOrbit Orbit;//轨道

    private int direction = 1;//运行方向 1为逆时针 -1为顺时针

    private int target = 0;//目标点
    [SerializeField]
    private float shakeDuration = 3f;//震动持续时间
    [SerializeField]
    private float shakeMagnitude = 0.1f;//震动幅度
    [SerializeField]
    private float shaketime = 0f;//震动计时
    public static Sun Instance//全局单例
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public Vector3 GetDirection(Transform entity)
    {
        return entity.position - transform.position;
    }

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("开始震动");
            shaketime = shakeDuration;
            
        }
        //Playing(Time.deltaTime);
        Shake(Time.deltaTime);
        
    }
    //public void Playing(float deltaTime)//太阳在轨道上面运行
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, Orbit.points[target].position, deltaTime * speed);
    //    if(Vector3.Distance(transform.position, Orbit.points[target].position) < 0.1f)
    //    {
    //        target += direction;
    //        if(target >= Orbit.points.Length)
    //        {
    //            target = 0;
    //        }
    //        if(target ==-1)
    //        {
    //            target = Orbit.points.Length - 1;
    //        }
    //    }
    //}

    public void Shake(float deltaTime)
    {
        if(shaketime > 0f)
        {
            shaketime -= deltaTime;

            transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

            speed = shakedSpeed;
        }
        else
        {
            speed = normalSpeed;
        }
    }
    public void SwitchDirection()//切换方向
    {
        direction = - direction;
    }

    public void BeAttacked(Vector3 attack)//被攻击
    {
        Debug.Log("开始震动");
        shaketime = shakeDuration;
        //根据攻击的方向
        var a = Vector2.Dot(attack, GetDirection(Orbit.points[target]));
        if (a < 0f) SwitchDirection();//改变方向
    }

}
