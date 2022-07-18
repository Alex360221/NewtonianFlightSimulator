using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    // Creates a line renderer that follows a Sin() function
    // and animates it.

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
    //game object which path will be created for
    public GameObject parent;
    bool running = false;
    public Vector3[] ponitarray;

    void Start()
    {
        //creates the line
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;
        lineRenderer.startColor = c2;
        lineRenderer.endColor = c2;
        ponitarray = new Vector3[lengthOfLineRenderer];
    }

    void Update()
    {
        if (running != true)
        {
            running = true;
            //updates the path of the line
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            //var t = Time.time;
            if (parent != null)
            {
                // postion of ship
                Vector3 postion = parent.GetComponent<Ship>().shipTransform.location;
                postion += parent.GetComponent<Ship>().velocity;

                Vector3 newVolcity = parent.GetComponent<Ship>().velocity;
 
                for (int i = 0; i < lengthOfLineRenderer; i++)
                {
                    Vector3 gravitationalForce = Vector3.zero;
                    Vector3 gravityLocation = new Vector3();
                    //add gravitational force on to force
                    for (int p = 0; p < parent.GetComponent<Ship>().gravityObjectAmount; p++)
                    {
                        if (parent.GetComponent<Ship>().gravityObjects[p] != null)
                        {
                            gravitationalForce += parent.GetComponent<Ship>().gravityObjects[p].GetComponent<ObjectGravity>().GravityForce(postion);
                            gravityLocation += parent.GetComponent<Ship>().gravityObjects[p].GetComponent<ObjectGravity>().objectTransform.location;

                        }
                    }
                    newVolcity -= gravitationalForce;
                    postion += newVolcity;
                    ponitarray[i] = postion;
                    lineRenderer.SetPosition(i, postion);

                }
            }
            running = false;
        }
        
       
    }
}
