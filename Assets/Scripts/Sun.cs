using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static Sun instance;
    public float speed = 10f;//太阳运行速度
    [SerializeField]
    private SolarOrbit Orbit;//轨道

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
        return entity.position - transform.localPosition;
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
        Playing(Time.deltaTime);
        Shake(Time.deltaTime);
        
    }
    public void Playing(float deltaTime)//太阳在轨道上面运行
    {
        transform.position = Vector3.MoveTowards(transform.position, Orbit.points[target].position, deltaTime * speed);
        if(Vector3.Distance(transform.position, Orbit.points[target].position) < 0.1f)
        {
            target++;
            if(target >= Orbit.points.Length)
            {
                target = 0;
            }
        }
    }

    public void Shake(float deltaTime)
    {
        if(shaketime > 0f)
        {
            shaketime -= deltaTime;

            transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }
    }
    
}
