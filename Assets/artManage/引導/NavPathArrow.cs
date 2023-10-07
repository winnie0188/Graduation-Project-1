using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPathArrow : MonoBehaviour
{
    //箭頭3D對象
    public MeshRenderer meshRenderer;
    //路徑點
    public List<Transform> points = new List<Transform>();
    //顯示的路徑
    private List<MeshRenderer> Lines = new List<MeshRenderer>();


    public float xscale = 1f;
    public float yscale = 1f;


    public static NavPathArrow navPathArrow;

    private void Awake()
    {
        navPathArrow = this;
    }

    void Start()
    {
        //箭頭縮放值
        xscale = meshRenderer.transform.localScale.x;
        yscale = meshRenderer.transform.localScale.y;

    }

    void drawPath()
    {
        if (points == null || points.Count <= 1)
        {
            return;
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            drawLine(points[i].position, points[i + 1].position, i);
        }
    }

    public void drawPath(Vector3[] points)
    {
        if (points == null || points.Length <= 1)
        {
            return;
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            drawLine(points[i], points[i + 1], i);
        }
    }

    void drawLine(Vector3 start, Vector3 end, int index)
    {
        MeshRenderer mr;
        if (index >= Lines.Count)
        {
            mr = Instantiate(meshRenderer);
            Lines.Add(mr);
        }
        else
        {
            mr = Lines[index];
        }

        // 限制 end 到最大距离为5的范围内
        // float maxDistance = 2.5f;
        // float distance = Vector3.Distance(start, end);
        // if (distance > maxDistance)
        // {
        //     Vector3 direction = (end - start).normalized;
        //     end = start + direction * maxDistance;
        // }

        start.y = start.y - 0.8126594f;
        end.y = start.y;

        Vector3 direction = (end - start).normalized;
        start = start + direction * 0.5f;
        end = start + direction * 1.5f;



        var tarn = mr.transform;
        var length = Vector3.Distance(start, end);

        tarn.localScale = new Vector3(0.63858f, 0.63858f, 1);
        tarn.position = (start + end) / 2;
        tarn.LookAt(end);
        tarn.Rotate(90, 0, 0);
        mr.material.mainTextureScale = new Vector2(1, length * yscale);
        mr.gameObject.SetActive(true);
    }


    public void openPath()
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            Lines[i].gameObject.SetActive(true);
        }
    }


    public void hidePath()
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            Lines[i].gameObject.SetActive(false);
        }
    }

    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(20, 40, 80, 20), "顯示路徑"))
    //     {
    //         drawPath();
    //     }
    //     if (GUI.Button(new Rect(20, 80, 80, 20), "隱藏路徑"))
    //     {
    //         hidePath();
    //     }
    // }
}
