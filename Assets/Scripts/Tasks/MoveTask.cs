using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;

// ??? 64-bit builds aren't working any more. Apparently this is the reason why
// http://forum.unity3d.com/threads/fallback-handler-could-not-load-library-unity-4-2-osx-10-8-4-standalone.191966/#post-1493495

// TO DO
// 2) The area for generating stimuli shoulnd't be a rectangle. it should be a pyramid extending out from the handcontroller with a rectangle on top of it OR if we can access the Leap InteractionBox, we could probably use that, but I couldn't figure it out.
// When really close to the surface of the table (small y), hand tracking is great when right above the hand tracker but terrible when out of that cone.
// 4) Animate reset button press (frivolous)

public class MoveTask : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject button;
    public GameObject blackboard;
    public Slider feedbackSlider;
    public UnityEngine.UI.Image highAccPanel;
    public UnityEngine.UI.Image lowAccPanel;
    public float studyTransparency = 0.925f;
    public int ITI;
    public GameObject oculusCamera;
    public GameObject regularCamera;
    private int totalTrials;
    public float popRate;
    public float studyTime = 3f;
    public Vector3 stimLocationSize;
    public Vector3 stimLocationOffset;
    public Vector3 stimOverlapBuffer;
    public Vector3 stimResetSize;
    public Vector3 stimResetOffset;
    private GameObject[] stimuli;
    private GameObject[] transparentStimuli;
    private GameObject practiceCube;
    private Transform handController;
    private bool practiceCubeGenerated;
    private int preallocatedStimLocationArraySizes;
    private float timeLeft;
    private int[] colorNums;
    private Vector3[] stimLocations;
    private Vector3[] resetLocations;
    private int currentTrial;
    private int newDiff;
    private int diff;
    private bool enableLogging;
    private Logger logger;
    private List<Texture> colors;
    private float grow;
    private float timer;
    private bool handsReset;


    enum State { PracticeStart, Practice, PracticeWaitToDestroyHands, PracticeWaitForNewHands, PracticeGiveInstructions, PracticeEnd, NoPractice, TrialStart, OnStudy, Study, OnRecall, Recall, RecallAllObjectsMoved, RecallFeedback, RecallEnd, TrialEnd, InterTrialInterval, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        //Grabs player preferences
        //Practice
        if (PlayerPrefs.GetInt("PracticeYesNo") == 1)
            currentState = State.PracticeStart;
        else
        {
            currentState = State.NoPractice;
            handsReset = false;
        }
            // Oculus Camera
        if (PlayerPrefs.GetInt("OculusCamYesNo") == 0)
        {
            oculusCamera.SetActive(false);
            regularCamera.SetActive(true);
        }
        else
        {
            oculusCamera.SetActive(true);
            regularCamera.SetActive(false);
        }

        //Starting Difficulty
        diff = PlayerPrefs.GetInt("StartingDifficulty");
        newDiff = diff;

        //Session length
        totalTrials = PlayerPrefs.GetInt("TrialLimit");

        //Logging
        enableLogging = PlayerPrefs.GetInt("LogDataYesNo") == 0 ? false : true;
        preallocatedStimLocationArraySizes = 10;
        handController = GameObject.Find("HandController").transform;
        stimLocations = new Vector3[preallocatedStimLocationArraySizes];
        resetLocations = new Vector3[preallocatedStimLocationArraySizes];
        stimuli = new GameObject[preallocatedStimLocationArraySizes];
        transparentStimuli = new GameObject[preallocatedStimLocationArraySizes];
        currentTrial = 0;
        if (enableLogging) logger = GameObject.Find("Logger").GetComponent<Logger>();
        if (enableLogging) logger.BeginLogging();
        practiceCubeGenerated = false;
        colors = GenerateColors();
        colorNums = new int[colors.Count];
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Press Esc to quit
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //Debug.Log(currentState);
        switch (currentState)
        {

            case State.PracticeStart:
                if (!practiceCubeGenerated)
                    GeneratePracticeCube();
                else
                {
                    blackboard.GetComponentInChildren<Text>().text = "Welcome to the Holodeck\n\nThese are your practice hands\nThey turn transparent as they go out of range\n\n Press the button to continue";
                    bool practicePopInComplete = PopIn(practiceCube);
                    if (practicePopInComplete)
                        currentState = State.Practice;
                }
                break;


            case State.Practice:
                //Grabs the fingerTouch variable from the DetectTouch script, which has been placed on a smaller sphere collider in the middle of the EndTrail sphere
                if (!button.GetComponentInChildren<ButtonManager>().GetButtonState())
                {
                    TurnOffbutton();
                    blackboard.GetComponentInChildren<Text>().text = "Grab the cube with two fingers\n\nIt's best to grab it like a shot glass\n\nMove your hands above your head to continue";
                    currentState = State.PracticeWaitToDestroyHands;
                }
                break;

            case State.PracticeWaitToDestroyHands:
                //If the hands have been removed from the scene
                if (GameObject.Find("RigidFullHand(Clone)") == null)
                {
                    blackboard.GetComponentInChildren<Text>().text = "Ok now bring them back in front of you";
                    DisableTransparency();
                    currentState = State.PracticeWaitForNewHands;
                }
                break;

            case State.PracticeWaitForNewHands:
                if (GameObject.Find("RigidFullHand(Clone)") != null)
                {
                    TurnOnbutton();
                    blackboard.GetComponentInChildren<Text>().text = "These are your real virtual hands\nWay nicer\n\nHit the button again to get started";
                    currentState = State.PracticeGiveInstructions;
                }
                break;

            case State.PracticeGiveInstructions:
                if (!button.GetComponentInChildren<ButtonManager>().GetButtonState())
                    {
                        TurnOffbutton();
                        currentState = State.PracticeEnd;
                        blackboard.GetComponentInChildren<Text>().text = "Soon you will see some cubes appear\n\n Your job is to remember their locations";
                        timeLeft = 3f;
                    }
                    break;

            case State.PracticeEnd:
                timeLeft -= Time.deltaTime;
                if (practiceCube != null)
                {
                    bool practicePopOutComplete = PopOut(practiceCube);
                    if (practicePopOutComplete)
                        Destroy(practiceCube);
                }
                if (timeLeft < 0)
                {
                    blackboard.GetComponentInChildren<Text>().text = "";
                    currentState = State.TrialStart;
                }
                break;
                
            case State.NoPractice:
                // Disables hand transparency
                if (!handsReset)
                {
                    DisableTransparency();
                    handsReset = true;
                }
                blackboard.GetComponentInChildren<Text>().text = "Welcome back! \n\n\n Press the button to begin";
                //For some reason these were activating at the start of the scene. 
                  feedbackSlider.gameObject.SetActive(false);
                    lowAccPanel.gameObject.SetActive(false);
                    highAccPanel.gameObject.SetActive(false);
                if (!button.GetComponentInChildren<ButtonManager>().GetButtonState())
                {
                    TurnOffbutton();
                    blackboard.GetComponentInChildren<Text>().text = "";
                    currentState = State.TrialStart;
                }
                break;

            case State.TrialStart:
                currentTrial += 1;
                GenerateStimLocations();
                GenerateStimuli();
                if (enableLogging) logger.RegenerateStimuliInSameFile();
                currentState = State.OnStudy;
                break;

            case State.OnStudy:
                if (currentTrial == 2 || currentTrial == 3)
                    blackboard.GetComponentInChildren<Text>().text = "Getting a new perspective can be useful\n\nMove your head around for a better look";
                bool popInComplete = PopIn(transparentStimuli);
                if (popInComplete)
                {
                    timeLeft = studyTime;
                    currentState = State.Study;
                }
                break;

            case State.Study:
                timeLeft -= Time.deltaTime;
                bool popOutComplete = false;
                if (timeLeft <= 0)
                {
                    popOutComplete = PopOut(transparentStimuli);
                    if (popOutComplete)
                    {
                        MakeInactive(transparentStimuli);
                        ResetStimuli();
                        currentState = State.OnRecall;
                    }
                }
                break;

            case State.OnRecall:
                if (currentTrial <= 3)
                    blackboard.GetComponentInChildren<Text>().text = "Move the cubes back to where they were";
                popInComplete = PopIn(stimuli);
                if (popInComplete)
                    currentState = State.Recall;
                break;

            case State.Recall:
                int movedCubes = GetNumberOfMovedCubes();
                if (movedCubes >= diff)
                {
                    TurnOnbutton();
                    currentState = State.RecallAllObjectsMoved;
                }

                break;

            case State.RecallAllObjectsMoved:
                if (currentTrial <= 3)
                    blackboard.GetComponentInChildren<Text>().text = "Hit the button to see how you did";
                if (!button.GetComponentInChildren<ButtonManager>().GetButtonState())
                {
                    TurnOffbutton();
                    PreparePracticeStimuli();
                    currentState = State.RecallFeedback;
                    feedbackSlider.value = 0;
                    feedbackSlider.gameObject.SetActive(true);
                }
                break;

            case State.RecallFeedback:
                if (currentTrial <= 3)
                    blackboard.GetComponentInChildren<Text>().text = "";
                bool doneFeedback = StimulusPositionFeedback();
                if (doneFeedback)
                {
                    if (currentTrial == 1) 
                        blackboard.GetComponentInChildren<Text>().text = "Not bad! \n\n Let's try again";
                    if (currentTrial == 2)
                        blackboard.GetComponentInChildren<Text>().text = "Getting better! \n\n Let's practice one more time";
                    if (currentTrial == 3)
                        blackboard.GetComponentInChildren<Text>().text = "Looks like you've got the hang of it \n\n Now let's do it for real \n\n You'll do " + (totalTrials) + " trials";
                    if (enableLogging)
                        logger.FinishTrial(currentTrial, diff);
                    timeLeft = 2f;
                    currentState = State.RecallEnd;
                    newDiff = AdaptDifficulty(feedbackSlider.value);
                    if (newDiff != diff)
                        Debug.Log("Changing diff from " + diff + " to " + newDiff);
                    else
                        Debug.Log("The difficulty will stay at " + newDiff);
 
                }
                break;

            case State.RecallEnd:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    feedbackSlider.value = 0;
                    feedbackSlider.gameObject.SetActive(false);
                    lowAccPanel.gameObject.SetActive(false);
                    highAccPanel.gameObject.SetActive(false);
                    if (currentTrial <= 4)
                        blackboard.GetComponentInChildren<Text>().text = "";
                    if (currentTrial >= totalTrials)
                    {
                        currentState = State.TaskEnd;
                        if (enableLogging) logger.FinishTrial(currentTrial, diff);
                        if (enableLogging) logger.Finish();
                    }
                    else
                        currentState = State.TrialEnd;
                }
                break;

            case State.TrialEnd:
                popOutComplete = PopOut(stimuli);
                PopOut(transparentStimuli);
                if (popOutComplete)
                {
                    DestroyStimuli();
                    stimuli = new GameObject[preallocatedStimLocationArraySizes];
                    transparentStimuli = new GameObject[preallocatedStimLocationArraySizes];
                    diff = newDiff;
                    currentState = State.InterTrialInterval;
                    timeLeft = ITI;
                }
                break;

            case State.InterTrialInterval:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                    currentState = State.TrialStart;
                break;

            case State.TaskEnd:
                DestroyStimuli();
                blackboard.GetComponentInChildren<Text>().text = "All done!";
                break;
        }
    }

    List<Texture> GenerateColors()
    {
        List<Texture> colors = new List<Texture>();
        colors.Add(Resources.Load("Red", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Orange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Yellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Green", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Blue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Violet", typeof(Texture2D)) as Texture);
        return colors;

    }
    void GeneratePracticeCube()
    {
        //Generates a random integer that we'll use to pick a color
        int practiceCubeColor = Random.Range(0, colors.Count - 1);
        practiceCube = (GameObject)Instantiate(cubePrefab);
        practiceCube.transform.position = handController.transform.position + stimLocationOffset;
        practiceCube.transform.rotation = Random.rotation;
        practiceCube.transform.renderer.material = new Material(Shader.Find("Diffuse"));
        practiceCube.transform.renderer.material.mainTexture = colors[practiceCubeColor];
        practiceCube.name = practiceCube.name + "-" + practiceCube.transform.renderer.material.mainTexture.name;
        practiceCube.transform.renderer.material.color = new Color(practiceCube.gameObject.transform.renderer.material.color.r, practiceCube.gameObject.transform.renderer.material.color.g, practiceCube.gameObject.transform.renderer.material.color.b);
        //Makes the stimuli tiny, so they can be poppped in
        practiceCube.transform.localScale = new Vector3(0, 0, 0);
        practiceCubeGenerated = true;
    }

    void GenerateStimLocations()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = 100;
        for (int i = 0; i < diff; i++)
        {
            stimLocations[i] = new Vector3(
                Random.Range(handController.position.x - stimLocationSize.x / 2, handController.position.x + stimLocationSize.x / 2) + stimLocationOffset.x,
                Random.Range(handController.position.y - stimLocationSize.y / 2, handController.position.y + stimLocationSize.y / 2) + stimLocationOffset.y,
                Random.Range(handController.position.z - stimLocationSize.z / 2, handController.position.z + stimLocationSize.z / 2) + stimLocationOffset.z);
            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimLocations[i].x, stimLocations[i].z, cubePrefab.transform.localScale.x + stimOverlapBuffer.x, cubePrefab.transform.localScale.z + stimOverlapBuffer.z);
            if (boxesOverlapArray(newBox, overlapCheckList))
            {
                retries--;
                if (retries <= 0)
                {
                    //Force quit and overlap to prevent hang
                    overlapCheckList.Add(newBox);
                    Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
                }
                else i--;
            }
            else
            {
                overlapCheckList.Add(newBox);
            }
        }
    }

    private void GenerateStimuli()
    {
        Debug.Log("Generating " + diff + " stimuli");
        //Create a knuth shuffle index list of random indicies within the range of possible colors
        for (int i = 0; i < colors.Count; i++)
            colorNums[i] = i;
        for (int i = 0; i < colors.Count; i++)
        {
            int index = UnityEngine.Random.Range(i, colorNums.Length - 1);
            int tmp = colorNums[index];
            colorNums[index] = colorNums[i];
            colorNums[i] = tmp;
        }
        for (int i = 0; i < diff; i++)
        {
            // Stimuli - solid, movable
            stimuli[i] = (GameObject)Instantiate(cubePrefab);
            stimuli[i].transform.position = stimLocations[i];
            stimuli[i].transform.rotation = Random.rotation;
            stimuli[i].transform.renderer.material = new Material(Shader.Find("Diffuse"));
            stimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            stimuli[i].name = "Cube - " + stimuli[i].transform.renderer.material.mainTexture.name;
            //Makes the stimuli tiny, so they can be poppped in
            stimuli[i].transform.localScale = new Vector3(0, 0, 0);
            stimuli[i].AddComponent<SimpleObjectLogger>();
            // Practice cubes - transparent, unmovable
            transparentStimuli[i] = (GameObject)Instantiate(cubePrefab);
            transparentStimuli[i].transform.position = stimLocations[i];
            transparentStimuli[i].transform.rotation = stimuli[i].transform.rotation;
            transparentStimuli[i].transform.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            transparentStimuli[i].transform.localScale = new Vector3(0, 0, 0);
            transparentStimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            transparentStimuli[i].name = "TransparentCube - " + stimuli[i].transform.renderer.material.mainTexture.name;
            transparentStimuli[i].transform.renderer.material.color = new Color(stimuli[i].gameObject.transform.renderer.material.color.r, stimuli[i].gameObject.transform.renderer.material.color.g, stimuli[i].gameObject.transform.renderer.material.color.b, studyTransparency);
            transparentStimuli[i].tag = "Untagged";
        }
    }

    bool boxesOverlapArray(Rect box, List<Rect> boxArray)
    {
        for (var i = 0; i < boxArray.Count; i++)
            if (boxesOverlap(box, boxArray[i])) return true;
        return false;
    }

    void MakeInactive(GameObject[] popthings)
    {
        Debug.Log("Making" + diff + " things inactive");
        for (int i = 0; i < diff; i++)
            popthings[i].SetActive(false);
    }


    private bool PopIn(GameObject popthing)
    {
        popthing.transform.localScale += Vector3.one * grow;
        grow += popRate * Time.deltaTime;
        if (popthing.transform.localScale.x >= cubePrefab.transform.localScale.x)
            return true;
        else
            return false;
    }

    private bool PopOut(GameObject popthing)
    {
        popthing.transform.localScale -= Vector3.one * grow;
        grow += popRate * Time.deltaTime;
        if (practiceCube.transform.localScale.x <= 0)
            return true;
        else
            return false;
    }
    //Makes Stimuli grow / shrink rather than just appearing
    private bool PopIn(GameObject[] popthings)
    {
        for (int i = 0; i < diff; i++)
        {
            popthings[i].transform.localScale += Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        }
        if (popthings[0].transform.localScale.x >= cubePrefab.transform.localScale.x)
            return true;
        else
            return false;
    }

    private bool PopOut(GameObject[] popthings)
    {
        for (int i = 0; i < diff; i++)
        {
            popthings[i].transform.localScale -= Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        }
        if (popthings[0].transform.localScale.x <= 0)
            return true;
        else
            return false;
    }


    //Reference: http://gamemath.com/2011/09/detecting-whether-two-boxes-overlap/
    bool boxesOverlap(Rect box0, Rect box1)
    {
        if (box0.x + box0.width < box1.x) return false;
        if (box0.x > box1.x + box1.width) return false;
        if (box0.y + box0.height < box1.y) return false;
        if (box0.y > box1.y + box1.height) return false;
        return true;
    }

    void ResetStimuli()
    {
        // Determine start and end points of a line offset from the Hand Controller
        float step = 0f;
        float stepIncrease = 1f / ((diff) - 1f);
        Vector3 resetLineStart = new Vector3(
            handController.position.x - stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y - stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z - stimResetSize.z / 2 + stimResetOffset.z);
        Vector3 resetLineEnd = new Vector3(
            handController.position.x + stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y + stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z + stimResetSize.z / 2 + stimResetOffset.z);
        for (int i = 0; i < diff; ++i)
        {
            // Space the stimuli equally along the line defined earlier
            stimuli[i].transform.position = Vector3.Lerp(resetLineStart, resetLineEnd, step);
            step += stepIncrease;
            //Puts the reset position into a vector to be used later to check if the stimuli have been moved
            resetLocations[i] = stimuli[i].transform.position;
            //Makes the feedback stimuli normal size again
            transparentStimuli[i].transform.localScale = cubePrefab.transform.localScale;

        }
    }


    int GetNumberOfMovedCubes()
    {
        int movedCubes = 0;
        for (int i = 0; i < diff; ++i)
        {
            if (stimuli[i].transform.position != resetLocations[i])
                movedCubes += 1;
        }
        return movedCubes;
    }

    void TurnOffbutton()
    {
        //button.GetComponentInChildren<DetectTouch>().enabled = false;
        //button.GetComponentInChildren<BoxCollider>().enabled = false;
        button.GetComponentInChildren<ButtonManager>().SetButtonState(false);
    }

    void TurnOnbutton()
    {
        //button.GetComponentInChildren<DetectTouch>().enabled = true;
        //button.GetComponentInChildren<BoxCollider>().enabled = true;
        button.GetComponentInChildren<ButtonManager>().SetButtonState(true);
    }
    void PreparePracticeStimuli()
    {
        for (int i = 0; i < diff; i++)
        {
            transparentStimuli[i].transform.position = stimuli[i].transform.position;
            transparentStimuli[i].SetActive(true);
            //Makes stimuli more transparent
            transparentStimuli[i].renderer.material.color = new Color(transparentStimuli[i].renderer.material.color.r, transparentStimuli[i].renderer.material.color.g, transparentStimuli[i].renderer.material.color.b, transparentStimuli[i].renderer.material.color.a * .5f);

        }
    }

    public int AdaptDifficulty(float distance)
    {
        if (distance < .25)
        {
            blackboard.GetComponentInChildren<Text>().text = "Great job!\n\nWe'll make it a little harder...";
            return diff + 1;
        }
        else if (distance < .75)
        {
            blackboard.GetComponentInChildren<Text>().text = "Nicely done.";
            return diff;
        }
        else
        {
            blackboard.GetComponentInChildren<Text>().text = "Yeesh.\n\nWe'll make it a little easier";
            return diff - 1;
        }
    }

    bool WaitForHandReset()
    {
        if (GameObject.Find("RigidFullHand(Clone)") == null)
            return true;
        else
            return false;
    }

    bool StimulusPositionFeedback()
    {
        float addToSlider = 0;
        //If the stimuli have been moved back to their original position
        if (stimuli[0].transform.position == stimLocations[0])
            return true;
        for (int i = 0; i < diff; ++i)
        {
            addToSlider = addToSlider + .0015f;
            stimuli[i].transform.position = Vector3.MoveTowards(stimuli[i].transform.position, stimLocations[i], Time.deltaTime * .075f);
        }
        feedbackSlider.value += addToSlider;

            return false;
    }
    void DestroyStimuli()
    {
        for (int i = 0; i < diff; ++i)
        {
            Destroy(stimuli[i]);
            Destroy(transparentStimuli[i]);
        }        
    }

    void DisableTransparency()
    {
       //Disables the transparency script
        handController.Find("ToonLeftHand").GetComponent<ConfidenceTransparency>().enabled = false;
        handController.Find("ToonRightHand").GetComponent<ConfidenceTransparency>().enabled = false;
        //Changes the material
        handController.Find("ToonLeftHand").Find("toon_hand_left").transform.renderer.material = Resources.Load("ToonyHand") as Material;
        handController.Find("ToonRightHand").Find("toon_hand_right").transform.renderer.material = Resources.Load("ToonyHand") as Material;
    }
}