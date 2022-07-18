using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerAngle
{
    public static float VectorToRadians(Vector2 V)
    {
        float rv = 0.0f;
        rv = Mathf.Atan(V.y / V.x);
        return rv;
    }
    public static Vector2 RadiansToVector(float angle)
    {
        Vector2 rv = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return rv;
    }
    public static Vector3 EulerAnglesToDirection(Vector3 EulerAngles)
    {
        Vector3 rv = new Vector3();

        //Makes sure the values stored inside "Euler Angles" are in RADIANS
        //depeding on the cooedinates systems some of these vaules may need change
        //currently set up for right hand system
        rv.z = Mathf.Cos(EulerAngles.y) * Mathf.Cos(EulerAngles.x);
        rv.y = Mathf.Sin(-EulerAngles.x);
        rv.x = Mathf.Cos(EulerAngles.x) * Mathf.Sin(EulerAngles.y);

        return rv;
    }
    public static Vector3 DegreeToRadians(Vector3 v)
    {
        Vector3 rv = v;
        rv = rv * Mathf.PI / 180.0f;
        return rv;
    }
}
