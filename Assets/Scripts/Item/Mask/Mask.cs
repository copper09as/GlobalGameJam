using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player"))
        { 
            BeUsed(collision.gameObject);
        }
    }

    public virtual void BeUsed(GameObject Player)
    {
        
    }
}
