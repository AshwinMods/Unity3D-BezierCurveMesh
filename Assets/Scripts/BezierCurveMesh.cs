using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveMesh : MonoBehaviour
{
	[System.Serializable]
	public struct PoinData
	{
		public Vector3 pos;
		public Vector3 cPoint1;
		public Vector3 cPoint2;
	}

	List<PoinData> pointDataList = new List<PoinData>();
	List<Vector3> curveDataList = new List<Vector3>();

	[SerializeField] int splitCount = 10;
	[SerializeField] float meshHeight = 2f;
	[SerializeField] float meshDepth = 1f;

	#region Editor Mode
	[Space]
	[Header("Editor Settings")]
	[SerializeField] Transform target = null;
	[SerializeField] MeshFilter meshTarget;
	[SerializeField] bool syncWithTarget = false;
	[SerializeField] bool createCurve = false;
	[SerializeField] bool updateCurve = false;
	[SerializeField] bool createMesh = false;

	[Space]
	[SerializeField] bool drawPoints = false;
	[SerializeField] bool drawControls = false;
	[SerializeField] bool drawLines = false;
	[SerializeField] bool drawCPLine = false;
	[SerializeField] bool drawCurve = false;

	public enum MeshType { _2D, _2D_TOP}
	public MeshType meshType;

	public enum UVType
	{
		None,
		VertexBased,
		AxisBased,
		LengthBased,
	}
	public UVType uvType;

	bool isUpdated;
	Mesh mesh = null;
	Vector3 drawSize = Vector3.one * 0.2f;
	private void OnDrawGizmos()
	{
		splitCount = Mathf.Max(2, splitCount);

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
						pData.cPoint1 = contPoint.position;
					}
					else
					{
						pData.cPoint1 = pData.pos;
					}
					if (targetPoint.childCount > 1)
					{
						Transform contPoint = targetPoint.GetChild(1);
						pData.cPoint2 = contPoint.position;
					}
					else
					{
						pData.cPoint2 = pData.pos;
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
			for (int i = 0; i < pointDataList.Count - 1; i++)
			{
				//curveDataList.AddRange(Bezier_QuadCurvePoints(pointDataList[i].pos, pointDataList[i].cPoint1, pointDataList[i + 1].pos, splitCount));
				curveDataList.AddRange(Bezier_CubicCurvePoints(pointDataList[i].pos, pointDataList[i].cPoint1, pointDataList[i + 1].cPoint2, pointDataList[i + 1].pos, splitCount));
			}
		}

		if (createMesh)
		{
			createMesh = false;
			Create_Mesh(meshType);
		}

		if (drawCurve)
		{
			Gizmos.color = Color.blue;
			for (int i = 0; i < curveDataList.Count - 1; i++)
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
			if (drawLines && i < pointDataList.Count - 1)
			{
				Gizmos.DrawLine(pointDataList[i].pos, pointDataList[i + 1].pos);
			}

			Gizmos.color = Color.yellow;
			if (drawControls)
			{
				Gizmos.DrawCube(pointDataList[i].cPoint1, drawSize);
				Gizmos.DrawCube(pointDataList[i].cPoint2, drawSize);
			}
			if (drawCPLine)
			{
				Gizmos.DrawLine(pointDataList[i].pos, pointDataList[i].cPoint1);
				Gizmos.DrawLine(pointDataList[i].pos, pointDataList[i].cPoint2);
				//Gizmos.DrawLine(pointDataList[i].cPoint1, pointDataList[i+1].pos);
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
	
	private Vector3[] Bezier_CubicCurvePoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int splits)
	{
		Vector3[] res = new Vector3[splits];
		float delta = 1f / (splits - 1);
		float dist = 0;
		for (int i = 0; i < (splits - 1); i++)
		{
			res[i] = Bezier_CubicCurvePoint(p1, p2, p3, p4, dist);
			dist += delta;
		}
		res[splits - 1] = Bezier_CubicCurvePoint(p1, p2, p3, p4, 1);
		return res;
	}

	private Vector3 Bezier_CubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		Vector3 res = Vector3.zero;
		res.x = (1 - t) * (1 - t) * (1 - t) * p1.x + 3 * (1 - t) * (1 - t) * t * p2.x + 3 * (1 - t) * t * t * p3.x + t * t * t * p4.x;
		res.y = (1 - t) * (1 - t) * (1 - t) * p1.y + 3 * (1 - t) * (1 - t) * t * p2.y + 3 * (1 - t) * t * t * p3.y + t * t * t * p4.y;
		res.z = (1 - t) * (1 - t) * (1 - t) * p1.z + 3 * (1 - t) * (1 - t) * t * p2.z + 3 * (1 - t) * t * t * p3.z + t * t * t * p4.z;
		return res;
	}

	public void Create_Mesh(MeshType type)
	{
		switch (type)
		{
			case MeshType._2D:
				Create_2DMesh();
				break;
			case MeshType._2D_TOP:
				Create_2DTOP();
				break;
			default:
				break;
		}
	}

	private void Create_2DMesh()
	{
		int numPoints = curveDataList.Count * 4;
		Vector3[] verts = new Vector3[numPoints];
		Vector2[] uvs = new Vector2[numPoints];

		float curvelength = 0;
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			curvelength += Vector3.Distance(curveDataList[i], curveDataList[i + 1]);
		}

		// Vertex DATA Setup
		float u1, u2, covered = 0;
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			verts[i * 4 + 0] = curveDataList[i];
			verts[i * 4 + 1] = curveDataList[i] + Vector3.down * meshHeight;
			verts[i * 4 + 2] = curveDataList[i + 1];
			verts[i * 4 + 3] = curveDataList[i + 1] + Vector3.down * meshHeight;

			switch (uvType)
			{
				case UVType.None:
					uvs[i * 4 + 0] = Vector3.zero;
					uvs[i * 4 + 1] = Vector3.zero;
					uvs[i * 4 + 2] = Vector3.zero;
					uvs[i * 4 + 3] = Vector3.zero;
					break;
				case UVType.VertexBased:
					uvs[i * 4 + 0] = verts[i * 4 + 0];
					uvs[i * 4 + 1] = verts[i * 4 + 1];
					uvs[i * 4 + 2] = verts[i * 4 + 2];
					uvs[i * 4 + 3] = verts[i * 4 + 3];
					break;
				case UVType.AxisBased:
					u1 = (curveDataList[i].x - curveDataList[0].x) / (curveDataList[curveDataList.Count - 1].x - curveDataList[0].x);
					u2 = (curveDataList[i + 1].x - curveDataList[0].x) / (curveDataList[curveDataList.Count - 1].x - curveDataList[0].x);
					uvs[i * 4 + 0] = new Vector2(u1, 1);
					uvs[i * 4 + 1] = new Vector2(u1, 0);
					uvs[i * 4 + 2] = new Vector2(u2, 1);
					uvs[i * 4 + 3] = new Vector2(u2, 0);
					break;
				case UVType.LengthBased:
					u1 = covered / curvelength;
					covered += Vector3.Distance(curveDataList[i], curveDataList[i + 1]);
					u2 = covered / curvelength;
					uvs[i * 4 + 0] = new Vector2(u1, 1);
					uvs[i * 4 + 1] = new Vector2(u1, 0);
					uvs[i * 4 + 2] = new Vector2(u2, 1);
					uvs[i * 4 + 3] = new Vector2(u2, 0);
					break;
				default:
					break;
			}
		}

		// Indices Setup
		int numTris = numPoints - 2;
		int[] indices = new int[numTris * 3];
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			indices[i * 6 + 0] = i * 4 + 0;
			indices[i * 6 + 1] = i * 4 + 2;
			indices[i * 6 + 2] = i * 4 + 1;

			indices[i * 6 + 3] = i * 4 + 1;
			indices[i * 6 + 4] = i * 4 + 2;
			indices[i * 6 + 5] = i * 4 + 3;
		}

		if (mesh == null)
		{
			mesh = new Mesh();
		}
		mesh.Clear();
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = indices;
		mesh.RecalculateBounds();
		meshTarget.mesh = mesh;

	}

	private void Create_2DTOP()
	{
		int numPoints = curveDataList.Count * 6;
		Vector3[] verts = new Vector3[numPoints];
		Vector2[] uvs = new Vector2[numPoints];

		float curvelength = 0;
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			curvelength += Vector3.Distance(curveDataList[i], curveDataList[i + 1]);
		}

		// Vertex DATA Setup
		float u1, u2, covered = 0;
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			verts[i * 6 + 0] = curveDataList[i];
			verts[i * 6 + 1] = curveDataList[i] + Vector3.down * meshHeight;
			verts[i * 6 + 2] = curveDataList[i + 1];
			verts[i * 6 + 3] = curveDataList[i + 1] + Vector3.down * meshHeight;

			verts[i * 6 + 4] = curveDataList[i] + Vector3.forward * meshDepth;
			verts[i * 6 + 5] = curveDataList[i + 1] + Vector3.forward * meshDepth;
		
			switch (uvType)
			{
				case UVType.None:
					uvs[i * 6 + 0] = Vector3.zero;
					uvs[i * 6 + 1] = Vector3.zero;
					uvs[i * 6 + 2] = Vector3.zero;
					uvs[i * 6 + 3] = Vector3.zero;
					uvs[i * 6 + 4] = Vector3.zero;
					uvs[i * 6 + 5] = Vector3.zero;
					break;
				case UVType.VertexBased:
					uvs[i * 6 + 0] = verts[i * 6 + 0];
					uvs[i * 6 + 1] = verts[i * 6 + 1];
					uvs[i * 6 + 2] = verts[i * 6 + 2];
					uvs[i * 6 + 3] = verts[i * 6 + 3];
					uvs[i * 6 + 4] = new Vector2(verts[i * 6 + 4].x, verts[i * 6 + 4].z);
					uvs[i * 6 + 5] = new Vector2(verts[i * 6 + 5].x, verts[i * 6 + 5].z);
					break;
				case UVType.AxisBased:
					u1 = (curveDataList[i].x - curveDataList[0].x) / (curveDataList[curveDataList.Count - 1].x - curveDataList[0].x);
					u2 = (curveDataList[i + 1].x - curveDataList[0].x) / (curveDataList[curveDataList.Count - 1].x - curveDataList[0].x);
					uvs[i * 6 + 0] = new Vector2(u1, 1);
					uvs[i * 6 + 1] = new Vector2(u1, 0);
					uvs[i * 6 + 2] = new Vector2(u2, 1);
					uvs[i * 6 + 3] = new Vector2(u2, 0);
					uvs[i * 6 + 4] = new Vector2(u1, 0);
					uvs[i * 6 + 5] = new Vector2(u2, 0);
					break;
				case UVType.LengthBased:
					u1 = covered / curvelength;
					covered += Vector3.Distance(curveDataList[i], curveDataList[i + 1]);
					u2 = covered / curvelength;
					uvs[i * 6 + 0] = new Vector2(u1, 1);
					uvs[i * 6 + 1] = new Vector2(u1, 0);
					uvs[i * 6 + 2] = new Vector2(u2, 1);
					uvs[i * 6 + 3] = new Vector2(u2, 0);
					uvs[i * 6 + 4] = new Vector2(u1, 0);
					uvs[i * 6 + 5] = new Vector2(u2, 0);

					break;
				default:
					break;
			}
		}

		// Indices Setup
		int numTris = numPoints - 2 - 2;
		int[] indices = new int[numTris * 3];
		for (int i = 0; i < curveDataList.Count - 1; i++)
		{
			indices[i * 12 + 0] = i * 6 + 0;
			indices[i * 12 + 1] = i * 6 + 2;
			indices[i * 12 + 2] = i * 6 + 1;

			indices[i * 12 + 3] = i * 6 + 1;
			indices[i * 12 + 4] = i * 6 + 2;
			indices[i * 12 + 5] = i * 6 + 3;

			indices[i * 12 + 6] = i * 6 + 0;
			indices[i * 12 + 7] = i * 6 + 4;
			indices[i * 12 + 8] = i * 6 + 5;

			indices[i * 12 + 9] = i * 6 + 5;
			indices[i * 12 +10] = i * 6 + 2;
			indices[i * 12 +11] = i * 6 + 0;
		}

		if (mesh == null)
		{
			mesh = new Mesh();
		}
		mesh.Clear();
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = indices;
		mesh.RecalculateBounds();
		meshTarget.mesh = mesh;

	}
}

