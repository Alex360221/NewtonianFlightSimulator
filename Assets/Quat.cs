using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quat
{
    public float w, x, y, z;


    public Quat()
    {
        w = 1;
    }
    public Quat(Vector3 Axis)
    {
        w = 0;
        x = Axis.x;
        y = Axis.y;
        z = Axis.z;      
    }
    public Quat(Vector4 Axis)
    {
        w = Axis.w;
        x = Axis.x;
        y = Axis.y;
        z = Axis.z;
    }
    public Quat(float Angle, Vector3 Axis)
    {
        float halfAngle = Angle / 2;
        w = Mathf.Cos(halfAngle);
        x = Axis.x * Mathf.Sin(halfAngle);
        y = Axis.y * Mathf.Sin(halfAngle);
        z = Axis.z * Mathf.Sin(halfAngle);
    }
    
    public static Quat operator*(Quat S, Quat R)
    {
        Quat rs = new Quat();
        rs.w = S.w * R.w - VectorArithmetic.DotProduct(S.GetAxis(),R.GetAxis(),false);
        rs.x = (S.w * R.x) + (R.w * S.x) + VectorArithmetic.VectorCrossProduct(S.GetAxis(), R.GetAxis()).x;
        rs.y = (S.w * R.y) + (R.w * S.y) + VectorArithmetic.VectorCrossProduct(S.GetAxis(), R.GetAxis()).y;
        rs.z = (S.w * R.z) + (R.w * S.z) + VectorArithmetic.VectorCrossProduct(S.GetAxis(), R.GetAxis()).z;
        return rs;
    }
    public static Vector3 operator*(Quat q, Vector3 v)
    {
        Vector3 rv = Vector3.zero;
        //Extract the vector part of the quaternion
        Vector3 z;
        z = q.GetAxis();

        //Extract the scalar part of the quaternion
        float s = q.w;
        if (z.x!=0 && z.y!=0 && z.z!=0 && s !=0)
        {
            //do the maths 
            rv = 2 * VectorArithmetic.DotProduct(z, v) * z
                + (s * s - VectorArithmetic.DotProduct(z, z)) * v
                + 2 * s * VectorArithmetic.VectorCrossProduct(z, v);
            return rv;
        }
        
        return v;
    }
    public static Vector3 operator *(Vector3 v, Quat q)
    {
        Vector3 rv = Vector3.zero;
        //Extract the vector part of the quaternion
        Vector3 z;
        z = q.GetAxis();

        //Extract the scalar part of the quaternion
        float s = q.w;
        if (z.x != 0 && z.y != 0 && z.z != 0 && s != 0)
        {
            //do the maths 
            rv = 2 * VectorArithmetic.DotProduct(z, v) * z
                + (s * s - VectorArithmetic.DotProduct(z, z)) * v
                + 2 * s * VectorArithmetic.VectorCrossProduct(z, v);
            return rv;
        }

        return v;
    }
    public Quat Inverse() 
    {
        Quat rv = new Quat();
        rv.w = w;
        rv.SetAxis(-GetAxis());
        return rv;
    }
    public float magnitude()
    {
        return (Mathf.Sqrt(w * w) + (x * x) + (y * y) + (z * z));
    }
    public Quat Normalize()
    {
        Quat ret = new Quat();
        ret.w = w / magnitude();
        ret.x = x / magnitude();
        ret.y = y / magnitude();
        ret.z = z / magnitude();
        return ret;
    }

    public void SetAxis(Vector3 Axis)
    {
        x = Axis.x;
        y = Axis.y;
        z = Axis.z;
    }
    public Vector3 GetAxis()
    {
        Vector3 rv;
        rv.x = x;
        rv.y = y;
        rv.z = z;
        return rv;
    }
    public Quat EulerAngleToQuat(Vector3 EulerAngle)
    {
        float cy = Mathf.Cos(EulerAngle.y * 0.5f);
        float sy = Mathf.Sin(EulerAngle.y * 0.5f);
        float cp = Mathf.Cos(EulerAngle.x * 0.5f);
        float sp = Mathf.Sin(EulerAngle.x * 0.5f);
        float cr = Mathf.Cos(EulerAngle.z * 0.5f);
        float sr = Mathf.Sin(EulerAngle.z * 0.5f);

        Quat q = new Quat();

        q.w = cy * cp * cr + sy * sp * sr;
        q.z = cy * cp * sr - sy * sp * cr;
        q.x = sy * cp * sr + cy * sp * cr;
        q.y = sy * cp * cr - cy * sp * sr;

        return q;
    }
    public Vector3 QuatToEulerAngle(Quat q)
    {
        Vector3 rv;

        float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
        float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
        rv.x = Mathf.Atan2(sinr_cosp, cosr_cosp);

        // pitch (y-axis rotation)
        float sinp = 2 * (q.w * q.y - q.z * q.x);
        if (Mathf.Abs(sinp) >= 1)
            rv.y = Mathf.PI / 2* sinp; // use 90 degrees if out of range
        else
            rv.y = Mathf.Asin(sinp);

        // yaw (z-axis rotation)
        float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
        float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
        rv.z = Mathf.Atan2(siny_cosp, cosy_cosp);

        return rv;
    }
    public Vector4 GetAxisAngle()
    {
        Vector4 rv = new Vector4();

        //Inverse cosine to get our half angle back
        float halfAngle = Mathf.Acos(w);
        rv.w = halfAngle * 2; //this is our full angle

        //simple calculations to get our normal axis back using the galf-angle
        rv.x = x / Mathf.Sin(halfAngle);
        rv.y = y / Mathf.Sin(halfAngle);
        rv.z = z / Mathf.Sin(halfAngle);
        return rv;
    }
    public Matrix.Matrix4by4 QuatToMatrix(Quat Q)
    {
        Q.Normalize();
        Matrix.Matrix4by4 mat;
        float[] vals = new float[16];
        float xx, xy, xz, xw, yy, yz, yw, zz, zw;
        xx = Q.x * Q.x;
        xy = Q.x * Q.y;
        xz = Q.x * Q.z;
        xw = Q.x * Q.w;

        yy = Q.y * Q.y;
        yz = Q.y * Q.z;
        yw = Q.y * Q.w;

        zz = Q.z * Q.z;
        zw = Q.z * Q.w;

        vals[0] = 1 - 2 * (yy + zz);
        vals[1] = 2 * (xy - zw);
        vals[2] = 2 * (xz + yw);

        vals[4] = 2 * (xy + zw);
        vals[5] = 1 - 2 * (xx + zz);
        vals[6] = 2 * (yz - xw);

        vals[8] = 2 * (xz - yw);
        vals[9] = 2 * (yz + xw);
        vals[10] = 1 - 2 * (xx + yy);

        vals[3] = vals[7] = vals[11] = vals[12] = vals[13] = vals[14] = 0;
        vals[15] = 1;
        mat = new Matrix.Matrix4by4(new Vector4(vals[0], vals[1], vals[2], vals[3]),
                             new Vector4(vals[4], vals[5], vals[6], vals[7]),
                             new Vector4(vals[8], vals[9], vals[10], vals[11]),
                             new Vector4(vals[12], vals[13], vals[14], vals[15]));
        return mat;
    }

    public Quaternion QuatToUnity(Quat A)
    {
        Quaternion rq;

        rq.w = A.w;
        rq.x = A.x;
        rq.y = A.y;
        rq.z = A.z;

        return rq;
    }
    public Quat UnityToQuat(Quaternion A)
    {
        Quat rq =  new Quat();

        rq.w = A.w;
        rq.x = A.x;
        rq.y = A.y;
        rq.z = A.z;

        return rq;
    }


}


