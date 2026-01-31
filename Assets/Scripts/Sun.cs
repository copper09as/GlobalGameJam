using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static Sun instance;
    public static Sun Instance//È«¾Öµ¥Àý
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
