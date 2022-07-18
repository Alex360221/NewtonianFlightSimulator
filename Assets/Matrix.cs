using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix
{
    public class Matrix4by4
    {
        public float[,] values;

        public Matrix4by4(Vector4 column1, Vector4 column2, Vector4 column3, Vector4 column4)
        {
            values = new float[4, 4];
            //column 1
            values[0, 0] = column1.x;
            values[1, 0] = column1.y;
            values[2, 0] = column1.z;
            values[3, 0] = column1.w;

            //column 2
            values[0, 1] = column2.x;
            values[1, 1] = column2.y;
            values[2, 1] = column2.z;
            values[3, 1] = column2.w;

            //column 3
            values[0, 2] = column3.x;
            values[1, 2] = column3.y;
            values[2, 2] = column3.z;
            values[3, 2] = column3.w;

            //column 4
            values[0, 3] = column4.x;
            values[1, 3] = column4.y;
            values[2, 3] = column4.z;
            values[3, 3] = column4.w;

        }

        public Matrix4by4(Vector3 column1, Vector3 column2, Vector3 column3, Vector3 column4)
        {
            values = new float[4, 4];
            //column 1
            values[0, 0] = column1.x;
            values[1, 0] = column1.y;
            values[2, 0] = column1.z;
            values[3, 0] = 0;

            //column 2
            values[0, 1] = column2.x;
            values[1, 1] = column2.y;
            values[2, 1] = column2.z;
            values[3, 1] = 0;

            //column 3
            values[0, 2] = column3.x;
            values[1, 2] = column3.y;
            values[2, 2] = column3.z;
            values[3, 2] = 0;

            //column 4
            values[0, 3] = column4.x;
            values[1, 3] = column4.y;
            values[2, 3] = column4.z;
            values[3, 3] = 1;
        }
        public static Vector4 operator *(Matrix4by4 lhs, Vector4 vector)
        {
            Vector4 rv;
            rv.x = (lhs.values[0, 0] * vector.x) + (lhs.values[0, 1] * vector.y) + (lhs.values[0, 2] * vector.z) + (lhs.values[0, 3] * vector.w);
            rv.y = (lhs.values[1, 0] * vector.x) + (lhs.values[1, 1] * vector.y) + (lhs.values[1, 2] * vector.z) + (lhs.values[1, 3] * vector.w);
            rv.z = (lhs.values[2, 0] * vector.x) + (lhs.values[2, 1] * vector.y) + (lhs.values[2, 2] * vector.z) + (lhs.values[2, 3] * vector.w);
            rv.w = (lhs.values[3, 0] * vector.x) + (lhs.values[3, 1] * vector.y) + (lhs.values[3, 2] * vector.z) + (lhs.values[3, 3] * vector.w);
            return rv;
        }
        public static Matrix4by4 Identity
        {
            get
            {
                return new Matrix4by4(
                    new Vector4(1, 0, 0, 0),
                    new Vector4(0, 1, 0, 0),
                    new Vector4(0, 0, 1, 0),
                    new Vector4(0, 0, 0, 1));
            }
        }



        public static Matrix4by4 operator *(Matrix4by4 lhs, Matrix4by4 rhs)
        {
            Matrix4by4 rm = new Matrix4by4(new Vector4(), new Vector4(), new Vector4(), new Vector4());
            rm.values = new float[4, 4];

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    rm.values[r, c] = (lhs.values[r, 0] * rhs.values[0, c]) + (lhs.values[r, 1] * rhs.values[1, c]) + (lhs.values[r, 2] * rhs.values[2, c]) + (lhs.values[r, 3] * rhs.values[3, c]);
                }
            }

            return rm;
        }
        public static Vector3 GetAxis(Matrix4by4 t)
        {
            Vector3 rv;
            rv.x = t.values[0, 0];
            rv.y = t.values[0, 1];
            rv.z = t.values[0, 2];

            return rv;
        }

        public Matrix4by4 TranslationInverse()
        {
            Matrix4by4 rm = Identity;

            rm.values[0, 3] = -values[0, 3];
            rm.values[1, 3] = -values[1, 3];
            rm.values[2, 3] = -values[2, 3];
            return rm;
        }

        //public Matrix4by4 QuatToMatrix(Quat q)
        //{
        //    //http://www.opengl-tutorial.org/assets/faq_quaternions/index.html#Q54
        //    Matrix4by4 rm = Identity;
        //    rm.values[0, 0] = 1 - 2 * ((q.y * q.y) + (q.z * q.z));
        //    rm.values[0, 1] = 2 * ((q.x * q.y) - (q.z * q.w));
        //    rm.values[0, 2] = 2 * ((q.x * q.z) + (q.y * q.w));

        //    rm.values[1, 0] = 2 * ((q.x * q.y) + (q.z * q.w));
        //    rm.values[1, 1] = 1 - 2 * ((q.x * q.x) + (q.z * q.z));
        //    rm.values[1, 2] = 2 * ((q.y * q.z) + (q.x * q.w));

        //    rm.values[2, 0] = 2 * ((q.x * q.z) - (q.y * q.w));
        //    rm.values[2, 1] = 2 * ((q.y * q.z) + (q.x * q.w));
        //    rm.values[2, 2] = 1 - 2 * ((q.x * q.x) + (q.y * q.y));
        //    return rm;
        //}
        public Matrix4by4 QuatToMatrix(Quat Q)
        {
            Q.Normalize();
            Matrix4by4 mat;
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
            mat = new Matrix4by4(new Vector4(vals[0], vals[1], vals[2], vals[3]),
                                 new Vector4(vals[4], vals[5], vals[6], vals[7]),
                                 new Vector4(vals[8], vals[9], vals[10], vals[11]),
                                 new Vector4(vals[12], vals[13], vals[14], vals[15]));
            return mat;
        }

        public Vector3 RotationMatrixToEulerAngle()
        {
            Vector3 rv;
            rv.y = Mathf.Asin(values[0,3]);        /* Calculate Y-axis angle */
            float C = Mathf.Cos(rv.y);
            rv.y *= (Mathf.PI/180);

            if (Mathf.Abs(C) > 0.005)             /* Gimball lock? */
            {
                float tx = values[2,2] / C;           /* No, so get X-axis angle */
                float ty = -values[2, 1] / C;

                rv.x = Mathf.Atan2( ty, tx ) *(Mathf.PI / 180);

                tx = values[0,0] / C;            /* Get Z-axis angle */
                ty = -values[0, 1] / C;

                rv.z = Mathf.Atan2( ty, tx ) *(Mathf.PI / 180);
             }
            else                                 /* Gimball lock has occurred */
            {
                rv.x = 0;                      /* Set X-axis angle to zero */

                float tx = values[1,1];                 /* And calculate Z-axis angle */
                float ty = values[1, 0];

                rv.z = Mathf.Atan2( ty, tx ) *(Mathf.PI / 180);
            }

            /* return only positive angles in [0,360] */
            if (rv.x < 0) rv.x += 360;
                            if (rv.y < 0) rv.y += 360;
                            if (rv.z < 0) rv.z += 360;
            
            return rv;
        }
       
    }
    
}
