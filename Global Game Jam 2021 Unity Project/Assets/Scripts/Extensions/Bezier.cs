using UnityEngine;

public static class Bezier
{
    public static Vector3 BezierFunction(Vector3 startPosition, Vector3 startTangent, Vector3 endTangent, Vector3 endPosition, float x)
    {
        x = Mathf.Clamp01(x);

        Vector3 ap1 = Vector3.Lerp(startPosition, startTangent, x);
        Vector3 ap2 = Vector3.Lerp(startTangent, endTangent, x);
        Vector3 ap3 = Vector3.Lerp(endTangent, endPosition, x);

        Vector3 bp1 = Vector3.Lerp(ap1, ap2, x);
        Vector3 bp2 = Vector3.Lerp(ap2, ap3, x);

        return Vector3.Lerp(bp1, bp2, x);
    }

    public static Vector3 BezierDerivativeFunction(Vector3 startPosition, Vector3 startTangent, Vector3 endTangent, Vector3 endPosition, float x)
    {
        x = Mathf.Clamp01(x);

        Vector3 a = Vector3.Lerp(startPosition, startTangent, x);
        Vector3 b = Vector3.Lerp(startTangent, endTangent, x);
        Vector3 c = Vector3.Lerp(endTangent, endPosition, x);

        Vector3 d = Vector3.Lerp(a, b, x);
        Vector3 e = Vector3.Lerp(b, c, x);

        return (e - d).normalized;
    }
}
