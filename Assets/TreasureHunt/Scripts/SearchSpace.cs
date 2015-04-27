using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using Leap;


public struct Phase
{
    int num;
    string pattern;
    bool timed;
    float length, wait, study, rest, completedTrials;


    public Phase(int num, bool timed)
    {
        this.num = num;
        this.timed = timed;
        pattern = "";
        length = wait = study = rest = completedTrials = 0;
    }


    public Phase(int num, bool timed, float length, float wait, float study, float rest)
    {
        this.num = num;
        pattern = "";
        this.timed = timed;
        this.length = length;
        this.wait = wait;
        this.study = study;
        this.rest = rest;
        completedTrials = 0;
    }


};

public enum GrabState
{
    open, grab, release

}


public class SearchSpace : MonoBehaviour
{

    // Access Kinect data to make bounds for search space   
    /* public GameObject BodySourceView;
    private BodySourceView _BodySourceView;
    private float shoulderHeight;
    private float shoulderWidth;
    private float armLength;*/

    /* private float shoulderHeight = 4.396132f;
    private float shoulderWidth = Vector3.Distance(new Vector3(-1.540944f, 4.396132f, 15.13844f), new Vector3( 2.052144f, 4.249248f, 15.0819f));
    private float armLength = Vector3.Distance(new Vector3(-1.540944f, 4.396132f, 15.13844f), new Vector3(-2.761098f, 2.57422f, 14.22677f)) + Vector3.Distance(new Vector3(-2.761098f, 2.57422f, 14.22677f), new Vector3(-1.349743f, 1.365262f, 11.68927f));*/


    /* Use this for initialization, 3 parameters received on Start, shoulder height, shoulder distance, and extended arm length
         Parameters to be used to position and scale searchspace*/

    private enum phaseTypes { Load, SearchBck, Wait, SearchFwd };
    private enum modes { Practice, TimedFwd, TimedBckwd, UTimedFwd, UTimedBckwd };

    private int numSpheres;
    [SerializeField]
    private int activeSphere;
    GameObject[] findSpheres;
    float sphereRatio;
    StimulusScript myStimScript;
    StimulusScript activeStimScript;
    MagneticPinch2[] pinchScripts = new MagneticPinch2[2];
    GameObject lHand;
    GameObject rHand;
    Phase[] modeAry;
    private int SubID;
    int j;
    private int framecount = 0;
    private bool timed;
    private bool randomPattern;
    private bool firstPass = true;
    private byte tempB;
    private string message = "";
    private string boneTemp;
    private string path;
    private int numFound;
    private int startClick = 0;
    private int bytesSent = 0;
    private int mode;
    public string[] stimFoundTimes;
    public List<string> stimMarkTimes = new List<string>();
    private string modeString;
    public string statusMssg = "";
    private Vector3[] gameObjcorners = new Vector3[8];
    private Vector3 rand;
    private Color[] stimColors = new Color[6] { Color.green, Color.blue, Color.cyan, Color.yellow, Color.black, Color.magenta };
    private bool pattern;
    //private BluetoothScript rightHand;
    GameObject randShape;
    Vector3[] randShapeCorners = new Vector3[8];
    public Vector3[] markLocations;
    public int pinchCount = 0;
    public static string COMPort = "COM3";
    public static SerialPort _SerialPort = new SerialPort(COMPort, 9600);
    public static bool Errorhandling = false;
    private int RchannelsPerFinger = 1;
    private bool RpalmChannel = false;
    private int RnumChannelFingers = 5;
    private int LchannelsPerFinger = 1;
    private bool LpalmChannel = false;
    private int exceptionCount = 0; 
    private int LnumChannelFingers = 5;
    private Color[] colors;
    private Dictionary<string, int> ChannelDist = new Dictionary<string, int>{    //use dictionary to alter channels if needed
                    {"palm", 0}, {"palmR", 0},

                    {"thumb1", 0}, {"thumb2", 2}, {"thumb3", 0},

                    {"index1", 1},{"index2", 5},  {"index3", 1},
                  
                    {"middle1", 2},{"middle2", 8},{"middle3", 2},           //order of handpoints in dictionary should omatch order of points in handPOints list 

                    {"ring1", 3},  {"ring2", 11}, {"ring3", 3},

                    {"pinky1", 4}, {"pinky2", 14}, {"pinky3", 4},

};




    private byte[] WriteToRightGlove;
    private byte[] WriteToLeftGlove;
    private List<string> handPoints = new List<string> { "palm", "thumb", "index", "middle", "ring",  "pinky", };

    public GUIStyle myGUIStyle;
    private HandController handControlScript;
    PerformanceCounter cpuCounter;
    PerformanceCounter ramCounter;

    private bool isovrcam;

    private string tempS1; 
    //Trial variables
    private int phase = 0;
    private float phaseWaitTime = 5f; //in s
    private float phaseWaitTimeStart;
    private bool phaseInit;
    private float testTime = 5000f;
    private float searchTime = 35f; //in s
    private float restTime = 1.000f; //in s
    private float studyTime = 25f; //s
    private float tStart;
    private float diff;
    private int numberOfCompletedTrials = 0;
    private int expectedNumberOfTrials;
    GameObject OvrCamObj;
    GameObject MainViewObj;
   GameObject riftTextPlane1;
   public GameObject riftTextPlane2;
   public GameObject riftTextPlane3;
        float grabThreshold = 6f;
    bool grabbingFlag = false;
    int grabCount = 0;
    int loopCount = 0;

    OVRManager riftManagerScript;
   



    void Awake(){

      
        riftManagerScript = GameObject.Find("OVRCameraRig").GetComponent("OVRManager") as OVRManager;
        riftTextPlane1 = GameObject.Find("TextPlane1");
        riftTextPlane2 = GameObject.Find("TextPlane2");
        riftTextPlane3 = GameObject.Find("TextPlane3");
        WriteToLeftGlove = new byte[(LpalmChannel? 1: 0 )+ LchannelsPerFinger * LnumChannelFingers];
        WriteToRightGlove = new byte[(LpalmChannel? 1: 0 )+ LchannelsPerFinger * LnumChannelFingers];
        colors = new Color[]{ Color.blue, Color.red, Color.yellow, Color.green, Color.cyan,   Color.magenta   };
        SubID =  PlayerPrefs.GetInt("Subject Identifier");
        numSpheres = PlayerPrefs.GetInt("Number of Stimuli");
        modeString = PlayerPrefs.GetString("Mode");
        diff =  PlayerPrefs.GetInt("Difficulty");
        expectedNumberOfTrials = PlayerPrefs.GetInt("Number of Trials");
        
        pattern = System.Convert.ToBoolean(PlayerPrefs.GetInt("Pattern"));
        setPatternBool();
        setTimedBool();
        stimFoundTimes = new string[numSpheres];


        cpuCounter = new PerformanceCounter();

        cpuCounter.CategoryName = "Processor";
        cpuCounter.CounterName = "% Processor Time";
        cpuCounter.InstanceName = "_Total";

        ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            path = @"C:\Users\Holodeck\Documents\GitHub\UnityHolodeckProject\Assets\TreasureHunt\SubjectData\ID-" + SubID + "_" + "Time-" + System.DateTime.Now.ToString("yyyy-MM-dd_H-mm") + ".txt";
       
        
        int i = 0;
       /* for (; ; )
        {
            if (File.Exists(@path))
            {
                i++;
                
            }
            else
            {
                path = path.Insert((path.Length - 5), i.ToString());
                break;
            }
        }*/

        
       using( StreamWriter stream = new StreamWriter(path, true)){
       
            string header = "Object Swap Task- Subj: " + SubID + ", Mode: " + modeString + "," + "Pattern Type: " + ((pattern ) ? "Pattern" : "Random") 
                + ", Trials: " + expectedNumberOfTrials + ", #Stim: " + numSpheres + ", Difficulty: " + diff + "\r\n\n";

            stream.WriteLine(header);
           
    }

     
       
        
        
        
        


        OpenConnection();
       
        byte[] test = {8};
        print(test);
        try
        {
            _SerialPort.Write(test, 0, 1);
        }
        catch (System.InvalidOperationException e) {  }

        handControlScript = GameObject.FindObjectOfType(typeof(HandController)) as HandController;

           
    
        
        

        randShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
        randShape.layer = 8;
        randShape.name = "RandShape";
        randShape.renderer.enabled = false;
        Destroy(randShape.collider);
        Destroy(randShape.transform.rigidbody);

        gameObjcorners = CalcSearchCorners(gameObject);

        gameObject.renderer.material.shader = Shader.Find("Transparent/Diffuse");
        Color temp = gameObject.renderer.material.color;
        temp.a = .3f;
        gameObject.renderer.material.color = temp;




        // foreach (Vector3 printVec in gameObjcorners) print("Corner is: (" + printVec.x + ", " + printVec.y + ", " + printVec.z + ")"); // for debugging
    }


    void Start()
    {
        //rightHand = new BluetoothScript();
        //print (rightHand);


       
        sphereRatio = 1f / (PlayerPrefs.GetInt("Difficulty") * (gameObject.transform.localScale.z / 2f));
       // sphereRatio = 1f / 10;  // scaling purposes; largest sphere diameter should be no more than half the width of the search space; need to determine smallest sphere size
        modeAry = new Phase[System.Enum.GetValues(typeof(phaseTypes)).Length];      // sphere size is then normalized on 1-10 scale
       

        for (int i = 0; i < modeAry.Length; i++)
        {
            if (timed & i != 0) modeAry[i] = new Phase(i, true, testTime, phaseWaitTime, searchTime, restTime);
            else modeAry[i] = new Phase(i, false);
        }

        //Use with Kevin's Kinect scripts
        /*
        _BodySourceView = BodySourceView.GetComponent<BodySourceView>();
        shoulderHeight = _BodySourceView.getShoulderHeight();
        shoulderWidth = _BodySourceView.getShoulderWidth();
        armLength = _BodySourceView.getArmLength();
         */

       
        markLocations = new Vector3[numSpheres];

        // Array to hold all stimuli


        findSpheres = new GameObject[numSpheres];


        if (!randomPattern) PatternSpheres();
        else GenerateSpheres();

        //  GameObject.Find("Logger").GetComponent<Logger>().BeginLogging();
        phaseInit = true;



    }

    // Update is called once per frame

    void Update()
    {
        framecount++;
        
       
       
    /*    if(phase ==1 || phase ==2 || phase ==4 ){
            if (!isovrcam)
            {
              isovrcam = true;
              OvrCamObj.SetActive(true);

            }
            else if (isovrcam)
            {
                // set the main cam as true, just in case something messed up or it got disabled somehow
                MainViewObj.SetActive(true);
                OvrCamObj.SetActive(false);
                isovrcam = false;
            }

    } */



      
       
        if (phase == 1 && !phaseInit )
        {
            if (framecount % 20 == 0)
            {
                try
                {

                    setHandPointDistanceBytes();
                    bytesSent = bytesSent + WriteToLeftGlove.Length;
                }
                catch (System.NullReferenceException e) { }
            }
           

           




        }


        try
        {


            
            
            if (phase == 4)
            {
                pinchScripts = GameObject.FindObjectsOfType<MagneticPinch2>();
                foreach (MagneticPinch2 magPinchScript in pinchScripts) magPinchScript.enabled = true;

            }
        }
        catch (System.NullReferenceException e) { }



        if (phase == 0)
        {

            riftManagerScript.enabled = false;
           
        }


        if (phase == 1)
        {

            
            if (phaseInit)
            {
               
                if (timed) tStart = Time.time;
                phaseInit = false;
                activeSphere = 0;
                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
                activeStimScript.setThisActive();
               riftManagerScript.enabled = true;
                


            }

            if (!timed && activeStimScript.getFound() && !phaseChangeFlag())
            {

                

                    

                

                // print("ActiveSphere is: " + active);
                //print("I'm here"); /* For debugging */
                activeStimScript.setThisActive();
                activeSphere++;
                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
                activeStimScript.setThisActive();
            }


            if (Time.time - tStart < searchTime && timed)
            {

                if (activeStimScript.getFound() && !phaseChangeFlag())
                {
                    //print("ActiveSphere is: " + active);
                    //print("I'm here"); /* For debugging */
                    activeStimScript.setThisActive();
                    activeSphere++;
                    activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
                    activeStimScript.setThisActive();
                }
                float t = (searchTime - (Time.time - tStart));


                if (t < .02f) timeLeft = 0 + "";
                else timeLeft = t + "";

            }

            if (phaseChangeFlag())
            {

                statusMssg = "FORWARD TRIAL COMPLETED; Please prepare to enter study phase...";
                riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;
                phase = 2;
                phaseInit = true;

            }
            else if (timed && Time.time - tStart > searchTime && !phaseChangeFlag())
            {
                phase = 0;
                statusMssg = " Time expired; Click to reset";
                string riftMssg = "Time expired";
                riftTextPlane2.GetComponent<TextMesh>().text = riftMssg;
                phaseInit = true;

               riftManagerScript.enabled = false;

            }

        }

        if (phase == 2)
        {
            if (phaseInit)
            {
                OpenConnection();
                byte[] test = new byte[16];
                for (int i = 0; i < 16; i++)
                {

                    test[i] = System.Convert.ToByte(i * 16 + 0);

                }
                _SerialPort.Write(test, 0, 16);
                _SerialPort.Close();
                
                if (loopCount == 0)
                {
                    tStart = Time.time;
                    loopCount++;
                }

                riftManagerScript.enabled = true;


                if (Time.time - tStart > phaseWaitTime)
                {

                    statusMssg = "Now in Study Phase";
                        
                    riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;
                    phaseInit = false;
                }

            }
            if (!phaseInit)
            {
                float t = (studyTime - (Time.time - tStart));


                if (t < .02f) timeLeft = 0 + "";
                else timeLeft = t + "";

                if (Time.time - tStart > studyTime)
                {
                    statusMssg = "STUDY PHASE COMPLETE";
                    riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;
                    phaseInit = true;
                    phase = 3;
                }

            }
        }

        if (phase == 3)
        {
            if (phaseInit)
            {
                   
                tStart = Time.time;
                phaseInit = false;

                riftManagerScript.enabled = false;


            }

            


            if (Time.time - tStart > phaseWaitTime)
            {

                numFound = 0;
                startClick = 0;
                phaseInit = true;
                foreach (GameObject sphere in findSpheres)
                {
                    activeStimScript = sphere.GetComponent("StimulusScript") as StimulusScript;
                    activeStimScript.hide();
                }
                List<GameObject> delList = new List<GameObject>();
                delList.AddRange(GameObject.FindGameObjectsWithTag("TestCube"));
                foreach (GameObject delObj in delList) Destroy(delObj);
                statusMssg = "Click Start for next trial";
                   
                string riftMssg = "Prepare for next trial";
                riftTextPlane2.GetComponent<TextMesh>().text = riftMssg;
            }
        }


        if (phase == 4)
        {

            if (phaseInit)

                riftManagerScript.enabled = true; 
                using (StreamWriter stream = new StreamWriter(path, true))
                {
                    int p = 1;

                        stream.WriteLine("Found Times \r\n");
                    foreach (GameObject gameobj in findSpheres) stream.WriteLine(gameobj.GetComponent<StimulusScript>().getFoundTime() + ",    ");
                    stream.WriteLine("\r\n\nFound Times \r\n");
                    foreach( GameObject sphr in findSpheres){

                        stream.WriteLine("Sphere " + p + " location: " + "(" + sphr.transform.position.x + ", " + sphr.transform.position.y + ", " + sphr.transform.position.z + "),   ");
                    }

                    

                    string s1 = "Recall Phase Time Start: " + Time.time.ToString() + "\r\n\n";

                    stream.Write(s1);

                }
            {
                foreach (GameObject sphere in findSpheres)
                {
                    activeStimScript = sphere.GetComponent("StimulusScript") as StimulusScript;
                    activeStimScript.setThisInactive();
                }
                phaseInit = false;
            }

            if (pinchCount == markLocations.Length)
            {
                try
                {

                    pinchScripts = GameObject.FindObjectsOfType<MagneticPinch2>();
                    foreach (MagneticPinch2 magPinchScript in pinchScripts) magPinchScript.enabled = false;


                }
                catch (System.NullReferenceException e) { }

                statusMssg = "All points recorded! Trial complete.";
                riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;
                phase = 5;
            }




        }
        if (phase == 5)
        {

            riftManagerScript.enabled = false;

            using (StreamWriter stream = new StreamWriter(path, true))
            {

                
                stream.WriteLine("\r\n\n");
                stream.WriteLine("Mark Times \r\n");
                foreach (string str in stimMarkTimes) stream.WriteLine(str + ",   " + "\r\n");
                foreach (Vector3 vec in markLocations) stream.WriteLine("(" + vec.x + ", " + vec.y + ", " + vec.z + "),    ");
                stream.WriteLine("End trial");
            }

            phase = 0;
        }
    }



    //randomly generate # of spheres contained within this gameobject
    void GenerateSpheres(bool randFlag = false)
    {
        int j = Mathf.RoundToInt(Random.Range(0, randShapeCorners.Length - 1));
        bool sphereOverlap = false;

      

      while (sphereOverlap || firstPass) {  
        // creates # of desired spheres, sets their color to yellow, calls isFound method to hide spheres
        for (int i = 0; i < numSpheres; i++)
        {

            findSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            findSpheres[i].layer = 8;
            findSpheres[i].renderer.enabled = false;
            findSpheres[i].collider.isTrigger = true;
            findSpheres[i].AddComponent("Rigidbody");
            findSpheres[i].rigidbody.useGravity = false;
            findSpheres[i].rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            findSpheres[i].renderer.material.color = colors[i];
            findSpheres[i].tag = "Unfound";
            findSpheres[i].AddComponent("StimulusScript");
            myStimScript = findSpheres[i].GetComponent("StimulusScript") as StimulusScript;
            if (randFlag)
            {
                if (!(i > randShapeCorners.Length))
                {
                    //print("J is : " + j);    Debug
                    findSpheres[i].transform.position = randShapeCorners[j];

                    if (j != randShapeCorners.Length - 1) j++;
                    else j = 0;
                }
                else print("Cannot place any more stimuli without second pattern");

                

            }
            else
            {
                // randomly place spheres, position bounded by searchSpace extents

                findSpheres[i].transform.localPosition = new Vector3(Random.Range(gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
                    gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
                    Random.Range(gameObject.transform.position.y - gameObject.transform.localScale.y / 2, gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
                    Random.Range(gameObject.transform.position.z - gameObject.transform.localScale.z / 2, gameObject.transform.position.z + gameObject.transform.localScale.z / 2));
            }
            findSpheres[i].transform.localScale = gameObject.transform.localScale * sphereRatio;

        }
     firstPass = false;
     for (int k = 0; k < numSpheres; k++)
     {
         if (findSpheres[k].GetComponent<StimulusScript>().hittingOtherSphere) sphereOverlap = true;
     }
          }
      
        //Do we need to worry about overlap of stimuli?
    }

    //generate spheres in a polygonal pattern- just using cube pattern for now; 
    void PatternSpheres()
    {

        /* float intAngle = -1; // -1 will represent that a circular pattern is desired
         Vector3 shapeCenter = gameObject.transform.position;  // space center is polygon center
         // first vertex point half way between center and maximum extents of search space
         Vector3 firstVertex = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y / 4, gameObject.transform.position.z);
         if (sides != 0)  intAngle = (sides - 2) * 180 / sides;
         float trajAngle = 180 - intAngle / 2;
         // make side length 1/4 the length of the overal scale ....this shouldn't fall outside bounds, but might for polygons with higher number of vertices
         float sideLength = (.5f) * gameObject.transform.localScale.x / Mathf.Cos(((Mathf.Deg2Rad * 180) - (Mathf.Deg2Rad * intAngle)) / 2);
         for (int i = 0; i < sides; i++)
         {

         }*/

        bool outOfBounds = false;


        do
        {

            randShape.transform.position = new Vector3(Random.Range(gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
                    gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
                    Random.Range(gameObject.transform.position.y - gameObject.transform.localScale.y / 2, gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
                    Random.Range(gameObject.transform.position.z - gameObject.transform.localScale.z / 2, gameObject.transform.position.z + gameObject.transform.localScale.z / 2));

            randShape.transform.localScale = new Vector3(Random.Range((gameObject.transform.localScale.x * sphereRatio) * 2, gameObject.transform.localScale.x),
                    Random.Range((gameObject.transform.localScale.y * sphereRatio) * 2, gameObject.transform.localScale.y), Random.Range((gameObject.transform.localScale.z * sphereRatio) * 2, gameObject.transform.localScale.z));

            outOfBounds = CalcOutOfBounds(randShape);

        }
        while (!outOfBounds);
        randShapeCorners = CalcSearchCorners(randShape);
        GenerateSpheres(true);



    }



    bool phaseChangeFlag()
    {
        //print("In PhaseChangeFlag");
        return numFound == findSpheres.Length;
    }

    void setPatternBool()
    {
        int tempI = PlayerPrefs.GetInt("Pattern");
        randomPattern = (tempI == 0) ? true : false;
    }

    void setTimedBool()
    {
        int tempI = PlayerPrefs.GetInt("Timed");
        timed = (tempI == 1) ? true : false;
    }

    void setPhase()
    {
        phase = PlayerPrefs.GetInt("Phase");

    }


    private string timeLeft = "";
    void OnGUI()
    {
        GUIStyle messageStyle = new GUIStyle();
        messageStyle.normal.textColor = Color.black;
        messageStyle.fontSize = 16;
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.textColor = Color.white;




        GUI.backgroundColor = Color.black;
        GUI.color = Color.white;

        GUI.Label(new Rect(0, 25, 350, 20), statusMssg, messageStyle);

        GUI.Label(new Rect(0, 0, 100, 90), message, messageStyle);

        if (GUI.Button(new Rect(0, 100, 100, 20), "Reset", buttonStyle)) reset();
        if (GUI.Button(new Rect(0, 400, 100, 20), "RandPoint", buttonStyle)) testRandPoint();
        if (GUI.Button(new Rect(150, 60, 100, 20), "TestSimFound", buttonStyle))
        {
            blockColTest(startClick);
            startClick++;
        }
        if (GUI.Button(new Rect(0, 140, 100, 20), "Back", buttonStyle)) Application.LoadLevel("TreasureHuntLoader");


        GUI.Label(new Rect(130, 0, 100, 90), timeLeft, messageStyle);
        riftTextPlane3.GetComponent<TextMesh>().text = timeLeft;
        message = modeString;
        riftTextPlane1.GetComponent<TextMesh>().text = message;
        bool startHit = GUI.Button(new Rect(0, 60, 100, 20), "Start");
        if (startHit && !(phase == 3))
        {
            using (StreamWriter stream = new StreamWriter(path, true))
            {

                string s1 = "Search Phase Time Start: " + Time.time.ToString() + "\r\n\n";

                stream.WriteLine(s1);

            }
            phase = 1;
            phaseInit = true;
            statusMssg = " Forward Trial in Progress";
            riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;

        }
        else if (startHit && phase == 3)
        {
            phase = 4;
            phaseInit = true;
            statusMssg = "Backward Trial in Progress";
            riftTextPlane2.GetComponent<TextMesh>().text = statusMssg;
        }





    }


    void DrawRect(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    void reset()
    {


        List<GameObject> delList = new List<GameObject>();
        delList.AddRange(GameObject.FindGameObjectsWithTag("Unfound"));
        delList.AddRange(GameObject.FindGameObjectsWithTag("Found"));
        delList.AddRange(GameObject.FindGameObjectsWithTag("TestCube"));


        foreach (GameObject delSphere in delList) Destroy(delSphere);

        numFound = startClick = 0;
        timeLeft = searchTime.ToString();
        statusMssg = "Trial reset!";
        phase = 0;
        Start();
    }

    public int getNumFound()
    {
        return numFound;
    }

    public void incNumFound()
    {
        numFound++;
    }

    private void blockColTest(int clickCount)
    {   //for testing
        try
        {
            if (phase == 1)
            {
                Vector3 pos = findSpheres[clickCount].transform.position;
                GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                testCube.transform.localScale = findSpheres[0].transform.localScale;
                testCube.tag = "TestCube";
                testCube.AddComponent("Rigidbody");
                testCube.rigidbody.useGravity = false;
                testCube.renderer.enabled = false;
                testCube.transform.position = pos;
            }
            else if (phase == 4)
            {

                Vector3 pos = findSpheres[findSpheres.Length - clickCount - 1].transform.position;
                GameObject testCube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                testCube2.tag = "TestCube";
                testCube2.AddComponent("Rigidbody");
                testCube2.rigidbody.useGravity = false;
                testCube2.renderer.enabled = false;
                testCube2.transform.position = pos;
            }
        }
        catch (System.IndexOutOfRangeException) { };
    }

    private void testRandPoint()
    {   //for testing
        rand = new Vector3(Random.Range(gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
               gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
               Random.Range(gameObject.transform.position.y - gameObject.transform.localScale.y / 2,
                gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
                Random.Range(gameObject.transform.position.z - gameObject.transform.localScale.z / 2,
                gameObject.transform.position.z + gameObject.transform.localScale.z / 2));
        print("Byte sent is: " + activeStimScript.getIntensity(rand));




        /*switch(dist)
        {
        case 1:
            rightHand.vibrate("rightHand", "rightThumb",155);
            break;
        case 2:
            rightHand.vibrate("rightHand", "rightThumb",175);
            break;
        case 3:
            rightHand.vibrate("rightHand", "rightThumb",195);
            break;
        case 4:
            rightHand.vibrate("rightHand", "rightThumb",215);
            break;
        case 5:
            rightHand.vibrate("rightHand", "rightThumb",235);
            break;
        case 6:
            rightHand.vibrate("rightHand", "rightThumb",255);
            break;
        }*/

    }

    public float getMaxDist()
    {

        float maxDist = 0;
        foreach (Vector3 compare in gameObjcorners)
        {
            float tempF = Vector3.Distance(gameObjcorners[0], compare);
            if (tempF > maxDist) maxDist = tempF;
        }
        return maxDist;
    }



    private Vector3[] CalcSearchCorners(GameObject shape)
    {
        Vector3[] shapeCorners = new Vector3[8];
        int[] xOnes = new int[] { 0, 0, 0, 0, 1, 1, 1, 1 };
        int[] yOnes = new int[] { 1, 1, 0, 0, 1, 1, 0, 0 };

        //1,2- top, neg x; 3,4- btm, neg x; 5,6- top, fwd x; 7, 8- btm, fwd x

        for (int i = 0; i < 8; i++)
        {
            shapeCorners[i] = new Vector3(shape.transform.position.x - (Mathf.Pow(-1, xOnes[i]) * shape.transform.localScale.x / 2), shape.transform.position.y - Mathf.Pow(-1, yOnes[i]) * shape.transform.localScale.y / 2,
                shape.transform.position.z - Mathf.Pow(-1, i) * shape.transform.localScale.z / 2);

        }

        return shapeCorners;
    }
    private bool CalcOutOfBounds(GameObject shape)
    {
        if (shape.transform.position.x + shape.transform.localScale.x / 2 > gameObjcorners[7].x) return false;
        if (shape.transform.position.x - shape.transform.localScale.x / 2 < gameObjcorners[0].x) return false;
        if (shape.transform.position.y + shape.transform.localScale.y / 2 > gameObjcorners[0].y) return false;
        if (shape.transform.position.y - shape.transform.localScale.y / 2 < gameObjcorners[7].y) return false;
        if (shape.transform.position.z + shape.transform.localScale.z / 2 > gameObjcorners[7].z) return false;
        if (shape.transform.position.z - shape.transform.localScale.z / 2 < gameObjcorners[0].z) return false;
        return true;



    }


    public static bool OpenConnection()
    {
        print("in openconnection");
        print(_SerialPort);
        if (_SerialPort != null)
        {
            if (_SerialPort.IsOpen)
            {
                byte[] test = { 0 };
                _SerialPort.Write(test, 0, 1);
                //_SerialPort.Close();
                print("closing port, because it was already open!");
                return false;
            }
            else
            {
                try
                {
                    print("trying to open");
                    _SerialPort.Open();
                    if (!Errorhandling)
                    {
                        print("port open = " + _SerialPort.IsOpen);
                    }
                }
                catch (System.IO.IOException e)
                {
                    if (e.Source != null)
                        print("could not pair with bt device!");
                    return false;
                }
            }
        }
        else
        {
            if (_SerialPort.IsOpen)
            {
                print("port is already open");
            }
            else
            {
                print("port == null");
                return false;
            }
        }
        return true;
    }

    void OnApplicationQuit()
    {
        closeAll();

        
    }

    void closeAll()
    {
        byte[] test = new byte[16];
        for (int i = 0; i < 16; i++)
        {

            test[i] = System.Convert.ToByte(i * 16 + 0);

        }
        _SerialPort.Write(test, 0, 16);
        _SerialPort.Close();
    }

   private void setHandPointDistanceBytes()
    {
       

      /*  try
        {
            if (RpalmChannel)
            {
                


                
               tempB = System.Convert.ToByte((ChannelDist["palmR"])*16 + activeStimScript.getIntensity(GameObject.Find("RigidHandRight(Clone)").transform.Find("palmR").transform.position)) ;
                
                WriteToRightGlove[0] = tempB;
                
            }
        }
        catch (System.NullReferenceException e) { }
        try{
        if (RnumChannelFingers > 0)
        {
            for (int j = 1; j < RnumChannelFingers+1; j++)
            {
                string tempS = handPoints[j];
                if (RchannelsPerFinger != 1)
                {
                    for (int z = 1; z < RchannelsPerFinger; z++)
                    {

                        string boneTemp = System.String.Concat("bone", z.ToString());
                        string tempS1 = System.String.Concat(handPoints[j], j.ToString());
                      
                        tempB =  System.Convert.ToByte((ChannelDist[tempS1]) *16 + activeStimScript.getIntensity(GameObject.Find("RigidHandRight(Clone)").transform.Find(handPoints[j]).Find(boneTemp).transform.position)) ;

                        WriteToRightGlove[j*z] = tempB;
                    }
                }
                else
                {
                    
                    string boneTemp = System.String.Concat("bone", 3.ToString());
                    string tempS1 = System.String.Concat(handPoints[j], 3.ToString());
                 
                    tempB =  System.Convert.ToByte((ChannelDist[tempS1]) * 16 + activeStimScript.getIntensity(GameObject.Find("RigidHandRight(Clone)").transform.Find(handPoints[j]).Find(boneTemp).transform.position));

                    if (RpalmChannel) WriteToRightGlove[j] = tempB;
                    else WriteToRightGlove[j - 1] = tempB;
                }

            }
        }
        }
        catch (System.NullReferenceException e) { }

        try { 
        if (LpalmChannel)
        {
           
            tempB = System.Convert.ToByte((ChannelDist["palm"]) * 16 + activeStimScript.getIntensity(GameObject.Find("RigidHandRight(Clone)").transform.Find("palm").transform.position));
            WriteToLeftGlove[0] = tempB;
            
        }
        }
        catch (System.NullReferenceException e) { }*/


        try { 
        if (LnumChannelFingers > 0)
        {
            for ( j = 1; j < LnumChannelFingers+1 ; j++)
            {
                string tempS = handPoints[j];
                if (LchannelsPerFinger != 1)
                {
                    for (int z = 1; z < LchannelsPerFinger; z++)
                    {

                        boneTemp = System.String.Concat("bone", z.ToString());
                         tempS1 = System.String.Concat(handPoints[j], j.ToString());
                       
                        tempB = System.Convert.ToByte((ChannelDist[tempS1]) * 16 + activeStimScript.getIntensity(GameObject.Find("RigidHandLeft(Clone)").transform.Find(handPoints[j]).Find(boneTemp).transform.position));

                        WriteToLeftGlove[j*z] =tempB;
                    }
                }
                else
                {
                     boneTemp = System.String.Concat("bone", 3.ToString());
                     tempS1 = System.String.Concat(handPoints[j], 3.ToString());
                  
                    tempB = System.Convert.ToByte((ChannelDist[tempS1]) * 16 + activeStimScript.getIntensity(GameObject.Find("RigidHandLeft(Clone)").transform.Find(handPoints[j]).Find(boneTemp).transform.position));

                    if (LpalmChannel) WriteToLeftGlove[j] = tempB;
                    else WriteToLeftGlove[j - 1] = tempB;
                }

            }
        }
        }
        catch (System.NullReferenceException e) { exceptionCount++; }
        _SerialPort.Write(WriteToLeftGlove, 0, (WriteToLeftGlove.Length));
    }

    public string getPathToSubDataFile()
    {
        return path;
    }


    public int getActiveSphere()
    {
        return activeSphere;
    }


    
}
