using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorArithmetic
{
    public static Vector2 VectorAdd(Vector2 A, Vector2 B)
    {
        Vector2 AddedVector;
        AddedVector.x = A.x + B.x;
        AddedVector.y = A.y + B.y;
        return AddedVector;
    }
    public static Vector2 VectorSubtract(Vector2 A, Vector2 B)
    {
        Vector2 SubtractVector;
        SubtractVector.x = A.x - B.x;
        SubtractVector.y = A.y - B.y;
        return SubtractVector;
    }
    public static Vector3 VectorAdd(Vector3 A, Vector3 B)
    {
        Vector3 AddedVector;
        AddedVector.x = A.x + B.x;
        AddedVector.y = A.y + B.y;
        AddedVector.z = A.z + B.z;

        return AddedVector;
    }
    public static Vector3 VectorXVector(Vector3 A, Vector3 B)
    {
        Vector3 rv;
        rv.x = A.x * B.x;
        rv.y = A.y * B.y;
        rv.z = A.z * B.z;
        return rv;
    }
    public static Vector3 VectorSubtract(Vector3 A, Vector3 B)
    {
        Vector3 SubtractVector;
        SubtractVector.x = A.x - B.x;
        SubtractVector.y = A.y - B.y;
        SubtractVector.z = A.z - B.z;

        return SubtractVector;
    }
    public static Vector3 MultiplyVector(Vector3 A, float scalar)
    {
        Vector3 rv;
        rv.x = A.x * scalar;
        rv.y = A.y * scalar;
        rv.z = A.z * scalar;
        return rv;
    }
    public static Vector3 DivideVector(Vector3 A, float divisor)
    {
        Vector3 rv;
        rv.x = A.x / divisor;
        rv.y = A.y / divisor;
        rv.z = A.z / divisor;
        return rv;
    }

    public static Vector3 RoundVector(Vector3 A, int Rounder)
    {
        Vector3 rv;
        rv.x = (Mathf.Round(A.x * Rounder)) / Rounder;
        rv.y = (Mathf.Round(A.y * Rounder)) / Rounder;
        rv.z = (Mathf.Round(A.z * Rounder)) / Rounder;
        return rv;
    }
    public static Vector2 RoundVector(Vector2 A, int Rounder)
    {
        Vector2 rv;
        rv.x = (Mathf.Round(A.x * Rounder)) / Rounder;
        rv.y = (Mathf.Round(A.y * Rounder)) / Rounder;
        return rv;
    }

    public static float LengthSq(Vector3 A)
    {
        float rv;
        rv = A.x * A.x + A.y * A.y + A.z * A.z;
        return rv;
    }
    public static float Length(Vector3 A)
    {
        float rv;
        rv = Mathf.Sqrt(A.x * A.x + A.y * A.y + A.z * A.z);
        return rv;
    }
    public static Vector3 VectorNormalized(Vector3 A)
    {
        Vector3 rv = A;
        rv = rv / Length(rv);
        return rv;
    }
    public static Vector3 Sqrvector(Vector3 A)
    {
        Vector3 rv;
        rv.x = Mathf.Sqrt(A.x);
        rv.y = Mathf.Sqrt(A.y);
        rv.z = Mathf.Sqrt(A.z);
        return rv;
    }

    public static float DotProduct(Vector3 A, Vector3 B, bool shouldNormalize = true)
    {
        float rv;
        Vector3 newA = A;
        Vector3 newB = B;
        if (shouldNormalize)
        {
            newA = VectorNormalized(newA);
            newB = VectorNormalized(newB);
        }
        rv = newA.x * newB.x + newA.y * newB.y + newA.z * newB.z;
        return rv;
    }
    public static Vector3 VectorLerp(Vector3 A, Vector3 B, float t)
    {
        return A * (1.0f - t) + B * t;
    }
    public static Vector3 VectorCrossProduct(Vector3 A, Vector3 B)
    {
        Vector3 C = new Vector3();

        C.x = A.y * B.z - A.z * B.y;
        C.y = A.z * B.x - A.x * B.z;
        C.z = A.x * B.y - A.y * B.x;
        return C;
    }
    public static Vector3 RotateVertexAroundAxis(float Angle, Vector3 Axis,Vector3 Vertex)
    {
        //the rodrigues rotation formula
        //makes sure angle is in radians
        Vector3 rv = (Vertex * Mathf.Cos(Angle)) +
            DotProduct(Vertex, Axis) * Axis * (1 - Mathf.Cos(Angle)) +
            VectorCrossProduct(Axis, Vertex) * Mathf.Sin(Angle);
        return rv;
    }
}
