using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
    [SerializeField] List<Vector3> pointList;

    #region Editor Mode
    [Space]
    [Header("Editor Settings")]
    [SerializeField] Transform target;
    [SerializeField] bool syncWithTarget;
    bool isUpdated;

    [Space]
    [SerializeField] bool drawPoints;
    [SerializeField] bool drawLines;

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
                    if (pointList.Count == i)
                    {
                        pointList.Add(t.position);
                    }
                    else
                    {
                        pointList[i] = t.position;
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
                Gizmos.DrawCube(p, drawSize);
            }
        }
        if (drawLines)
        {
            for (int i = 0; i < pointList.Count-1; i++)
            {
                Gizmos.DrawLine(pointList[i], pointList[i + 1]);
            }
        }
    }
    #endregion
}
