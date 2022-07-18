using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilisation : MonoBehaviour
{
    public GameObject ship;
    public bool stabilisation = false;
    public bool stabilisationActive = false;
    public bool parking = false;

    public Vector3 roundedangularVelocity;
    public Vector3 roundedVelocity;
    public Vector3 roundedForwardThrusterDirection;

    public int TargetCube = 0;
    public int CurrentCube = 0;
    public int StartCube = 0;

    public Vector2 Target;
    public Vector2 Current;

    public GameObject[] spinThruster;
    public GameObject[] forwardThruster;

    public int thrusterAmount;

    GameObject thisThrusterY;// = new GameObject();

    //moving from A to B
    public bool MoveToTarget = false;
    public Vector3 TargetLocation;
    public bool workOutStartCube = false;

    // Start is called before the first frame update
    void Start()
    {
        spinThruster = new GameObject[thrusterAmount];
        forwardThruster = new GameObject[thrusterAmount];
        //loops through the array
        for (int i = 0; i < thrusterAmount; i++)
        {
            //checks the slot is empty
            if (spinThruster[i] == null)
            {
                //thruster name + i (i is the number of the thruster)
                spinThruster[i] = GameObject.Find("Thruster" + i);
                if (spinThruster[i].GetComponent<Thruster>().spin == false)
                {
                    forwardThruster[i] = spinThruster[i];                              
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //turn on or off stabilisation
        if (Input.GetKeyDown(KeyCode.P))
        {
           if(stabilisation == false) { stabilisation = true; stabilisationActive = true; }
           else { stabilisation = false; stabilisationActive = false; }
        }

        if (stabilisation != false)
        {
            //stop ship 
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (parking == false) 
                { 
                    parking = true;
                    StartCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));
                }
                else { parking = false; StartCube = 0; }
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (MoveToTarget == false)
                {
                    MoveToTarget = true;
                    StartCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));
                    workOutStartCube = true;
                }
                else { MoveToTarget = false; StartCube = 0; workOutStartCube = false; }
            }

        }
        

        //disables stabilisation if thruster is active
        if (stabilisation != false)
        {
            for (int i = 0; i < thrusterAmount; i++)
            {
                if (spinThruster[i] != null)
                {
                    if (spinThruster[i].GetComponent<Thruster>().thrusterActive == true) { stabilisationActive = false; break;/*i = 10*/; }
                    else { stabilisationActive = true; }
                }
            }
        }     
    }
    void FixedUpdate()
    {
       
        if (ship != null)
        {
            //physics stuff
            roundedangularVelocity = VectorArithmetic.RoundVector(ship.GetComponent<Ship>().angularVelocity, 1000);
            //the rounded velocity is what we will work off for this section

            if (stabilisationActive == true)
            {
                // as no thrusters are active we can stablisie the ship
                if (roundedangularVelocity.y > 0.00)
                {
                    //turns ship 
                    TurnShipY(-1);                   
                }
                else if (roundedangularVelocity.y < 0.00)
                {
                    //turns ship 
                    TurnShipY(1);                   
                }
                else
                {
                    //turns off all thruster of the angular velocity = 0
                    StopTurnY();
                }

                if (roundedangularVelocity.z > 0.00)
                {
                    //turns ship 
                    TurnShipZ(-1);                   
                }
                else if (roundedangularVelocity.z < 0.00)
                {
                    //turns ship 
                    TurnShipZ(1);                   
                }
                else
                {
                    //turns off all thruster of the angular velocity = 0
                    StopTurnZ();
                }
            }
            else
            {
                StopTurnY();
                StopTurnZ();
            }

            //this gets an directional vector and rounds it
            roundedVelocity = VectorArithmetic.RoundVector(VectorArithmetic.VectorNormalized(ship.GetComponent<Ship>().velocity * 5), 100);
            roundedForwardThrusterDirection = VectorArithmetic.RoundVector(VectorArithmetic.VectorNormalized(forwardThruster[0].GetComponent<Thruster>().thrusterPointDirection * 5), 100);



            if (MoveToTarget == true)
            {
                if (workOutStartCube == true)
                {
                    workOutStartCube = false;
                    StartCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));
                    
                }
                //move ship to target
                //directional vector to target location
                Vector3 directionalVector = VectorArithmetic.RoundVector(VectorArithmetic.VectorNormalized(TargetLocation - ship.GetComponent<Ship>().shipTransform.location), 100);
                //print(directionalVector);

                //turn ship to face in that direction
                Current = ConvertAngle(roundedForwardThrusterDirection);
                //convert angle around 2,
                AdjustTargetPCurrent(ConvertAngle(InvertVector(directionalVector)));
                Vector3 shipLocation = ship.GetComponent<Ship>().shipTransform.location;
                if (WithinArea(TargetLocation.x,20, shipLocation.x) == false 
                    || WithinArea(TargetLocation.y, 20, shipLocation.y) == false
                    || WithinArea(TargetLocation.z, 20, shipLocation.z) == false)
                {
                    //not at target point yet
                    //check if thrusters at at the right point
                    if ((WithinArea(Target.x, 0.05f, Current.x) == false) || (WithinArea(Target.y, 0.05f, Current.y) == false))
                    {
                        print("Turn");
                        StopShipForward();
                        TurnShipToFaceDirection(directionalVector); 
                    }
                    else
                    {
                        workOutStartCube = true;
                        //facing right way to thrust ship in direction
                        //check Speed of ship 
                        print("AdjustSpeed");
                        AdjustSpeed(2);
                    }
                }
                else
                {
                    //AdjustSpeed(1);
                    //at target point
                    //print("At Target point Stop ship");                   
                    //is it facing the right way
                    //updates current so it has the converted angle in
                    Current = ConvertAngle(roundedForwardThrusterDirection);
                    AdjustTargetPCurrent(ConvertAngle(roundedVelocity));
                    if ((WithinArea(Target.x, 0.1f, Current.x) == false) || (WithinArea(Target.y, 0.1f, Current.y) == false))
                    {
                        StopShipForward();
                        //this works out the what cubes the target and currrent is in
                        TargetCube = FindCube(ConvertAngle(roundedVelocity));
                        CurrentCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));

                        FindPath(TargetCube, CurrentCube);
                    }
                    else
                    {
                        //print("Slow Ship");
                        if (VectorArithmetic.ReferenceEquals(VectorArithmetic.RoundVector(ship.GetComponent<Ship>().velocity, 100), Vector3.zero) == false)
                        {
                           // print("Come to stop");
                            StopShip();

                        }
                        else
                        {
                            //complete 
                            StopShipForward();
                        }
                    }
                }   
            }

            if (parking == true)
            {
                //updates current so it has the converted angle in
                Current = ConvertAngle(roundedForwardThrusterDirection);
                //this works out the what cubes the target and currrent is in
                TargetCube = FindCube(ConvertAngle(roundedVelocity));
                CurrentCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));

                AdjustTargetPCurrent(ConvertAngle(roundedVelocity));


                FindPath(TargetCube, CurrentCube);
            }
        }
    }

    private GameObject FindThruster(Vector3 spinDirection)
    {
        for (int i = 0; i< thrusterAmount; i++)
        {
            if (spinThruster[i] != null)
            {
                //check if thruster is the one we want
                if (spinThruster[i].GetComponent<Thruster>().spinDirection == spinDirection)
                {
                    return spinThruster[i];
                }
            }
        }
        return null;
    }
    private void TurnShipY(float turnWay)
    {      
        if (turnWay == -1)
        {
            //find thruster
            thisThrusterY = FindThruster(new Vector3(0, -1, 0));
            if (thisThrusterY != null)
            {

                //active thruster in opposit direction
                thisThrusterY.GetComponent<Thruster>().Active();
                //deactives other thruster
                FindThruster(new Vector3(0, 1, 0)).GetComponent<Thruster>().Deactive();
            }
        }
        else if (turnWay == 1)
        {
            //check if its spinning to fast  

            thisThrusterY = FindThruster(new Vector3(0, 1, 0));
            if (thisThrusterY != null)
            {
                //active thruster in opposit direction
                thisThrusterY.GetComponent<Thruster>().Active();
                //deactives other thruster
                FindThruster(new Vector3(0, -1, 0)).GetComponent<Thruster>().Deactive();
            }
        }        
    }
    private void StopTurnY()
    {
        //turns off all thruster of the angular velocity = 0
        thisThrusterY = FindThruster(new Vector3(0, -1, 0));
        thisThrusterY.GetComponent<Thruster>().Deactive();
        thisThrusterY = FindThruster(new Vector3(0, 1, 0));
        thisThrusterY.GetComponent<Thruster>().Deactive();
    }
    private void TurnShipZ(float turnWay)
    {     
        if (turnWay == -1)
        {
            //if (SpinningToFast(roundedangularVelocity.z, 0.3f) == false)
            //{
                //find thruster
                thisThrusterY = FindThruster(new Vector3(0, 0, -1));
                if (thisThrusterY != null)
                {
                    //active thruster in opposit direction
                    thisThrusterY.GetComponent<Thruster>().Active();
                    //deactives other thruster
                    FindThruster(new Vector3(0, 0, 1)).GetComponent<Thruster>().Deactive();
                }
            //}
            //else
            //{
            //    FindThruster(new Vector3(0, 0, -1)).GetComponent<Thruster>().Deactive();
            //    //FindThruster(new Vector3(0, 0, 1)).GetComponent<Thruster>().Active();
            //}
        }
        else if (turnWay == 1)
        {
            //if (SpinningToFast(roundedangularVelocity.z, 0.3f) == false)
            //{
                //find thruster
                thisThrusterY = FindThruster(new Vector3(0, 0, 1));
                if (thisThrusterY != null)
                {
                    //active thruster in opposit direction   
                    thisThrusterY.GetComponent<Thruster>().Active();
                    //deactives other thruster
                    FindThruster(new Vector3(0, 0, -1)).GetComponent<Thruster>().Deactive();
                }
            //}
            //else
            //{
            //    FindThruster(new Vector3(0, 0, 1)).GetComponent<Thruster>().Deactive();
            //    //FindThruster(new Vector3(0, 0, -1)).GetComponent<Thruster>().Active();
            //}
        }
        
    }
    private void StopTurnZ()
    {
        //turns off all thruster of the angular velocity = 0
        thisThrusterY = FindThruster(new Vector3(0, 0, -1));
        thisThrusterY.GetComponent<Thruster>().Deactive();
        thisThrusterY = FindThruster(new Vector3(0, 0, 1));
        thisThrusterY.GetComponent<Thruster>().Deactive();
    }
    private void MoveShipForward(float direction)
    {
        if (direction == -1)
        {
            thisThrusterY = FindThruster(new Vector3(-1, 0, 0));
            if (thisThrusterY != null)
            {
                //active thruster in opposit direction
                thisThrusterY.GetComponent<Thruster>().Active();
                //deactives other thruster
                FindThruster(new Vector3(1, 0, 0)).GetComponent<Thruster>().Deactive();
            }
        }
        else if (direction == 1)
        {
            thisThrusterY = FindThruster(new Vector3(1, 0, 0));
            if (thisThrusterY != null)
            {
                //active thruster in opposit direction   
                thisThrusterY.GetComponent<Thruster>().Active();
                //deactives other thruster
                FindThruster(new Vector3(-1, 0, 0)).GetComponent<Thruster>().Deactive();
            }
        }
    }
    private void StopShipForward()
    {
        FindThruster(new Vector3(1, 0, 0)).GetComponent<Thruster>().Deactive();
        FindThruster(new Vector3(-1, 0, 0)).GetComponent<Thruster>().Deactive();
    }

    private void AdjustSpeed(float Speed)
    {
        //Speed = 4;
        //this slows or speeds up the ship depending on the speed input
        Vector3 currentVelocity = ship.GetComponent<Ship>().velocity;
        //works out the current speed
        if (currentVelocity.x < 0) { currentVelocity.x *= -1; }
        if (currentVelocity.y < 0) { currentVelocity.y *= -1; }
        if (currentVelocity.z < 0) { currentVelocity.z *= -1; }
        float currentSpeed = currentVelocity.x + currentVelocity.y + currentVelocity.z;
        
        if (currentSpeed < Speed)
        {
            //increase speed
            //print("Speed");
            MoveShipForward(1);
        }
        else if (currentSpeed > Speed && currentSpeed >0)
        {
            //going to fast slow down speed
            //print("Slow");
            MoveShipForward(-1);
        }
        else
        {
            //print("Stop thrusters!!!");
            StopShipForward();
        }
       // print("MAx Speed" + Speed);
        //print("Currrent Speed" + currentSpeed);

    }
    private void StopShip()
    {
        Vector3 currentVelocity = ship.GetComponent<Ship>().velocity;
        //works out the current speed
        if (currentVelocity.x < 0) { currentVelocity.x *= -1; }
        if (currentVelocity.y < 0) { currentVelocity.y *= -1; }
        if (currentVelocity.z < 0) { currentVelocity.z *= -1; }
        float currentSpeed = Mathf.Round(currentVelocity.x + currentVelocity.y + currentVelocity.z* 1000) / 1000;

        if (currentSpeed < 0)
        {
            //print("Speed SlowDown please");
            MoveShipForward(1);
        }
        else if (currentSpeed > 0)
        {
           // print("Speed Up please");
            MoveShipForward(1);
        }
        


    }
    private bool WithinArea(float AxisVaule, float MinMax, float currentVaule)
    {
        if (currentVaule > (AxisVaule -MinMax) && currentVaule < (AxisVaule + MinMax))
        {
            //true within area
            return true;
        }
        //false not in area
        return false;
    }
    private void AdjustTargetPCurrent(Vector2 RawTarget)
    {
        //this fucntion adjust the current position so the target is always 2,2
        //so the current position will move by the target amount 
        //2 - target to work out how much the current needs to change by
        float tx = 2 - RawTarget.x;
        float ty = 2 - RawTarget.y;
        //works out current x and curent y from tx and ty
        if (tx < 0) //X
        {
            //if negative it needs to move back on itself so we go back from our max num which is 3.99
            if (Current.x < 2)
            {

                if (Current.x + tx <= 0) { Current.x = 4 + (tx + Current.x); } //less than 0 how much is left
                else{ Current.x += tx;}
            }
            else
            {
                if (Current.x + tx >= 4){ Current.x = tx + (4 - Current.x);}
                else { Current.x += tx; }
            }
        }
        else
        {
            if (Current.x + tx >= 4){ Current.x = tx + (4 - Current.x); }
            else{ Current.x += tx; }
        }

        if (ty < 0) //Y
        {
            if (Current.y < 2)
            {
                if (Current.y + ty <= 0) { Current.y = 4 + (ty + Current.y); }//less than 0 how much is left
                else{Current.y += ty;}
            }
            else
            {
                if (Current.y + ty >= 4){Current.y = ty + (4 - Current.y);}
                else{Current.y += ty;}
            }
        }  //if negative it needs to move back on itself so we go back from our max num which is 3.99
        else
        {
            if (Current.y + ty >= 4){Current.y = ty + (4 - Current.y);}
            else { Current.y += ty;}
        }

        Target.x = 2;
        Target.y = 2;
    }
    private Vector2 ConvertAngle(Vector3 angle)
    {
        Vector2 rv;
        //x = z rotation
        //y = y rotation

        if (angle.z >= 0)
        {
            //z == top half / postive
            if (angle.x >= 0)
            {
                //x == right side / postive
                //Section A
                //A = z
                rv.x = angle.z;
            }
            else
            {
                // x == left side / negative
                //Section B
                //B = (1 - z) + 1
                rv.x = (1 - angle.z) + 1;
            }
        }
        else
        {
            //z == bottom half / negative
            if (angle.x < 0)
            {
                // x == left side / negative
                //Section C   
                //C = (z *-1) +2
                rv.x = (angle.z * -1) + 2;
            }
            else
            {
                //x == right side / postive
                //Section D
                //D = (1-(z *-1)) +3
                rv.x = (1 - (angle.z * -1)) + 3;
            }
        }

        if (angle.y >= 0)
        {
            //y == top half / postive
            if (angle.x >= 0)
            {
                //x == right side / postive
                //Section A
                //A = y
                rv.y = angle.y;
            }
            else
            {
                // x == left side / negative
                //Section B
                //B = (1 - y) + 1
                rv.y = (1 - angle.y) + 1;
            }
        }
        else
        {
            //y == bottom half / negative
            if (angle.x < 0)
            {
                // x == left side / negative
                //Section C   
                //C = (y *-1) +2
                rv.y = (angle.y * -1) + 2;
            }
            else
            {
                //x == right side / postive
                //Section D
                //D = (1-(y *-1)) +3
                rv.y = (1 - (angle.y * -1)) + 3;
            }
        }
        return rv;
    }
    private Vector3 InvertVector(Vector3 A)
    {
        //inverts the directional vector
        Vector3 rv;
        rv.x = A.x * -1;
        rv.y = A.y * -1;
        rv.z = A.z * -1;
        return rv;
    }
   private int FindCube(Vector2 A)
    {
        A = VectorArithmetic.RoundVector(A, 100);
        if (A.y < 2)
        {
            //top half
            if (A.x < 2)
            {
                //right side
                if (A.x < 1)
                {
                    //A
                    return 1;
                }
                else
                {
                    //B
                    return 2;
                }
            }
            else
            {
                //Left side
                if (A.x < 3)
                {
                    //C
                    return 3;
                }
                else
                {
                    //D
                    return 4;
                }
            }
        }
        else
        {
            //bottome half
            if (A.x < 2)
            {
                //right side
                if (A.x < 1)
                {
                    //E
                    return 5;
                }
                else
                {
                    //F
                    return 6;
                }
            }
            else
            {
                //Left side
                if (A.x < 3)
                {
                    //G
                    return 7;
                }
                else
                {
                    //H
                    return 8;
                }
            }
        }
    }
    private void FindPath(int T, int C)
    {
        bool X = false;
        bool Y = false;
        //this finds the target column
        int tc = 0;
        int tcP = 0;
        int tcM = 0;

        int tP = 0;
        int tM = 0;

        //works out the target column plus 1 and minus 1 positions
        if (T <= 4)
        {
            //adds 4 as thats how many cubes are in the layer to get the cube below or above
            //less tahn 4
            tc = T + 4; 
            //sets the T + and -
            if (T-1 < 1) { tM = 4; }
            else { tM = T - 1; }
            if (T+1 > 4) { tP = 1; }
            else { tP = T + 1; }
            
            //sets the TC + and -
            if (tc - 1 < 5) { tcM = 8; }
            else { tcM = tc - 1; }
            if (tc + 1 > 8) { tcP = 5; }
            else { tcP = tc + 1; }
        }  
        else 
        {
            //adds 4 as thats how many cubes are in the layer to get the cube below or above
            //greater than 4
            tc = T - 4;
            //sets the T + and -
            if (T - 1 <= 4) { tM = 8; }
            else { tM = T - 1; }
            if (T + 1 >= 8 ) { tP = 5; }
            else { tP = T + 1; }
            
            //sets the TC + and -
            if (tc - 1 < 1) { tcM = 4; }
            else { tcM = tc - 1; }
            if (tc + 1 > 4) { tcP = 1; }
            else { tcP = tc + 1; }
        }

        if (C != T && C != tc)
        {         
            //not in right column
            if(C == tcM || C == tM)
            {
                //print("C Minus");
                //at column -1
                if (T <= 4)
                {
                    //print("T T");
                    //Target is top 

                    if (C == tM)
                    {
                        //at top move across (1)
                       // print("C==m");
                        TurnShipY(1);                            
                    }
                    else
                    {
                        //print("C!=m");                          
                        TurnShipY(1);
                        TurnShipZ(-1);
                    }
                }
                else
                {
                    //print("T b");
                    //Target is bottom
                    if (C == tM)
                    {
                        //at bottom move across (1) 
                        //print("C==m");
                        if ((StartCube == 3 && T == 8) || (StartCube == 7 && T == 8))   //cube cases
                        {
                            TurnShipZ(-1);      //adjustment for cube cases
                        }
                        else
                        {
                            TurnShipY(1);   //main adjusment used for when no cube case
                        }                           
                    }
                    else
                    {
                       // print("C!=m");
                        //at top move down (1) and across (1)
                        if ((StartCube == 3 && T == 8) || (StartCube == 7 && T == 8) 
                            || (StartCube == 1 && T == 6))//cube cases
                        {
                            TurnShipY(1);   //cube case adjusment
                            TurnShipZ(-1);
                        }
                        else
                        {
                            TurnShipY(1);   //main adjustments to use when no cube cases
                            TurnShipZ(1);
                        }                            
                    }
                }
            }
            else if (C == tcP || C == tP)
            {
                //print("C Plus");
                //at column +1
                if (T <= 4)
                {
                    //print("T T");
                    //Target is top 
                    if (C == tP)
                    {
                        //at top move across -1
                       // print("C==m");
                        TurnShipY(-1);
                    }
                    else
                    {
                        //print("C!=m");
                        //at bottom move up(-1) and across (-1)
                        TurnShipY(-1);
                        TurnShipZ(-1);

                    }
                }
                else
                {
                   // print("T b");
                    //Target is bottom
                    if (C == tP)
                    {
                        //at bottome move across -1
                       // print("C==m");
                        TurnShipY(-1);
                    }
                    else
                    {
                        //print("C!=m");
                        //at top move down(1) and across(-1)  
                        TurnShipY(-1);
                        TurnShipZ(1);

                    }
                }
            }
            else
            {
                //not in next column
                TurnShipY(-1);
            }
        }
        else if (C != T)
        {
           // print("Right column");
            if (T <= 4 && C > 4)
            {
               // print("Target top, Current below");
                TurnShipZ(-1);
            }
            else if (T > 4 && C <= 4)
            {
                //print("TargetBelow, Current Above");
                if ((StartCube == 1 && T == 6)) //cube case
                {
                    TurnShipY(1);
                }
                else
                {
                    TurnShipZ(1); //main adjustment used when their is no cube case
                }                   
            }
            else
            {
                //print("Error");
            }
        }
        else
        {
            //X adjustment
            //print("In Right Cube");
            if (WithinArea(Target.x, 0.1f, Current.x) == false)
            {
                int iv = 0;
                if (T <= 4)
                { 
                    //target is top layer
                    iv = 1; 
                }
                else
                { 
                    //target is bottom layer
                    if (StartCube <=4) { iv = 1; }
                    else { iv = 1; }
                }
               // print("Fianl Adjust of x");
                if (Current.x < Target.x)
                {
                    //move towrds target
                    TurnShipY(1 * iv);
                }
                else if (Current.x > Target.x)
                {
                    TurnShipY(-1 * iv);
                }
            }
            else
            {
                //X is at right point
               // print("X Good");
                X = true;
            }

            //adjust of Y within area
            if (WithinArea(Target.y, 0.1f, Current.y) == false)
            {
                //print("Fianl Adjust of y");
                int iv = 0;
                if (T <= 4)
                {
                    //target is top layer
                    if ((StartCube == 1 && T ==2) || (StartCube == 5 && T == 2)
                        || (StartCube == 8 && T == 2) )
                    {iv = -1;}
                    else {iv = 1; }            
                }
                else
                {
                    //Works out the invert for certain cases
                    if (StartCube <= 4) { iv = 1; }
                    else 
                    { 
                        if ((StartCube == 3 && T == 6)) { iv = -1; }
                        else{ iv = 1;}
                    }
                }
                //adjust the Y 
                if (Current.y < Target.y)
                {
                    TurnShipZ(-1 * iv);
                }
                else if (Current.y > Target.y)
                {
                    TurnShipZ(1 * iv);
                }
            }
            else
            {
                //Y is at the right point
                //print("Y Good");
                Y = true;
            }
        }

        if (X == true && Y ==true)
        {
            //ship turn to face right direction
            //print("Complete!!!!");           
            parking = false;
        }
    }
    private void TurnShipToFaceDirection(Vector3 directionalVector)
    {
        //invert direction vector
        Vector3 InvertedDirectionalVector = InvertVector(directionalVector);
        //turn ship to face in that direction
        //Current = ConvertAngle(roundedForwardThrusterDirection);
        //this works out the what cubes the target and currrent is in
        TargetCube = FindCube(ConvertAngle(InvertedDirectionalVector));
        CurrentCube = FindCube(ConvertAngle(roundedForwardThrusterDirection));
        //convert angle around 2,
       // AdjustTargetPCurrent(ConvertAngle(InvertedDirectionalVector));
        //turn ship to face target
        FindPath(TargetCube, CurrentCube);
    }
}


