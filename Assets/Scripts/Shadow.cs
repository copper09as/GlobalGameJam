using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public SpriteRenderer sr;
    public SpriteRenderer sr_Parent;
    public GameObject parent;
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr_Parent = GetComponentInParent<SpriteRenderer>();
        sr.sprite = sr_Parent.sprite;//设置精灵
        sr.color = new Color(0, 0, 0, 0.5f);//设置颜色
    }
    // Update is called once per frame
    void Update()
    {
        SetShadow();//实时更新阴影位置
    }

    void SetShadow()
    {
        float angle  = Vector2.SignedAngle(GetDirection(), Sun.Instance.GetDirection(transform));//计算阴影旋转角度
        sr.transform.rotation = sr.transform.rotation * Quaternion.Euler(0, 0, angle);//设置阴影旋转
    }

    Vector3 GetDirection()
    {
        return transform.position-transform.parent.position;
    }
}
