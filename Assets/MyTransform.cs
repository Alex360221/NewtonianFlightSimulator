using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTransform : MonoBehaviour
{
    public Vector3 location;// = new Vector3(0,0,0);
    public Vector3 localLocation;// = new Vector3(0,0,0);
    public Vector3 rotation;// = new Vector3(1, 1, 1);
    public Vector3 scale;// = new Vector3(1, 1, 1);

    public Matrix.Matrix4by4 TRSMatrix;
    Matrix.Matrix4by4 translationMatrix;
    Matrix.Matrix4by4 scaleMatrix;
    public Matrix.Matrix4by4 rotationMatrix;
    Matrix.Matrix4by4 rollMatrix;
    Matrix.Matrix4by4 pitchMatrix;
    Matrix.Matrix4by4 yawMatrix;

    public GameObject owner;
    public GameObject parent;

    Vector3[] vertices;
    public Matrix.Matrix4by4 localTRSMatrix;
    Matrix.Matrix4by4 localtranslationMatrix;
    Matrix.Matrix4by4 localscaleMatrix;
    public Matrix.Matrix4by4 localrotationMatrix;
    Matrix.Matrix4by4 localrollMatrix;
    Matrix.Matrix4by4 localpitchMatrix;
    Matrix.Matrix4by4 localyawMatrix;

    public Quat orientation;

    public Vector4 orientationV4;

    public Vector3[] UpdateLocation(Vector3[] modelVertices)
    {
        orientationV4.w = orientation.w;
        orientationV4.x = orientation.x;
        orientationV4.y = orientation.y;
        orientationV4.z = orientation.z;
       
        if (parent == null)
        {
            //update rotation TRS martix to include the new rotation
           
            Matrix.Matrix4by4 newrotationMatrix = orientation.QuatToMatrix(orientation);
            rotationMatrix = newrotationMatrix;

        }

        //Update all our matrices
        //update  the locations of vertices
        Vector3[] rv = new Vector3[modelVertices.Length];
        Vector4[] nrv = new Vector4[modelVertices.Length];
        
        for (int i = 0; i < rv.Length; i++)
        {
            //work out the new vertex positions
            nrv[i].x = modelVertices[i].x;
            nrv[i].y = modelVertices[i].y;
            nrv[i].z = modelVertices[i].z;
            nrv[i].w = 1;
            if (parent != null)
            {
                //this updates the rotation and all of the child stuff
                rv[i] = parent.GetComponent<Ship>().shipTransform.TRSMatrix * localTRSMatrix * nrv[i];
            }
            else
            {
                //update vertices
                rv[i] = TRSMatrix * nrv[i];               
            }

        }
        //return new vertex positions
        return rv;
    }
    public void Location()
    {

        //updates the vertices
        owner.GetComponent<MeshFilter>().mesh.vertices = UpdateLocation(vertices);
        //update the bounds and normals
        owner.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        owner.GetComponent<MeshFilter>().mesh.RecalculateBounds();
      
        

        if (parent != null)
        {
            //update location of the thruster
            location = parent.GetComponent<Ship>().shipTransform.location + localLocation;
        }
    }

    void Start()
    {
        //set up translation matrix
        orientation = new Quat();
        //golbal space
        translationMatrix = new Matrix.Matrix4by4(
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            location);
        //set up scale matrix
        scaleMatrix = new Matrix.Matrix4by4(
            new Vector3(scale.x, 0, 0),
            new Vector3(0, scale.y, 0),
            new Vector3(0, 0, scale.z),
            Vector3.zero);
        //set up the rotation matrix
        rollMatrix = new Matrix.Matrix4by4(
             new Vector3(Mathf.Cos(rotation.z), Mathf.Sin(rotation.z), 0),
            new Vector3(-Mathf.Sin(rotation.z), Mathf.Cos(rotation.z), 0),
            new Vector3(0, 0, 1),
            Vector3.zero);
        pitchMatrix = new Matrix.Matrix4by4(
             new Vector3(1, 0, 0),
            new Vector3(0, Mathf.Cos(rotation.x), Mathf.Sin(rotation.x)),
            new Vector3(0, -Mathf.Sin(rotation.x), Mathf.Cos(rotation.x)),
            Vector3.zero);
        yawMatrix = new Matrix.Matrix4by4(
            new Vector3(Mathf.Cos(rotation.y), 0, -Mathf.Sin(rotation.y)),
            new Vector3(0, 1, 0),
            new Vector3(Mathf.Sin(rotation.y), 0, Mathf.Cos(rotation.y)),
            Vector3.zero);
        //times from the right
        rotationMatrix = yawMatrix * (pitchMatrix * rollMatrix);
        //TRS Matrix
        TRSMatrix = translationMatrix * (rotationMatrix * scaleMatrix);

        if (parent != null)
        {
            //update local matrix
            localtranslationMatrix = new Matrix.Matrix4by4(
           new Vector3(1, 0, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 0, 1),
           localLocation);
            //set up scale matrix
            localscaleMatrix = new Matrix.Matrix4by4(
                new Vector3(scale.x, 0, 0),
                new Vector3(0, scale.y, 0),
                new Vector3(0, 0, scale.z),
                Vector3.zero);
            //set up the rotation matrix
            localrollMatrix = new Matrix.Matrix4by4(
                 new Vector3(Mathf.Cos(rotation.z), Mathf.Sin(rotation.z), 0),
                new Vector3(-Mathf.Sin(rotation.z), Mathf.Cos(rotation.z), 0),
                new Vector3(0, 0, 1),
                Vector3.zero);
            localpitchMatrix = new Matrix.Matrix4by4(
                 new Vector3(1, 0, 0),
                new Vector3(0, Mathf.Cos(rotation.x), Mathf.Sin(rotation.x)),
                new Vector3(0, -Mathf.Sin(rotation.x), Mathf.Cos(rotation.x)),
                Vector3.zero);
            localyawMatrix = new Matrix.Matrix4by4(
                new Vector3(Mathf.Cos(rotation.y), 0, -Mathf.Sin(rotation.y)),
                new Vector3(0, 1, 0),
                new Vector3(Mathf.Sin(rotation.y), 0, Mathf.Cos(rotation.y)),
                Vector3.zero);
            //times from the right
            localrotationMatrix = localyawMatrix * (localpitchMatrix * localrollMatrix);
            //TRS Matrix
            localTRSMatrix = localtranslationMatrix * (localrotationMatrix * localscaleMatrix);
        }

        //gets the mesh filter
        MeshFilter MF = owner.GetComponent<MeshFilter>();
        //gets the vertices
        vertices = MF.mesh.vertices;
    }
    void FixedUpdate()
    {

         //update my location if i have a parent
        if (parent!= null)
        {
            //update location varibles
            localLocation = owner.GetComponent<Thruster>().thrusterLocationRelative;
            

            //update local trs matrix
            //update local matrix
            localtranslationMatrix = new Matrix.Matrix4by4(
           new Vector3(1, 0, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 0, 1),
           localLocation);
            //set up scale matrix
            localscaleMatrix = new Matrix.Matrix4by4(
                new Vector3(scale.x, 0, 0),
                new Vector3(0, scale.y, 0),
                new Vector3(0, 0, scale.z),
                Vector3.zero);

            ////times from the right
            localrotationMatrix = rotationMatrix;
            //TRS Matrix
            localTRSMatrix = localtranslationMatrix * (localrotationMatrix * localscaleMatrix); 
        }

        //set up translation matrix
        translationMatrix = new Matrix.Matrix4by4(
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            location);
        //set up scale matrix
        scaleMatrix = new Matrix.Matrix4by4(
            new Vector3(scale.x, 0, 0),
            new Vector3(0, scale.y, 0),
            new Vector3(0, 0, scale.z),
            Vector3.zero);

        TRSMatrix = translationMatrix * (rotationMatrix * scaleMatrix);

        Location();
    }
}
