using UnityEngine;

public static class BezierMath
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }

    public static Vector3 GetSecondDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        return
            6f * (1f - t) * (p2 - 2f * p1 + p0) +
            6f * t * (p3 - 2f * p2 + p1);
    }

    public static float GetCurvature(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 first = GetFirstDerivative(p0, p1, p2, p3, t);
        Vector3 second = GetSecondDerivative(p0, p1, p2, p3, t);
        float firstMag = first.sqrMagnitude;
        if (firstMag < Mathf.Epsilon) return 0f;
        return Vector3.Cross(first, second).magnitude / Mathf.Pow(firstMag, 1.5f);
    }

    public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return GetFirstDerivative(p0, p1, p2, p3, t).normalized;
    }
}