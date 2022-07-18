using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public GameObject[] thrusters;
    public int thrusterAmount = 12;

    public GameObject[] gravityObjects;
    public int gravityObjectAmount = 12;

    public Vector3 force = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public float mass = 1;

    //Angular motion
    public Vector3 torque = Vector3.zero;
    public Vector3 angularAcceleration = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;
    public float inertia = 20;


    public MyTransform shipTransform;


    public Vector3 planetGravitationalForce;
    public Vector3 ForceWithoutGravity;
    public Quat qRotation = new Quat();


    // Start is called before the first frame update
    void Start()
    {
        //intilize amount of thrusters in the array
        thrusters = new GameObject[thrusterAmount];
        //adds all the thrusters to an array
        AddThrusterToShip();
        gravityObjects = new GameObject[gravityObjectAmount];
        AddGravityObjects();
    }

    //runs at fixed rate for simulating physics
    void FixedUpdate()
    {
       // planetGravitationalForce = Vector3.zero;
        //------------update the ship velocity
        force = Vector3.zero;
        Vector3 newTorque = Vector3.zero;
        for (int i = 0; i < thrusterAmount; i++)    //loop through all the thrusters
        {
            if (thrusters[i] != null)   //checks thruster is vaild
            {
                force += thrusters[i].GetComponent<Thruster>().Force; //gets the force of all the thrusters
                //rotate the force to the current ships rotation   
                newTorque += VectorArithmetic.VectorCrossProduct(thrusters[i].GetComponent<Thruster>().Force, shipTransform.location - thrusters[i].GetComponent<Thruster>().thrusterTransform.location); //gets the force of all the thrusters
            }
        }
        //rotate the force to the current ships rotation   
        force = shipTransform.rotationMatrix * force;
       
        Vector3 gravitationalForce = Vector3.zero;
        //add gravitational force on to force
        for (int i = 0; i < gravityObjectAmount; i++)
        {
            if (gravityObjects[i] != null)
            {
                gravitationalForce += gravityObjects[i].GetComponent<ObjectGravity>().gravityOnShip;
            }
        }
        planetGravitationalForce = gravitationalForce;
        //add the gravitional force onto the ship
        force -= gravitationalForce;

        //now work out the overal ship velocity
        acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;
        //need to update the velocity so it takes into account the roartion of the ship to move in that direction
        //testvelocity = angularVelocity+ velocity;

        torque += newTorque;
        //inertia for soild sphere = 2/5(m)(r)(r)
        inertia = (2.0f / 5.0f) * mass * 0.5f * 0.5f;
        //work out agular acceleration = torque / inertia
        angularAcceleration = torque / inertia;
        
        //agular velocity
        angularVelocity = angularAcceleration * Time.fixedDeltaTime;
        //convert angular velocity into quat
        float avMag = VectorArithmetic.Length(angularVelocity * Time.fixedDeltaTime);
        if (avMag != 0)
        {
            qRotation.w = Mathf.Cos(avMag / 2);
            qRotation.x = Mathf.Sin(avMag / 2) * (angularVelocity * Time.fixedDeltaTime).x / avMag;
            qRotation.y = Mathf.Sin(avMag / 2) * (angularVelocity * Time.fixedDeltaTime).y / avMag;
            qRotation.z = Mathf.Sin(avMag / 2) * (angularVelocity * Time.fixedDeltaTime).z / avMag;
        }
        //Update transform orientation
        shipTransform.orientation = qRotation * shipTransform.orientation;

        //now update the ship postion
        shipTransform.location += velocity * Time.fixedDeltaTime;

    }
    

    public void AddThrusterToShip()
    {
        //loops through the array
        for (int i = 0; i < thrusterAmount; i++)
        {
            //checks the slot is empty
            if (thrusters[i] == null)
            {
                //thruster name + i (i is the number of the thruster)
                thrusters[i] = GameObject.Find("Thruster"+i);
                if (thrusters[i] != null)
                {
                    //add mass up
                    mass += thrusters[i].GetComponent<Thruster>().Mass;
                }            
            }
        }
    }

    public void AddGravityObjects()
    {
        //loops through the array
        for (int i = 0; i < gravityObjectAmount; i++)
        {
            //checks the slot is empty
            if (gravityObjects[i] == null)
            {
                //thruster name + i (i is the number of the thruster)
                gravityObjects[i] = GameObject.Find("GravityObject" + i);              
            }
        }
    }
}
