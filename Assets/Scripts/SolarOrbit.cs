using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarOrbit : MonoBehaviour
{
    public Transform[] points;
    // Start is called before the first frame update
    void Start()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            points[i] = transform.GetChild(i);
        }
    }


    [SerializeField] private float pointSize = 0.2f;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private Color lineColor = Color.green;
    [SerializeField] private bool drawLines = true;

    void OnDrawGizmos()
    {
        // 保存原始颜色
        Gizmos.color = pointColor;

        // 绘制所有点
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldPos = points[i].position;
            Gizmos.DrawWireSphere(worldPos, pointSize);

            // 显示序号
            UnityEditor.Handles.Label(worldPos, i.ToString());
        }
        if (drawLines && points.Length > 1)
        {
            Gizmos.color = lineColor;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 start = points[i].position;
                Vector3 end = points[(i + 1) % points.Length].position;
                Gizmos.DrawLine(start, end);
            }
        }
    }

}