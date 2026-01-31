using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class Wall : MonoBehaviour, IBeAttacked
{

    private int hp = 3;

    public void OnBeAttacked(Bullet bullet, Vector3 moveDir)
    {
        hp--;
        if (hp<=0)
        {
            BeDestroyed();
        }
    }

    void Start()
    {
        Debug.Log(GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().Walls + "有没有报错");
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().Walls.Add(this);
    }

    
    void BeDestroyed()
    {
        if(Random.Range(1,5) ==1 ) 
        {
            var collection =GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().maskCollection;
            GreatMask(collection.MaskDataList[Random.Range(0, collection.MaskDataList.Count)]);
        }
        Destroy(gameObject);
    }

    private void GreatMask(MaskSO maskSO)
    {
        var mask = Instantiate(Resources.Load<GameObject>("Prefabs/Mask"));
        mask.GetComponent<Mask>().MaskSO = maskSO;
        mask.transform.position = transform.position;
    }

    private void OnDestroy()
    {
        GameEntry.Instance.GetSystem<ContextSystem>().GetContext<SessionContext>().Walls.Remove(this);
    }
}
