using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
    [System.Serializable]
    public struct PoinData
    {
        public Vector3 pos;
        public Vector3 cPoint;
    }
    
    List<PoinData> pointDataList = new List<PoinData>();
    List<Vector3> curveDataList = new List<Vector3>();

    [SerializeField] int splitCount = 10;

    #region Editor Mode
    [Space]
    [Header("Editor Settings")]
    [SerializeField] Transform target = null;
    [SerializeField] bool syncWithTarget = false;
    [SerializeField] bool createCurve = false;
    [SerializeField] bool updateCurve = false;

    [Space]
    [SerializeField] bool drawPoints = false;
    [SerializeField] bool drawControls = false;
    [SerializeField] bool drawLines = false;
    [SerializeField] bool drawCPLine = false;
    [SerializeField] bool drawCurve = false;

    bool isUpdated;
    Vector3 drawSize = Vector3.one * 0.2f;
    private void OnDrawGizmos()
    {
        if (syncWithTarget && target != null)
        {
            if (target.hasChanged || !isUpdated)
            {
                for (int i = 0; i < target.childCount; i++)
                {
                    Transform targetPoint = target.GetChild(i);
                    PoinData pData = new PoinData();
                    pData.pos = targetPoint.position;
                    if (targetPoint.childCount > 0)
                    {
                        Transform contPoint = targetPoint.GetChild(0);
                        pData.cPoint = contPoint.position;
                    }
                    else
                    {
                        pData.cPoint = pData.pos;
                    }

                    if (pointDataList.Count == i)
                    {
                        pointDataList.Add(pData);
                    }
                    else
                    {
                        pointDataList[i] = pData;
                    }
                }
                if (pointDataList.Count > target.childCount)
                {
                    pointDataList.RemoveRange(target.childCount, pointDataList.Count - target.childCount);
                }
                isUpdated = true;
            }
        }

        if (createCurve || updateCurve)
        {
            createCurve = false;
            curveDataList.Clear();
            for (int i = 0; i < pointDataList.Count-1; i++)
            {
                curveDataList.AddRange(Bezier_QuadCurvePoints(pointDataList[i].pos, pointDataList[i].cPoint, pointDataList[i + 1].pos, splitCount));
            }
        }

        if (drawCurve)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < curveDataList.Count-1; i++)
            {
                Gizmos.DrawLine(curveDataList[i], curveDataList[i + 1]);
            }
        }

        for (int i = 0; i < pointDataList.Count; i++)
        {
            Gizmos.color = Color.white;
            if (drawPoints)
            {
                Gizmos.DrawCube(pointDataList[i].pos, drawSize);
            }
            if (drawLines && i < pointDataList.Count-1)
            {
                Gizmos.DrawLine(pointDataList[i].pos, pointDataList[i + 1].pos);
            }

            Gizmos.color = Color.yellow;
            if (drawControls)
            {
                Gizmos.DrawCube(pointDataList[i].cPoint, drawSize);
            }
            if (drawCPLine && i < pointDataList.Count - 1)
            {
                Gizmos.DrawLine(pointDataList[i].pos, pointDataList[i].cPoint);
                Gizmos.DrawLine(pointDataList[i].cPoint, pointDataList[i+1].pos);
            }
        }
    }
    #endregion


    public Vector3[] Bezier_QuadCurvePoints(Vector3 p1, Vector3 p2, Vector3 p3, int splits)
    {
        Vector3[] res = new Vector3[splits];
        float delta = 1f / (splits - 1);
        float dist = 0;
        for (int i = 0; i < (splits - 1); i++)
        {
            res[i] = Bezier_QuadCurvePoint(p1, p2, p3, dist);
            dist += delta;
        }
        res[splits - 1] = Bezier_QuadCurvePoint(p1, p2, p3, 1);
        return res;
    }

    public Vector3 Bezier_QuadCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 res = Vector3.zero;
        res.x = (1 - t) * (1 - t) * p1.x + 2 * (1 - t) * t * p2.x + t * t * p3.x;
        res.y = (1 - t) * (1 - t) * p1.y + 2 * (1 - t) * t * p2.y + t * t * p3.y;
        res.z = (1 - t) * (1 - t) * p1.z + 2 * (1 - t) * t * p2.z + t * t * p3.z;
        return res;
    }
}

