using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
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

    public void UpdateLocation(Vector3 modelVertice)
    {
        orientationV4.w = orientation.w;
        orientationV4.x = orientation.x;
        orientationV4.y = orientation.y;
        orientationV4.z = orientation.z;

        //Quat rotationQuat = new Quat();

        if (parent == null)
        {
            //update rotation TRS martix to include the new rotation
            Matrix.Matrix4by4 newrotationMatrix = orientation.QuatToMatrix(orientation);

            rotationMatrix = newrotationMatrix;

        }

        //Update all our matrices
        //update  the locations of vertices
        Vector3 rv = modelVertice;
        Vector4 nrv = modelVertice;
        
        //work out the new vertex positions
        nrv.x = modelVertice.x;
        nrv.y = modelVertice.y;
        nrv.z = modelVertice.z;
        nrv.w = 1;
        if (parent != null)
        {
            //this updates the rotation and all of the child stuff
            rv = parent.GetComponent<Ship>().shipTransform.TRSMatrix * localTRSMatrix * nrv;
             
            //update all the varaiables to do with camera
            location = rv;
            owner.GetComponent<Camera>().transform.position = rv;
            owner.GetComponent<Camera>().transform.LookAt(parent.GetComponent<Ship>().shipTransform.location, Vector3.up);
        }     

    }
    public void Location()
    {

        //updates the camera location
        UpdateLocation(owner.transform.position);

    }

    void Start()
    {
        //set up translation matrix
        orientation = new Quat();
        orientation = orientation.UnityToQuat(owner.GetComponent<Camera>().transform.rotation);
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
    
    }
    void FixedUpdate()
    {

        //update my location if i have a parent
        if (parent != null)
        {
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

            localrotationMatrix = rotationMatrix;//localyawMatrix * (localpitchMatrix * localrollMatrix);
            //TRS Matrix
            localTRSMatrix = localtranslationMatrix * (localrotationMatrix * localscaleMatrix);
        }

        //UPDATE ALL OUR MATRICES
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
