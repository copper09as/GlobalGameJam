using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMove : MonoBehaviour
{
    public SpriteRenderer sr;
    public SpriteRenderer sr_Parent;
    public GameObject child;
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr_Parent = GetComponentInParent<SpriteRenderer>();
        sr.sprite = sr_Parent.sprite;//���þ���
        sr.color = new Color(0, 0, 0, 0.5f);//������ɫ
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        SetShadow();//ʵʱ������Ӱλ��
    }
    void SetShadow()
    {
        float angle  = Vector2.SignedAngle(GetDirection(), Sun.Instance.GetDirection(transform));//���������Ƕ�
        angle = angle - transform.rotation.eulerAngles.z;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, angle);//������Ӱ��ת
    }
    Vector3 GetDirection()
    {

        return child.transform.localPosition;
    }
}
