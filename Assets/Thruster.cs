using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    //Linear motion
    public Vector3 Force;
    public float Mass = 1;
    public Vector3 thrusterDirection;

     public Vector3 thrusterLocationRelative;

    Vector3 centreOfMass;
    public GameObject shipRef;
    public string thrusterKey;
    KeyCode thisKey;

    public MyTransform thrusterTransform;
    //stuff for stabilisation
    public bool thrusterActive = false;
    public bool thrusterActiveS = false;
    public bool spin = false;
    public Vector3 spinDirection;
    public Vector3 thrusterPointDirection;

    // Start is called before the first frame update
    void Start()
    {

        centreOfMass = shipRef.GetComponent<Ship>().shipTransform.location;
        thisKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), thrusterKey);
        thrusterLocationRelative = thrusterTransform.location;
       
        thrusterTransform.localLocation = thrusterLocationRelative;        
    }
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(thisKey))
        {
            thrusterActive = true;
        }
        if (Input.GetKeyUp(thisKey))
        {
            //stabilisation needs to override this
            thrusterActive = false;
        }

        if (thrusterActive == true || thrusterActiveS == true) { Force += thrusterDirection / 10; }
        else { Force = Vector3.zero; }
    }

    void FixedUpdate()
    {
        //update the direction of the thruster
        thrusterPointDirection = shipRef.GetComponent<Ship>().shipTransform.TRSMatrix * thrusterTransform.localTRSMatrix * (thrusterTransform.location - shipRef.GetComponent<Ship>().shipTransform.location);
    }

    //fucntions used for stabilisation
    public void Active()
    {
        thrusterActiveS = true;
    }
    public void Deactive()
    {
        thrusterActiveS = false;
        Force = Vector3.zero;
    }
}
