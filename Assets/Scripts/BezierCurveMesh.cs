using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
    [System.Serializable]
    public struct PoinData
    {
        public Vector3 pos;
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
                    Transform t = target.GetChild(i);
                    PoinData pData = new PoinData();
                    if (pointList.Count == i)
                    {
                        pData.pos = t.position;
                        pointList.Add(pData);
                    }
                    else
                    {
                        pData.pos = t.position;
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
