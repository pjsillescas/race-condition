using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SplinePoint
{
	public Vector3 position;

	public SplinePoint(Vector3 pos)
	{
		position = pos;
	}
}

[RequireComponent(typeof(LineRenderer))]
public class SplineTrack : MonoBehaviour
{
	[SerializeField]
	private List<SplinePoint> points = new();

	[SerializeField]
	private bool closed;

	[SerializeField]
	private int resolution = 20;

	private LineRenderer lineRenderer;

	public List<SplinePoint> Points => points;
	public bool Closed => closed;
	public int Resolution => resolution;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = 0;
		lineRenderer.useWorldSpace = false;
	}

	public int SegmentCount => closed ? points.Count : Mathf.Max(0, points.Count - 1);

	public void AddPoint(Vector3 position)
	{
		points.Add(new SplinePoint(position));
		UpdateLineRenderer();
	}

	public void RemovePoint(int index)
	{
		if (0 <= index && index < points.Count)
		{
			points.RemoveAt(index);
			UpdateLineRenderer();
		}
	}

	public Vector3 GetPoint(float t)
	{
		if (points.Count < 2)
		{
			return transform.position;
		}

		t = Mathf.Clamp01(t);
		int segmentCount = SegmentCount;
		if (segmentCount == 0) return transform.position;

		float scaledT = t * segmentCount;
		int segmentIndex = Mathf.FloorToInt(scaledT);
		float localT = scaledT - segmentIndex;

		segmentIndex = Mathf.Clamp(segmentIndex, 0, segmentCount - 1);

		return GetCatmullRomPoint(segmentIndex, localT);
	}

	public Vector3 GetTangent(float t)
	{
		if (points.Count < 2)
		{
			return transform.forward;
		}

		t = Mathf.Clamp01(t);
		int segmentCount = SegmentCount;
		if (segmentCount == 0)
		{
			return transform.forward;
		}

		float scaledT = t * segmentCount;
		int segmentIndex = Mathf.FloorToInt(scaledT);
		float localT = scaledT - segmentIndex;

		segmentIndex = Mathf.Clamp(segmentIndex, 0, segmentCount - 1);

		float delta = 0.001f;
		Vector3 p1 = GetCatmullRomPoint(segmentIndex, Mathf.Clamp01(localT - delta));
		Vector3 p2 = GetCatmullRomPoint(segmentIndex, Mathf.Clamp01(localT + delta));
		return (p2 - p1).normalized;
	}

	private Vector3 GetCatmullRomPoint(int segmentIndex, float t)
	{
		int p0, p1, p2, p3;

		if (closed)
		{
			p0 = (segmentIndex - 1 + points.Count) % points.Count;
			p1 = segmentIndex;
			p2 = (segmentIndex + 1) % points.Count;
			p3 = (segmentIndex + 2) % points.Count;
		}
		else
		{
			p0 = Mathf.Max(0, segmentIndex - 1);
			p1 = segmentIndex;
			p2 = Mathf.Min(points.Count - 1, segmentIndex + 1);
			p3 = Mathf.Min(points.Count - 1, segmentIndex + 2);
		}

		Vector3 v0 = points[p0].position;
		Vector3 v1 = points[p1].position;
		Vector3 v2 = points[p2].position;
		Vector3 v3 = points[p3].position;

		return CatmullRom(v0, v1, v2, v3, t);
	}

	private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;

		return 0.5f * (
			(2f * p1) +
			(-p0 + p2) * t +
			(2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
			(-p0 + 3f * p1 - 3f * p2 + p3) * t3
		);
	}

	public float GetTotalLength()
	{
		float length = 0f;
		int baseSteps = resolution * SegmentCount;
		if (baseSteps == 0)
		{
			return 0f;
		}

		int steps = closed ? baseSteps + 1 : baseSteps;

		Vector3 lastPoint = GetPoint(0f);
		for (int i = 1; i <= steps; i++)
		{
			float t = (float)i / steps;
			Vector3 currentPoint = GetPoint(t);
			length += Vector3.Distance(lastPoint, currentPoint);
			lastPoint = currentPoint;
		}

		return length;
	}

	public void UpdateLineRenderer()
	{
		if (lineRenderer == null)
		{
			return;
		}

		int totalPoints = resolution * SegmentCount;
		if (totalPoints == 0)
		{
			lineRenderer.positionCount = 0;
			return;
		}

		Vector3[] positions = new Vector3[totalPoints + 1];
		float step = 1f / resolution;

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
		points.Clear();
		UpdateLineRenderer();
	}

	public void GetClosestPointIndexTo(Vector3 targetPosition, out float index, out float bestDistance)
	{
		/*
		int segmentCount = SegmentCount;
		if (segmentCount == 0)
		{
			index = -1f;
			bestDistance = 0f;
			return;
		}

		float step = 1f / resolution;
		float bestT = 0f;
		bestDistance = float.MaxValue;

		for (int i = 0; i <= resolution * segmentCount; i++)
		{
			float t = i * step;
			Vector3 point = GetPoint(t);
			float distance = Vector3.Distance(point, targetPosition);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				bestT = t;
			}
		}

		bestT = GoldenSectionSearch(bestT, targetPosition);
		index = bestT;
		*/
		FindClosestPoint(targetPosition, out float tFinal, out Vector3 position);
		index = tFinal;
		bestDistance = (targetPosition - position).magnitude;
	}

	public Vector3 GetClosestPointTo(Vector3 targetPosition)
	{
		//GetClosestPointIndexTo(targetPosition, out float bestT, out _);
		//return (bestT < 0) ? transform.position : GetPoint(bestT);
		FindClosestPoint(targetPosition, out float tFinal, out Vector3 position);

		return (tFinal < 0) ? transform.position : GetPoint(tFinal);
	}

	private float GoldenSectionSearch(float initialT, Vector3 targetPosition)
	{
		const float EPSILON = 0.0001f;

		float a = Mathf.Max(0f, initialT - 0.1f);
		float b = Mathf.Min(1f, initialT + 0.1f);
		float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;

		float c = b - (b - a) / goldenRatio;
		float d = a + (b - a) / goldenRatio;

		for (int i = 0; i < 20; i++)
		{
			if (b - a < EPSILON)
			{
				return (a + b) / 2f;
			}

			if (DistanceTo(targetPosition, c) < DistanceTo(targetPosition, d))
			{
				b = d;
			}
			else
			{
				a = c;
			}

			var delta = (b - a) / goldenRatio;
			c = b - delta;
			d = a + delta;
		}

		return (a + b) / 2f;
	}

	public void FindClosestPoint(Vector3 target, out float tFinal, out Vector3 position)
	{
		int samples = 128;
		int refinementIterations = 16;

		//----------------------------------------
		// STEP 1:
		// Coarse search by sampling the spline
		//----------------------------------------

		float tMax = GetTotalLength();
		float bestT = 0.0f;
		float bestDistSq = float.MaxValue;

		for (int i = 0; i <= samples; i++)
		{
			float t = tMax * i / samples;

			Vector3 p = GetPoint(t);

			float distSq = (p - target).sqrMagnitude;

			if (distSq < bestDistSq)
			{
				bestDistSq = distSq;
				bestT = t;
			}
		}

		//----------------------------------------
		// STEP 2:
		// Local refinement around best sample
		//
		// Uses shrinking interval search
		// (simple + robust for splines)
		//----------------------------------------

		float range = tMax / samples;

		for (int iter = 0; iter < refinementIterations; iter++)
		{
			float leftT = Mathf.Max(0.0f, bestT - range);
			float rightT = Mathf.Min(tMax, bestT + range);

			float midLeft = (leftT + bestT) * 0.5f;
			float midRight = (bestT + rightT) * 0.5f;

			float leftDist =
				(GetPoint(midLeft) - target).sqrMagnitude;

			float rightDist =
				(GetPoint(midRight) - target).sqrMagnitude;

			if (leftDist < bestDistSq)
			{
				bestDistSq = leftDist;
				bestT = midLeft;
			}

			if (rightDist < bestDistSq)
			{
				bestDistSq = rightDist;
				bestT = midRight;
			}

			// shrink search region
			range *= 0.5f;
		}

		tFinal = bestT;
		position = GetPoint(bestT);
	}

	private float DistanceTo(Vector3 targetPosition, float t)
	{
		//return Vector3.Distance(GetPoint(t), targetPosition);
		return (targetPosition - GetPoint(t)).sqrMagnitude;
	}
}