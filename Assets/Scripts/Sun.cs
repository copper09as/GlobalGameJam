using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static Sun instance;

    [SerializeField]
    private SolarOrbit Orbit;//轨道

    
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

    public void Playing()//太阳在轨道上面运行
    {

    }

    // Update is called once per frame
    void Update()
    {
        Playing();
    }
}
