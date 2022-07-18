using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGravity : MonoBehaviour
{
    public float mass;
    public float radius;
    public Vector3 gravityOnShip;
    public Vector3 gravityActingOnObject;
    public Vector3 velocity;
    public Vector3 initialVelocity;
  

    public GameObject Ship;
    public GameObject PlannetOwner;
    public MyTransform objectTransform;
    // Start is called before the first frame update
    void Start()
    {
        //objectTransform.Location();
        if (PlannetOwner != null)
        {
            velocity = initialVelocity;
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Ship != null)
        {
            //work out the gravity of the object
            //Gravity = GravityConstant * ((m1 * m2)/sq(distance))
            float GC = 0.006f;
            float sqrDst = VectorArithmetic.Length(Ship.GetComponent<Ship>().shipTransform.location - objectTransform.location);
            Vector3 forceDirection = VectorArithmetic.VectorNormalized(Ship.GetComponent<Ship>().shipTransform.location - objectTransform.location);

            gravityOnShip = forceDirection * GC * mass * Ship.GetComponent<Ship>().mass / sqrDst;

        }   

        if (PlannetOwner != null)
        {
            //plannet should be orbiting around another plannet
            float GC = 0.006f;
            float sqrDst = VectorArithmetic.Length(PlannetOwner.GetComponent<ObjectGravity>().objectTransform.location - objectTransform.location);
            Vector3 forceDirection = VectorArithmetic.VectorNormalized(PlannetOwner.GetComponent<ObjectGravity>().objectTransform.location - objectTransform.location);

            //update this object location
            Vector3 force = new Vector3(0, 0, 0); ;
            gravityActingOnObject = forceDirection * GC * mass * PlannetOwner.GetComponent<ObjectGravity>().mass / sqrDst;
            force += gravityActingOnObject;
            //now work out the overal ship velocity
            Vector3 acceleration = force / mass;
            velocity += acceleration * Time.fixedDeltaTime;

            objectTransform.location += velocity * Time.fixedDeltaTime;

        }
        objectTransform.Location();
    }

    public Vector3 GravityForce(Vector3 objectLocation)
    {
        Vector3 rv = new Vector3();
        
        //work out the gravity of the object
        //Gravity = GravityConstant * ((m1 * m2)/sq(distance))
        float GC = 0.006f;
        float sqrDst = VectorArithmetic.Length(objectLocation - objectTransform.location);
        Vector3 forceDirection = VectorArithmetic.VectorNormalized(objectLocation - objectTransform.location);

        rv = forceDirection * GC * mass * Ship.GetComponent<Ship>().mass / sqrDst;

        return rv;
    }
}
