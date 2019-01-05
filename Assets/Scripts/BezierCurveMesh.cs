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

    [SerializeField] List<PoinData> pointList = new List<PoinData>();

    #region Editor Mode
    [Space]
    [Header("Editor Settings")]
    [SerializeField] Transform target = null;
    [SerializeField] bool syncWithTarget = false;
    bool isUpdated;

    [Space]
    [SerializeField] bool drawPoints = false;
    [SerializeField] bool drawLines = false;

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

                    if (pointList.Count == i)
                    {
                        pointList.Add(pData);
                    }
                    else
                    {
                        pointList[i] = pData;
                    }
                }
                if (pointList.Count > target.childCount)
                {
                    pointList.RemoveRange(target.childCount, pointList.Count - target.childCount);
                }
                isUpdated = true;
            }
        }

        if (drawPoints)
        {
            foreach (var p in pointList)
            {
                Gizmos.DrawCube(p.pos, drawSize);
            }
        }
        if (drawLines)
        {
            for (int i = 0; i < pointList.Count-1; i++)
            {
                Gizmos.DrawLine(pointList[i].pos, pointList[i + 1].pos);
            }
        }
    }
    #endregion
}
