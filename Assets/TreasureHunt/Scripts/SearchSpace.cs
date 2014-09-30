//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;


//public struct Phase{
//    int num;
//    string pattern;
//    bool timed;
//    float length , wait , study , rest , completedTrials ;
  

//    public Phase(int num, bool timed)
//    {
//        this.num = num;
//        this.timed = timed;
//        pattern = "";
//        length =  wait = study = rest = completedTrials = 0;
//    }


//    public Phase(int num,  bool timed, float length, float wait, float study, float rest)
//    {
//        this.num = num;
//        pattern = "";
//        this.timed = timed;
//        this.length = length;
//        this.wait = wait;
//        this.study = study;
//        this.rest = rest;
//        completedTrials = 0;
//    }

    
//};


//public class SearchSpace : MonoBehaviour {
    
//    // Access Kinect data to make bounds for search space   
//    /* public GameObject BodySourceView;
//    private BodySourceView _BodySourceView;
//    private float shoulderHeight;
//    private float shoulderWidth;
//    private float armLength;*/

//    /* private float shoulderHeight = 4.396132f;
//    private float shoulderWidth = Vector3.Distance(new Vector3(-1.540944f, 4.396132f, 15.13844f), new Vector3( 2.052144f, 4.249248f, 15.0819f));
//    private float armLength = Vector3.Distance(new Vector3(-1.540944f, 4.396132f, 15.13844f), new Vector3(-2.761098f, 2.57422f, 14.22677f)) + Vector3.Distance(new Vector3(-2.761098f, 2.57422f, 14.22677f), new Vector3(-1.349743f, 1.365262f, 11.68927f));*/
	
    
//    /* Use this for initialization, 3 parameters received on Start, shoulder height, shoulder distance, and extended arm length
//         Parameters to be used to position and scale searchspace*/

//    private enum phaseTypes { Load, SearchBck, Wait, SearchFwd };
//    private enum modes { Practice, TimedFwd, TimedBckwd, UTimedFwd, UTimedBckwd };

//    private int numSpheres;
//    [SerializeField] private int activeSphere;
//    GameObject[] findSpheres;
//    float sphereRatio;
//    StimulusScript myStimScript;
//    StimulusScript activeStimScript;
//    Phase[] modeAry;
//    private bool randomPattern;
//    private bool timed;
//    private string message = "";
//    private int numFound;
//    private int startClick = 0;
//    private int mode;
//    private string statusMssg = "";
//    private Vector3[] corners = new Vector3[4];
//    private Vector3 rand;
//    private BluetoothScript rightHand;
    

   
//    //Trial variables
//    private int phase = 0;
//    private float phaseWaitTime = 10; //in s
//    private float phaseWaitTimeStart;
//    private bool phaseInit;
//    private float testTime = 5000f;
//    private float searchTime = 100000f; //in s
//    private float restTime = 3.000f; //in s
//    private float tStart;
//    private int numberOfCompletedTrials = 0;
//    private int expectedNumberOfTrials;

//    void Awake()
//    {
        
//        Color tempcolor = gameObject.renderer.material.color;
//        tempcolor.a = .2f; //0 is invis, 1 is fully visible
//        gameObject.renderer.material.color = tempcolor;
//    }
    

//     void Start () {
         
//        rightHand = new BluetoothScript();
//        print (rightHand);
//        sphereRatio = 1f / ( PlayerPrefs.GetInt("Difficulty") * 5);   // scaling purposes
//         modeAry = new Phase[System.Enum.GetValues(typeof(phaseTypes)).Length];
//         setPatternBool();
//         setTimedBool();
        
//        for (int i = 0; i < modeAry.Length  ; i++)
//        {
//            if (timed & i != 0) modeAry[i] = new Phase(i, true, testTime, phaseWaitTime, searchTime, restTime);
//            else modeAry[i] = new Phase(i, false);
//        }
         
//         //Use with Kevin's Kinect scripts
//        /*
//        _BodySourceView = BodySourceView.GetComponent<BodySourceView>();
//        shoulderHeight = _BodySourceView.getShoulderHeight();
//        shoulderWidth = _BodySourceView.getShoulderWidth();
//        armLength = _BodySourceView.getArmLength();
//         */

//        numSpheres = PlayerPrefs.GetInt("Number of Stimuli");

//        // Array to hold all stimuli
         
        
//         findSpheres = new GameObject[numSpheres];


//        if (!randomPattern) PatternSpheres(numSpheres);
//        else RandomSpheres(numSpheres);

//        GameObject.Find("Logger").GetComponent<Logger>().BeginLogging();
//        phaseInit = true;
         

        
//     }
	
//    // Update is called once per frame
//    void Update () {

       
       

//        if (phase == 1 )
//        {
            
//            if (phaseInit)
//            {
                
//               if(timed) tStart = Time.time;
//                phaseInit = false;
//                activeSphere = 0;
//                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
//                activeStimScript.toggleActive();

//            }
            
            
//            if ((Time.time - tStart < searchTime && timed) )
//            {
                
//                if (activeStimScript.getFound() && !phaseChangeFlag())
//                {
//                    print("ActiveSphere is: " + active);
//                    //print("I'm here"); /* For debugging */
//                    activeStimScript.toggleActive();
//                    activeSphere++;
//                    activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
//                    activeStimScript.toggleActive();
//                }
//                float t = (searchTime - (Time.time - tStart));


//                if (t < .02f) timeLeft = 0 + "";
//                else timeLeft = t + "";

//            }

//            if (phaseChangeFlag())
//            {
                
//                statusMssg = "TRIAL COMPLETED; Please Wait";       
//                phase = 2;
                
//                    }
//            else if (timed && Time.time - tStart > searchTime && !phaseChangeFlag())
//            {
//                phase = 0;
//                statusMssg = " Time expired; Click to reset";
//                phaseInit = true;
//            }

//                    }
//        if (phase == 2)
//        {
//            if (phaseInit)
//            {
                
//                tStart = Time.time;
//                phaseInit = false;
               

//            }


//            if (Time.time - tStart > phaseWaitTime)
//            {

//                numFound = 0;
//                startClick = 0;
//                phaseInit = true;
//                foreach (GameObject sphere in findSpheres)
//                {
//                    activeStimScript = sphere.GetComponent("StimulusScript") as StimulusScript;
//                    activeStimScript.hide();
//                }
//                List<GameObject> delList = new List<GameObject>();
//                delList.AddRange(GameObject.FindGameObjectsWithTag("TestCube"));
//                foreach (GameObject delObj in delList) Destroy(delObj);
//                statusMssg = "Click Start for next trial"; 
//            }
//        }
        

//        if (phase == 3)
//        {

            
//            if (phaseInit)
//            {
                
//                if (timed) tStart = Time.time;
//                phaseInit = false;
//                activeSphere = findSpheres.Length - 1;
//                activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
//                activeStimScript.toggleActive();

//            }
//            else
//            {
//                activeSphere = findSpheres.Length - numFound;
//            }

//            if (Time.time - tStart < searchTime && timed)
//            {
               
//                if (activeStimScript.getFound() && !phaseChangeFlag())
//                {
//                   // print("Numfound is: " + numFound);
//                    //print("I'm here"); /* For debugging */
//                    activeStimScript.toggleActive();
//                    activeSphere--;
//                    activeStimScript = findSpheres[activeSphere].GetComponent("StimulusScript") as StimulusScript;
//                    activeStimScript.toggleActive();
//                }
//                float t = (searchTime - (Time.time - tStart));


//                if (t < .02f) timeLeft = 0 + "";
//                else timeLeft = t + "";

//            }

//            if (phaseChangeFlag())
//            {

//                statusMssg = "Backward Trial Complete; Click to reset or exit";
//                phase = 2;

//            }
//            else if (timed && Time.time - tStart > searchTime && !phaseChangeFlag())
//            {
//                phase = 0;
//                message = "Time Expired";
//                statusMssg = "Click to reset";
//                phaseInit = true;
//            }
           
//        }

            
	
//    }


    
//    //randomly generate # of spheres contained within this gameobject
//    void RandomSpheres(int num )
//    {
		
		
//        // creates # of desired spheres, sets their color to yellow, calls isFound method to hide spheres
//        for (int i = 0; i < num; i++) {
//            findSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//            findSpheres[i].layer = 8;
//            findSpheres[i].renderer.enabled = false;
//            findSpheres[i].collider.isTrigger = true;
//            findSpheres[i].AddComponent("Rigidbody");
//            findSpheres[i].rigidbody.useGravity = false;
//            findSpheres[i].rigidbody.constraints = RigidbodyConstraints.FreezeAll;
//            findSpheres[i].renderer.material.color = Color.green;
//            findSpheres[i].tag = "Unfound";
//            findSpheres[i].AddComponent("StimulusScript");
//            myStimScript = findSpheres[i].GetComponent("StimulusScript") as StimulusScript;
            

//            // randomly place spheres, position bounded by searchSpace extents
//            findSpheres[i].transform.localScale = gameObject.transform.localScale*sphereRatio;
//            findSpheres [i].transform.localPosition = new Vector3 ( Random.Range(gameObject.transform.position.x-gameObject.transform.localScale.x/2, 
//                gameObject.transform.position.x + gameObject.transform.localScale.x/2), 
//                Random.Range(gameObject.transform.position.y-gameObject.transform.localScale.y/2, gameObject.transform.position.y + gameObject.transform.localScale.y/2),
//                Random.Range(gameObject.transform.position.z-gameObject.transform.localScale.z/2, gameObject.transform.position.z + gameObject.transform.localScale.z/2));
//            } 
//        //Do we need to worry about overlap of stimuli?
// }

//    //generate spheres in a polygonal pattern; polygon determined by number of vertices/sides (int sides) parameter
//    void PatternSpheres(int num)
//    {
        
//       /* float intAngle = -1; // -1 will represent that a circular pattern is desired
//        Vector3 shapeCenter = gameObject.transform.position;  // space center is polygon center
//        // first vertex point half way between center and maximum extents of search space
//        Vector3 firstVertex = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y / 4, gameObject.transform.position.z);
//        if (sides != 0)  intAngle = (sides - 2) * 180 / sides;
//        float trajAngle = 180 - intAngle / 2;
//        // make side length 1/4 the length of the overal scale ....this shouldn't fall outside bounds, but might for polygons with higher number of vertices
//        float sideLength = (.5f) * gameObject.transform.localScale.x / Mathf.Cos(((Mathf.Deg2Rad * 180) - (Mathf.Deg2Rad * intAngle)) / 2);
//        for (int i = 0; i < sides; i++)
//        {

//        }*/

//         bool outOfBounds = false;
//         int [] ones = new int[]{1, 0, 0, 1};
//        GameObject randShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        do{
           
//        randShape.transform.position = new Vector3(Random.Range(gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
//                gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
//                Random.Range(gameObject.transform.position.y - gameObject.transform.localScale.y / 2, gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
//                Random.Range(gameObject.transform.position.z - gameObject.transform.localScale.z / 2, gameObject.transform.position.z + gameObject.transform.localScale.z / 2));
        
//        randShape.transform.localScale =  new Vector3 ( Random.Range(0, gameObject.transform.localScale.x / 4), 
//                Random.Range(0, gameObject.transform.localScale.y / 4), Random.Range( 0, gameObject.transform.localScale.z / 4));

//            for( int i = 0; i < 4; i++){
//                corners[i] = new Vector3(randShape.transform.position.x - randShape.transform.localScale.x / 2, randShape.transform.position.y 
//                    - Mathf.Pow(-1, i) * randShape.transform.localScale.y / 2, randShape.transform.position.z - Mathf.Pow(-1, ones[i]  ));

//                if (!gameObject.renderer.bounds.Contains(corners[i])) outOfBounds = true;
//            }

//        }
//        while( !outOfBounds);
        



//    }

//    bool phaseChangeFlag()
//    {
//        //print("In PhaseChangeFlag");
//        return numFound == findSpheres.Length;
//    }
//    void setPatternBool()
//    {
//        int temp = PlayerPrefs.GetInt("Pattern");
//        randomPattern = (temp == 0) ? true : false;
//    }

//    void setTimedBool()
//    {
//        int temp = PlayerPrefs.GetInt("Timed");
//        timed = (temp == 1) ? true : false;
//    }

//    void setPhase()
//    {
//       phase = PlayerPrefs.GetInt("Phase");
        
//    }


//    private string timeLeft = "";
//    void OnGUI()
//    {
        
//        GUI.Label(new Rect(0, 25, 350, 20), statusMssg);
//        GUI.Label(new Rect(0 , 0 , 100, 90), message);
//        if (GUI.Button(new Rect(0, 100, 100, 20), "Reset")) reset();
//        if (GUI.Button(new Rect(0, 400, 100, 20), "RandPoint")) testRandPoint();
//        if (GUI.Button(new Rect(150, 60 , 100, 20), "TestSimFound"))
//        {
//            blockColTest(startClick);
//            startClick++;
//        }
//        if (GUI.Button(new Rect(0, 140, 100, 20), "Back")) Application.LoadLevel("TreasureHuntLoader");
      
//        if (timed)
//        {
//            GUI.Label(new Rect(130, 0, 100, 90), timeLeft);
//            message = PlayerPrefs.GetString("Mode") + " Mode";
//        }
//            bool startHit = GUI.Button(new Rect(0, 60, 100, 20), "Start");
//            if (startHit && ! (phase ==2))
//            {
                
//                phase = 1;
//                phaseInit = true;
//                statusMssg = "Trial in Progress";
                
//            }
//            else if (startHit && phase == 2)
//            {
//                phase = 3;
//                phaseInit = true;
//                statusMssg = "Backward Trial in Progress";
//            }

            
            

        
//    }


//    void reset()
//    {
//        List<GameObject> delList= new List<GameObject>(); 
//        delList.AddRange( GameObject.FindGameObjectsWithTag("Unfound"));
//        delList.AddRange(GameObject.FindGameObjectsWithTag("Found"));
//        foreach (GameObject delSphere in delList) Destroy(delSphere);
//        numFound = startClick = 0;
//        timeLeft = searchTime.ToString();
//        statusMssg = "Trial reset!";
//        phase = 0;
//        Start();
//    }

//    public int getNumFound()
//    {
//        return numFound;
//    }

//    public void incNumFound()
//    {
//        numFound++;
//    }
   
//    private void blockColTest(int clickCount){
//        try
//        {
//            if (phase == 1)
//            {
//                Vector3 pos = findSpheres[clickCount].transform.position;
//                GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                testCube.tag = "TestCube";
//                testCube.AddComponent("Rigidbody");
//                testCube.rigidbody.useGravity = false;
//                testCube.renderer.enabled = false;
//                testCube.transform.position = pos;
//            }
//            else if (phase == 3)
//            {
                    
//                Vector3 pos = findSpheres[findSpheres.Length - clickCount - 1].transform.position;
//                GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                testCube.tag = "TestCube";
//                testCube.AddComponent("Rigidbody");
//                testCube.rigidbody.useGravity = false;
//                testCube.renderer.enabled = false;
//                testCube.transform.position = pos;
//            }
//        }
//        catch (System.IndexOutOfRangeException) { };
//    }

//    private void testRandPoint(){
//                rand = new Vector3 (Random.Range (gameObject.transform.position.x - gameObject.transform.localScale.x / 2,
//                       gameObject.transform.position.x + gameObject.transform.localScale.x / 2),
//                       Random.Range (gameObject.transform.position.y - gameObject.transform.localScale.y / 2, 
//                        gameObject.transform.position.y + gameObject.transform.localScale.y / 2),
//                        Random.Range (gameObject.transform.position.z - gameObject.transform.localScale.z / 2, 
//                        gameObject.transform.position.z + gameObject.transform.localScale.z / 2));
				
//        int dist = (int) ( Mathf.Round( activeStimScript.getDistance (rand)));
//        print ("Distance is: " + dist);

//        switch(dist)
//        {
//        case 1:
//            rightHand.vibrate("rightHand", "rightThumb",155);
//            break;
//        case 2:
//            rightHand.vibrate("rightHand", "rightThumb",175);
//            break;
//        case 3:
//            rightHand.vibrate("rightHand", "rightThumb",195);
//            break;
//        case 4:
//            rightHand.vibrate("rightHand", "rightThumb",215);
//            break;
//        case 5:
//            rightHand.vibrate("rightHand", "rightThumb",235);
//            break;
//        case 6:
//            rightHand.vibrate("rightHand", "rightThumb",255);
//            break;
//        }

//        }
//    }
