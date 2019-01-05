using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
    [SerializeField] List<Vector3> pointList;

    #region Editor Mode
    [Space]
    [Header("Editor Settings")]
    [SerializeField] bool drawPoints;
    [SerializeField] bool drawLines;

    Vector3 drawSize = Vector3.one * 0.2f;
    private void OnDrawGizmos()
    {
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
