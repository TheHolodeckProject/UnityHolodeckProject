using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Leap;


public struct Phase{
    int num;
    string pattern;
    bool timed;
    float length , wait , study , rest , completedTrials ;
  

    public Phase(int num, bool timed)
    {
        this.num = num;
        this.timed = timed;
        pattern = "";
        length =  wait = study = rest = completedTrials = 0;
    }


    public Phase(int num,  bool timed, float length, float wait, float study, float rest)
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


public class SearchSpace : MonoBehaviour {
    
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
    [SerializeField] private int activeSphere;
    GameObject[] findSpheres;
    float sphereRatio;
    StimulusScript myStimScript;
    StimulusScript activeStimScript;
    Phase[] modeAry;
    private bool randomPattern;
    private bool timed;
    private string message = "";
    private int numFound;
    private int startClick = 0;
    private int mode;
    private string statusMssg = "";
    private string studyTimeLeft = "";
    private Vector3[] gameObjcorners = new Vector3[8];
	private Vector3 rand;
	//private BluetoothScript rightHand;
    GameObject randShape;
    Vector3[] randShapeCorners = new Vector3[8];
    Vector3[] markLocations;
    

    public GUIStyle myGUIStyle;
    private HandController handControlScript;
   
    //Trial variables
    private int phase = 0;
    private float studyTime = 10.0f; //in s
    private float phaseWaitTimeStart;
    private bool phaseInit;
    private bool timeExpired = false;
    private float testTime = 5000f;
    private float searchTime = 5000f; //in s
    private float restTime = 3.000f; //in s
    private float tStart;
    private int numberOfCompletedTrials = 0;
    private int expectedNumberOfTrials;
    GameObject OccViewObjLeft;
    GameObject OccViewObjRight;
    GameObject MainViewObj;
    grabData grabD = new grabData();
    float grabThreshold = 3f;
    bool grabbingFlag = false;
    int grabCount = 0;
    Vector3[] lineRensPoints = new Vector3[4];
    LineRenderer[] lineRens;
    HandController controller;

    // Haptic Glove Variables
    public GameObject RigidHandRight;
    public UnitySerialPort UnitySerialPortRightHand;

    void Awake()
    {

        print(GameObject.Find("ControllerSandBox").transform.position);
        handControlScript = GameObject.FindObjectOfType(typeof(HandController)) as HandController; //Redundant code....TODO: need to fix/clean up later
         OccViewObjLeft= GameObject.Find("CameraLeft");
         OccViewObjRight = GameObject.Find("CameraRight");
         MainViewObj = GameObject.Find("MainCameraView");
        randShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
        randShape.layer = 8;
        randShape.name = "RandShape";
        randShape.renderer.enabled = false;
        Destroy(randShape.collider);
        Destroy(randShape.transform.rigidbody);
       
        gameObjcorners = CalcSearchCorners(gameObject);
       int j =0;
        for(int i = 0; i < gameObjcorners.Length; i++){
             if(gameObjcorners[i].y > gameObject.transform.position.y){
                 lineRensPoints[j] = gameObjcorners[i];
                 j++;
             }
             }
        Vector3 tempVec = lineRensPoints[2];
        lineRensPoints[2] = lineRensPoints[3];
        lineRensPoints[3] = tempVec;
        LineRenderer lineren1 = new GameObject().AddComponent<LineRenderer>();
        LineRenderer lineren2 = new GameObject().AddComponent<LineRenderer>();
        LineRenderer lineren3 = new GameObject().AddComponent<LineRenderer>();
        LineRenderer lineren4 = new GameObject().AddComponent<LineRenderer>();
            lineRens = new LineRenderer[] {lineren1, lineren2, lineren3, lineren4};
        
        for (int i = 0; i < lineRens.Length; i++)
        {
            
            lineRens[i].SetVertexCount(2);
            lineRens[i].material.color = Color.black;
            lineRens[i].SetWidth(.05f, .05f);
            if (i == lineRensPoints.Length - 1)
            {
                lineRens[i].SetPosition(0, lineRensPoints[i]);
                lineRens[i].SetPosition(1, lineRensPoints[0]);
                
            }
            else
            {
                lineRens[i].SetPosition(0, lineRensPoints[i]);
                lineRens[i].SetPosition(1, lineRensPoints[i + 1]);
              
            }
        }
        Color temp = new Color();
        temp.a = .1f;
        gameObject.renderer.material.color= temp;

        
       // foreach (Vector3 printVec in gameObjcorners) print("Corner is: (" + printVec.x + ", " + printVec.y + ", " + printVec.z + ")"); // for debugging
    }
    

	 void Start () {

         print("In start");
		//rightHand = new BluetoothScript();
		//print (rightHand);
         HandController controller = GetComponent<HandController>();
		sphereRatio = 1f / ( PlayerPrefs.GetInt("Difficulty") *2.5f );   // scaling purposes
         modeAry = new Phase[System.Enum.GetValues(typeof(phaseTypes)).Length];
         setPatternBool();
         setTimedBool();
        
        for (int i = 0; i < modeAry.Length  ; i++)
        {
            if (timed & i != 0) modeAry[i] = new Phase(i, true, testTime, studyTime, searchTime, restTime);
            else modeAry[i] = new Phase(i, false);
        }
         
         //Use with Kevin's Kinect scripts
        /*
        _BodySourceView = BodySourceView.GetComponent<BodySourceView>();
        shoulderHeight = _BodySourceView.getShoulderHeight();
        shoulderWidth = _BodySourceView.getShoulderWidth();
        armLength = _BodySourceView.getArmLength();
         */

        numSpheres = PlayerPrefs.GetInt("Number of Stimuli");
        markLocations = new Vector3[numSpheres];

        // Array to hold all stimuli
         
        
         findSpheres = new GameObject[numSpheres];

       
        if (!randomPattern) PatternSpheres();
        else GenerateSpheres();

      //  GameObject.Find("Logger").GetComponent<Logger>().BeginLogging();
        phaseInit = true;

       // UnitySerialPortRightHand = this.GetComponent("UnitySerialPort") as UnitySerialPort;
        
     }
	
	// Update is called once per frame
	void Update () {

        if (phase == 0)
        {

     /*  OccViewObjLeft.camera.enabled = false;
            OccViewObjRight.camera.enabled = false;
            MainViewObj.camera.enabled = true; */
         

        }
       

        if (phase == 1 )
        {
    /*    OccViewObjLeft.camera.enabled = true;
            OccViewObjRight.camera.enabled = true;
            MainViewObj.camera.enabled = false; */

            GameObject tempCam = GameObject.Find("CameraDepthView");
            print
            tempCam.camera.backgroundColor = Color.Lerp(Color.red, Color.green, activeStimScript.getIntensityFloat(GameObject.Find("RigidHandLeft(Clone)").transform.position));
            
            
            if (phaseInit)
            {
                
               if(timed) tStart = Time.time;
                phaseInit = false;
                activeSphere = 0;
                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
                print(activeStimScript.name);
                activeStimScript.setThisActive();

            }

            /*UnitySerialPortRightHand.thumbTip = activeStimScript.getIntensity(GameObject.Find("palmR").transform.position);
            UnitySerialPortRightHand.indexTip = activeStimScript.getIntensity(GameObject.Find("palmR").transform.position);
            UnitySerialPortRightHand.middleTip = activeStimScript.getIntensity(GameObject.Find("palmR").transform.position);
            UnitySerialPortRightHand.ringTip = activeStimScript.getIntensity(GameObject.Find("palmR").transform.position);
            UnitySerialPortRightHand.pinkyTip = activeStimScript.getIntensity(GameObject.Find("palmR").transform.position);
            print(activeStimScript.getIntensity(GameObject.Find("palmR").transform.position));*/

            if (!timed && activeStimScript.getFound() && !phaseChangeFlag())
            {
                //print("ActiveSphere is: " + active); /* For debugging */
                //print("I'm here"); /* For debugging */
                
                activeSphere++;
                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
                activeStimScript.setThisActive();
            }
            
            
            if (Time.time - tStart < searchTime && timed)
            {
                
                if (activeStimScript.getFound() && !phaseChangeFlag())
                {
                    print("ActiveSphere is: " + active);
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
                
                statusMssg = "TRIAL COMPLETED; Now Entering Study Phase";       
                phase = 2;
                
                    }
            else if (timed && Time.time - tStart > searchTime && !phaseChangeFlag())
            {
                phase = 0;
                statusMssg = " Time expired; Click to reset";
                phaseInit = true;
            }

                    }
        if (phase == 2)
        {
            if (phaseInit)
            {
                
                tStart = Time.time;
                phaseInit = false;
               

            }

            
            if(!timeExpired) studyTimeLeft = (studyTime - Time.time).ToString();
           

            if (Time.time - tStart > studyTime)
            {
                timeExpired = true;
                studyTimeLeft = (0).ToString();
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
                statusMssg = "Study Phase Complete; Click Start for next trial"; 
            }
        }
        

        if (phase == 3)
        {
            grabD = handControlScript.getGrabStrength();
            if (grabCount == numSpheres) showLocations();
            
             
               
           else if (grabD.grabLevel > .85 && !grabbingFlag)
            {
                tStart = Time.time;
                grabbingFlag = true;
                string side = (grabD.isItLeft) ? "Left" : "Right";
                print(side + " hand is grabbing");
                    
            }
            else if (grabbingFlag)
            {
                 
                    if( (Time.time - tStart > grabThreshold) && !(grabCount == numSpheres)){

                        string side = (grabD.isItLeft) ? "Left" : "Right";
                        markLocations[grabCount] = new Vector3(grabD.position.x, grabD.position.y, grabD.position.z);   //Do we want subjects to start with the first object or the second?
                        grabCount++;
                        print("Grab by " + side + "Hand recorded" + " at Pos: (" + grabD.position.x + ", " + grabD.position.y + "," + grabD.position.z + ")" + "\n" );
                        print("Number recorded = " + grabCount + "; " + (numSpheres - grabCount) + " left to record" );
                        grabbingFlag = false;
                        for(int i = 0; i < 5000; i++){
                            // provide wait period between grabs 
                        }
                        
                    }
                    else if (grabD.grabLevel < .5)
                    {
                        grabbingFlag = false;
                        print("No longer grabbing");
                    }
            }
        }
	}

    void showLocations()
    {
        foreach (GameObject sphere in findSpheres) sphere.renderer.enabled = true;
        foreach (Vector3 mark in markLocations)
        {
            
           GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
           sphere.transform.position = new Vector3(mark.x, mark.y, mark.z);
           sphere.transform.localScale = findSpheres[0].transform.localScale;

        }
        phase = 4;
    }


    
    //randomly generate # of spheres contained within this gameobject
    void GenerateSpheres( bool randFlag = false )
    {
        int j = Mathf.RoundToInt(Random.Range(0, randShapeCorners.Length - 1));
        
		// creates # of desired spheres, sets their color to yellow, calls isFound method to hide spheres
		for (int i = 0; i < numSpheres; i++) {
            
            findSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            findSpheres[i].layer = 8;
            findSpheres[i].renderer.enabled = false;
            findSpheres[i].collider.isTrigger = true;
            findSpheres[i].AddComponent("Rigidbody");
            findSpheres[i].rigidbody.useGravity = false;
            findSpheres[i].rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            findSpheres[i].renderer.material.color = Color.green;
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
            else{
            // randomly place spheres, position bounded by searchSpace extents
            
			findSpheres [i].transform.localPosition = new Vector3 ( Random.Range(gameObject.transform.position.x-gameObject.transform.localScale.x/2, 
                gameObject.transform.position.x + gameObject.transform.localScale.x/2), 
                Random.Range(gameObject.transform.position.y-gameObject.transform.localScale.y/2, gameObject.transform.position.y + gameObject.transform.localScale.y/2),
                Random.Range(gameObject.transform.position.z-gameObject.transform.localScale.z/2, gameObject.transform.position.z + gameObject.transform.localScale.z/2));
            }
            findSpheres[i].transform.localScale = gameObject.transform.localScale * sphereRatio;
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

            randShape.transform.localScale = new Vector3(Random.Range((gameObject.transform.localScale.x *sphereRatio)*2, gameObject.transform.localScale.x ),
                    Random.Range((gameObject.transform.localScale.y*sphereRatio)*2, gameObject.transform.localScale.y ), Random.Range((gameObject.transform.localScale.z *sphereRatio)*2, gameObject.transform.localScale.z));

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
        int temp = PlayerPrefs.GetInt("Pattern");
        randomPattern = (temp == 0) ? true : false;
    }

    void setTimedBool()
    {
        int temp = PlayerPrefs.GetInt("Timed");
        timed = (temp == 1) ? true : false;
    }

    void setPhase()
    {
       phase = PlayerPrefs.GetInt("Phase");
        
    }


    private string timeLeft = "";
    void OnGUI()
    {


        GUI.backgroundColor = Color.black;
        GUI.color = Color.white;

        GUI.Label(new Rect(0, 0, 350, 20), studyTimeLeft);
        GUI.Label(new Rect(0, 25, 350, 20), statusMssg);
        
        GUI.Label(new Rect(0 , 0 , 100, 90), message);
       
        if (GUI.Button(new Rect(0, 100, 100, 20), "Reset")) reset();
		if (GUI.Button(new Rect(0, 400, 100, 20), "RandPoint")) testRandPoint();
        if (GUI.Button(new Rect(150, 60 , 100, 20), "TestSimFound"))
        {
            blockColTest(startClick);
            startClick++;
        }
        if (GUI.Button(new Rect(0, 140, 100, 20), "Back")) Application.LoadLevel("TreasureHuntLoader");
      
        if (timed)
        {
            GUI.Label(new Rect(130, 0, 100, 90), timeLeft);
            message = PlayerPrefs.GetString("Mode") + " Mode";
        }
            bool startHit = GUI.Button(new Rect(0, 60, 100, 20), "Start");
            if (startHit && ! (phase ==2))
            {
                
                phase = 1;
                phaseInit = true;
                statusMssg = "Trial in Progress";
                handControlScript.setTrialBool(true, 1);
               
                
            }
            else if (startHit && phase == 2)
            {
                phase = 3;
                phaseInit = true;
                statusMssg = "Backward Trial in Progress";
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

        
        List<GameObject> delList= new List<GameObject>(); 
        delList.AddRange( GameObject.FindGameObjectsWithTag("Unfound"));
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
   
    private void blockColTest(int clickCount){   //for testing
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
            else if (phase == 3)
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

	private void testRandPoint(){   //for testing
				rand = new Vector3 (Random.Range (gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
		               gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
	                   Random.Range (gameObject.transform.position.y - gameObject.transform.localScale.y / 2, 
		                gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
		                Random.Range (gameObject.transform.position.z - gameObject.transform.localScale.z / 2, 
		                gameObject.transform.position.z + gameObject.transform.localScale.z / 2));
                        print("Byte sent is: " + activeStimScript.getIntensityByte(rand));


		

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

    public float getMaxDist(){

        float maxDist = 0;
        foreach(Vector3 compare in gameObjcorners){
            float temp = Vector3.Distance(gameObjcorners[0], compare);
            if (temp > maxDist) maxDist = temp;
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
        //foreach (Vector3 vec in shapeCorners) print("(" + vec.x + ", " + vec.y + ", " + vec.z + ")");  //Debugging

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

    

	}
