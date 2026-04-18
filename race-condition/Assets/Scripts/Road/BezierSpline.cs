using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierControlPoint
{
	public Vector3 point;
	public Vector3 tangentIn;
	public Vector3 tangentOut;

	public BezierControlPoint(Vector3 position)
	{
		point = position;
		tangentIn = Vector3.zero;
		tangentOut = Vector3.zero;
	}
}

[RequireComponent(typeof(LineRenderer))]
public class BezierSpline : MonoBehaviour
{
	[SerializeField]
	private List<BezierControlPoint> controlPoints = new();

	[SerializeField]
	private bool closed;

	[SerializeField]
	private int curveResolution = 50;

	private LineRenderer lineRenderer;

	public List<BezierControlPoint> ControlPoints() => controlPoints;
	public bool Closed() => closed;
	public int CurveResolution() => curveResolution;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = 0;
		lineRenderer.useWorldSpace = false;
	}

	public int CurveCount() => closed ? controlPoints.Count : Mathf.Max(0, controlPoints.Count - 1);

	public void AddCurve(Vector3 position)
	{
		BezierControlPoint newPoint = new(position);
		controlPoints.Add(newPoint);
		UpdateLineRenderer();
	}

	public void RemoveCurve(int index)
	{
		if (index >= 0 && index < controlPoints.Count)
		{
			controlPoints.RemoveAt(index);
			UpdateLineRenderer();
		}
	}

	public Vector3 GetPoint(float t)
	{
		if (controlPoints.Count < 2)
		{
			return transform.position;
		}

		t = Mathf.Clamp01(t);
		GetCurveIndexAndLocalT(t, out int curveIndex, out float localT);

		return GetPointForCurve(curveIndex, localT);
	}

	public Vector3 GetTangent(float t)
	{
		if (controlPoints.Count < 2)
		{
			return transform.forward;
		}

		t = Mathf.Clamp01(t);
		GetCurveIndexAndLocalT(t, out int curveIndex, out float localT);

		Vector3 p0 = GetPointForCurveIndex(curveIndex, 0);
		Vector3 p1 = GetPointForCurveIndex(curveIndex, 1);
		Vector3 p2 = GetPointForCurveIndex(curveIndex, 2);
		Vector3 p3 = GetPointForCurveIndex(curveIndex, 3);

		return BezierMath.GetTangent(p0, p1, p2, p3, localT);
	}

	public float GetCurvature(float t)
	{
		if (controlPoints.Count < 4)
		{
			return 0f;
		}

		t = Mathf.Clamp01(t);
		int curveIndex;
		float localT;
		GetCurveIndexAndLocalT(t, out curveIndex, out localT);

		Vector3 p0 = GetPointForCurveIndex(curveIndex, 0);
		Vector3 p1 = GetPointForCurveIndex(curveIndex, 1);
		Vector3 p2 = GetPointForCurveIndex(curveIndex, 2);
		Vector3 p3 = GetPointForCurveIndex(curveIndex, 3);

		return BezierMath.GetCurvature(p0, p1, p2, p3, localT);
	}

	public float GetTotalLength()
	{
		float length = 0f;
		float step = 1f / (curveResolution * CurveCount());

		Vector3 lastPoint = GetPoint(0f);
		for (float t = step; t <= 1f; t += step)
		{
			Vector3 currentPoint = GetPoint(t);
			length += Vector3.Distance(lastPoint, currentPoint);
			lastPoint = currentPoint;
		}

		return length;
	}

	private void GetCurveIndexAndLocalT(float t, out int curveIndex, out float localT)
	{
		int curves = CurveCount();
		if (curves == 0)
		{
			curveIndex = 0;
			localT = 0f;
			return;
		}

		float scaledT = t * curves;
		curveIndex = Mathf.FloorToInt(scaledT);
		curveIndex = Mathf.Clamp(curveIndex, 0, curves - 1);
		localT = scaledT - curveIndex;
	}

	private Vector3 GetPointForCurve(int curveIndex, float t)
	{
		Vector3 p0 = GetPointForCurveIndex(curveIndex, 0);
		Vector3 p1 = GetPointForCurveIndex(curveIndex, 1);
		Vector3 p2 = GetPointForCurveIndex(curveIndex, 2);
		Vector3 p3 = GetPointForCurveIndex(curveIndex, 3);

		return BezierMath.GetPoint(p0, p1, p2, p3, t);
	}

	private Vector3 GetPointForCurveIndex(int curveIndex, int pointInCurve)
	{
		int totalPoints = controlPoints.Count;
		if (totalPoints == 0) return transform.position;

		int index;
		if (closed)
		{
			index = (curveIndex + pointInCurve) % totalPoints;
		}
		else
		{
			index = curveIndex + pointInCurve;
			if (index >= totalPoints) index = totalPoints - 1;
		}

		BezierControlPoint cp = controlPoints[index];

		if (pointInCurve == 0)
		{
			return cp.point;
		}

		if (pointInCurve == 1)
		{
			return cp.point + cp.tangentOut;
		}

		if (pointInCurve == 2)
		{
			return cp.point + cp.tangentIn;
		}

		return cp.point;
	}

	public void UpdateLineRenderer()
	{
		if (lineRenderer == null)
		{
			return;
		}

		int totalPoints = curveResolution * CurveCount();
		if (totalPoints == 0)
		{
			lineRenderer.positionCount = 0;
			return;
		}

		Vector3[] positions = new Vector3[totalPoints + 1];
		float step = 1f / curveResolution;

		for (int i = 0; i <= totalPoints; i++)
		{
			float t = i * step;
			positions[i] = transform.InverseTransformPoint(GetPoint(t));
		}

		lineRenderer.positionCount = positions.Length;
		lineRenderer.SetPositions(positions);
	}

	public void SetClosed(bool value)
	{
		closed = value;
		UpdateLineRenderer();
	}

	public void Clear()
	{
		controlPoints.Clear();
		UpdateLineRenderer();
	}
}