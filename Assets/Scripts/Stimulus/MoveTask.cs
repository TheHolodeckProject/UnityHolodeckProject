using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// ??? What's the proper way for other scripts to access the current state? Should I make it a public variable?

// TO DO
// 1) Figure out how to consistently align the Oculus camera with player position
// 2) Add a "Press Esc to quit" function
// 3) The area for generating stimuli shoulnd't be a rectangle. it should be a pyramid extending out from the handcontroller with a rectangle on top of it OR if we can access the Leap InteractionBox, we could probably use that, but I couldn't figure it out.
        // When really close to the surface of the table (small y), hand tracking is great when right above the hand tracker but terrible when out of that cone.
// 4) Animate reset button press (frivolous)

public class MoveTask : MonoBehaviour
{
    public GameObject cube;
    public GameObject resetButton;
    public float studyTransparency = 0.925f;
    public int ITI;
    public float popRate;
    public int totalTrials = 3;
    public int minNumberOfStim = 2;
    public int maxNumberOfStim = 5;
    public float studyTime = 3f;
    public Vector3 stimLocationSize;
    public Vector3 stimLocationOffset;
    public Vector3 stimOverlapBuffer;
    public Vector3 stimResetSize;
    public Vector3 stimResetOffset;
    private GameObject[] stimuli;
    private GameObject template;
    private GameObject practiceCube;
    private int trialNumberOfStim;
    private Transform handController;
    private bool practiceCubeGenerated;
    private float timeLeft;
    private int[] colorNums;
    private Vector3[] stimLocations;
    private Vector3[] resetLocations;
    private int currentTrial;
    private Controller controller;
    private bool doneRecall;
    private Logger logger;
    private List<Texture> colors;
    private float grow;
    enum State { PracticeStart, Practice, PracticeEnd, TrialStart, OnStudy, Study, OnRecall, Recall, RecallFinished, TrialEnd, InterTrialInterval, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        handController = GameObject.Find("HandController").transform;
        stimLocations = new Vector3[maxNumberOfStim];
        resetLocations = new Vector3[maxNumberOfStim];
        stimuli = new GameObject[maxNumberOfStim];
        template = Resources.Load("MovableCube") as GameObject;
        currentTrial = 0;
        doneRecall = false;
        logger = GameObject.Find("Logger").GetComponent<Logger>();
        logger.BeginLogging();
        practiceCubeGenerated = false;
        //Initializes a list to hold the different colors
        colors = new List<Texture>();
        colors.Add(Resources.Load("Red", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkRed", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightRed", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Orange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightOrange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkOrange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Yellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightYellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkYellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Green", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightGreen", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkGreen", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Blue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightBlue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkBlue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Purple", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("LightPurple", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("DarkPurple", typeof(Texture2D)) as Texture);
        colorNums = new int[colors.Count];
        currentState = State.PracticeStart;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        //Debug.Log(currentState);
        switch (currentState)
        {

            case State.PracticeStart:
                if (!practiceCubeGenerated)
                    GeneratePracticeCube();
                else
                {
                    bool practicePopInComplete = PopInPracticeCube();
                    if (practicePopInComplete)
                        currentState = State.Practice;
                }
                break;
        

            case State.Practice:
                    bool practiceOver = WaitForResetButtonTouch();
                    if (practiceOver)
                    {
                        //Have to reset fingertouch or else it's still true when the resetButton gets reactivated in the next trial
                        resetButton.transform.FindChild("Button").GetComponent<DetectTouch>().fingerTouch = false;
                        resetButton.transform.FindChild("Button").transform.renderer.material.color = new UnityEngine.Color(resetButton.transform.FindChild("Button").transform.renderer.material.color.r - .5f, resetButton.transform.FindChild("Button").transform.renderer.material.color.g, resetButton.transform.FindChild("Button").transform.renderer.material.color.b, resetButton.transform.FindChild("Button").transform.renderer.material.color.a);
                        currentState = State.PracticeEnd;
                    }
                    break;

            case State.PracticeEnd:
                bool practicePopOutComplete = PopOutPracticeCube();
                if (practicePopOutComplete)
                {
                    //Changes the hand to stop being transparent
                    handController.Find("RiggedLeftHand").GetComponent<ConfidenceTransparency>().enabled = false;
                    handController.Find("RiggedRightHand").GetComponent<ConfidenceTransparency>().enabled = false;

                    Destroy(practiceCube);
                    currentState = State.TrialStart;
                }
                break;
            
            case State.TrialStart:
                currentTrial += 1;
                trialNumberOfStim = Random.Range(minNumberOfStim, maxNumberOfStim);
                GenerateStimLocations();
                GenerateStimuli();
                logger.RegenerateStimuliInSameFile();
                MakeStimuliUngrabbable();
                currentState = State.OnStudy;
                break;

            case State.OnStudy:
                bool popInComplete = PopInStimuli();
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
                    popOutComplete = PopOutStimuli();
                    if (popOutComplete)
                    {
                        ResetStimuli();
                        currentState = State.OnRecall;
                    }
                }
                break;

            case State.OnRecall:
                popInComplete = PopInStimuli();
                if (popInComplete)
                {
                    currentState = State.Recall;
                }
                break;

            case State.Recall:
                int movedCubes = GetNumberOfMovedCubes();
                if (movedCubes >= trialNumberOfStim)
                {
                    resetButton.transform.FindChild("Button").transform.renderer.material.color = new UnityEngine.Color(resetButton.transform.FindChild("Button").transform.renderer.material.color.r + .5f, resetButton.transform.FindChild("Button").transform.renderer.material.color.g, resetButton.transform.FindChild("Button").transform.renderer.material.color.b, resetButton.transform.FindChild("Button").transform.renderer.material.color.a);
                    currentState = State.RecallFinished;
                }

                break;

            case State.RecallFinished:
                doneRecall = WaitForResetButtonTouch();
                if (doneRecall)
                {
                    resetButton.transform.FindChild("Button").transform.renderer.material.color = new UnityEngine.Color(resetButton.transform.FindChild("Button").transform.renderer.material.color.r - .5f, resetButton.transform.FindChild("Button").transform.renderer.material.color.g, resetButton.transform.FindChild("Button").transform.renderer.material.color.b, resetButton.transform.FindChild("Button").transform.renderer.material.color.a);
                    if (currentTrial >= totalTrials)
                    {
                        currentState = State.TaskEnd;
                        logger.FinishTrial(currentTrial);
                        logger.Finish();
                    }
                    else
                    {
                        //Have to reset fingertouch or else it's still true when the resetButton gets reactivated in the next trial
                        resetButton.transform.FindChild("Button").GetComponent<DetectTouch>().fingerTouch = false;
                        currentState = State.TrialEnd;
                        logger.FinishTrial(currentTrial);
                    }
                }
                break;

            case State.TrialEnd:
                popOutComplete = PopOutStimuli();
                if (popOutComplete)
                {
                    DestroyStimuli();
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
                ShowLogo();
                break;
        }
    }


    private void GeneratePracticeCube()
    {
        //Generates a random integer that we'll use to pick a color
        int practiceCubeColor = Random.Range(0, colors.Count - 1);
        practiceCube = (GameObject)Instantiate(template);
        practiceCube.transform.position = handController.transform.position + stimLocationOffset;
        practiceCube.transform.rotation = Random.rotation;
        //practiceCube.transform.renderer.material = new Material(Shader.Find("Self-Illumin/Diffuse"));
        //practiceCube.transform.renderer.material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
        practiceCube.transform.renderer.material = new Material(Shader.Find("Diffuse"));
        practiceCube.transform.renderer.material.mainTexture = colors[practiceCubeColor];
        practiceCube.name = practiceCube.name + "-" + practiceCube.transform.renderer.material.mainTexture.name;
        practiceCube.transform.renderer.material.color = new Color(practiceCube.gameObject.transform.renderer.material.color.r, practiceCube.gameObject.transform.renderer.material.color.g, practiceCube.gameObject.transform.renderer.material.color.b);
        //Makes the stimuli tiny, so they can be poppped in
        practiceCube.transform.localScale = new Vector3(0, 0, 0);
        practiceCubeGenerated = true;
    }

    private bool PopInPracticeCube()
    {
      practiceCube.transform.localScale += Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        if (practiceCube.transform.localScale.x >= cube.transform.localScale.x)
            return true;
        else
            return false;
    }

    private bool PopOutPracticeCube()
    {
        practiceCube.transform.localScale -= Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        if (practiceCube.transform.localScale.x <= 0)
            return true;
        else
            return false;
    }

    void GenerateStimLocations()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = 100;
        for (int i = 0; i < trialNumberOfStim; i++)
        {
            stimLocations[i] = new Vector3(
                Random.Range(handController.position.x - stimLocationSize.x / 2, handController.position.x + stimLocationSize.x / 2) + stimLocationOffset.x,
                Random.Range(handController.position.y - stimLocationSize.y / 2, handController.position.y + stimLocationSize.y / 2) + stimLocationOffset.y,
                Random.Range(handController.position.z - stimLocationSize.z / 2, handController.position.z + stimLocationSize.z / 2) + stimLocationOffset.z);
            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimLocations[i].x, stimLocations[i].z, cube.transform.localScale.x + stimOverlapBuffer.x, cube.transform.localScale.z + stimOverlapBuffer.z);
            //Rect newBox = new Rect(stimLocations[i].x, stimLocations[i].z, cube.transform.localScale.x * 2 + stimOverlapBuffer.x, cube.transform.localScale.z * 2 + stimOverlapBuffer.z);

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
        //Create a knuth shuffle index list of random indicies within the range of possible colors
        //int[] colorNums = new int[colors.Count];
        for (int i = 0; i < colors.Count; i++)
            colorNums[i] = i;
        for (int i = 0; i < colors.Count; i++)
        {
            int index = UnityEngine.Random.Range(i, colorNums.Length - 1);
            int tmp = colorNums[index];
            colorNums[index] = colorNums[i];
            colorNums[i] = tmp;
        }
        //Defines the stimulus properties
        for (int i = 0; i < trialNumberOfStim; i++)
        {
            stimuli[i] = (GameObject)Instantiate(template);
            stimuli[i].transform.position = stimLocations[i];
            stimuli[i].transform.rotation = Random.rotation;
            //stimuli[i].transform.renderer.material = new Material(Shader.Find("Self-Illumin/Diffuse"));
            //Using transparent shader during study phase
            //stimuli[i].transform.renderer.material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
            stimuli[i].transform.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            stimuli[i].transform.renderer.material.color = new Color(stimuli[i].gameObject.transform.renderer.material.color.r, stimuli[i].gameObject.transform.renderer.material.color.g, stimuli[i].gameObject.transform.renderer.material.color.b,studyTransparency);
            stimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            stimuli[i].name = stimuli[i].name + "-" + stimuli[i].transform.renderer.material.mainTexture.name;
            //Makes the stimuli tiny, so they can be poppped in
            stimuli[i].transform.localScale = new Vector3(0, 0, 0);

            stimuli[i].AddComponent<SimpleObjectLogger>();
        }
    }

    private void MakeStimuliUngrabbable()
    {
        for (int i = 0; i < trialNumberOfStim; i++)
            stimuli[i].tag = "Untagged";
    }

    bool boxesOverlapArray(Rect box, List<Rect> boxArray)
    {
        for (var i = 0; i < boxArray.Count; i++)
            if (boxesOverlap(box, boxArray[i])) return true;
        return false;
    }

    //Makes Stimuli grow / shrink rather than just appearinginto their natural size
    private bool PopInStimuli()
    {
        for (int i = 0; i < trialNumberOfStim; i++)
        {
            stimuli[i].transform.localScale += Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        }
            if (stimuli[0].transform.localScale.x >= cube.transform.localScale.x)
                return true;
            else
                return false;
    }

    private bool PopOutStimuli()
    {
        for (int i = 0; i < trialNumberOfStim; i++)
        {
            stimuli[i].transform.localScale -= Vector3.one * grow;
            grow += popRate * Time.deltaTime;
        }
        if (stimuli[0].transform.localScale.x <= 0)
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
        float stepIncrease = 1f / ((trialNumberOfStim) - 1f);
        Vector3 resetLineStart = new Vector3(
            handController.position.x - stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y - stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z - stimResetSize.z / 2 + stimResetOffset.z);
        Vector3 resetLineEnd = new Vector3(
            handController.position.x + stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y + stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z + stimResetSize.z / 2 + stimResetOffset.z);

        for (int i = 0; i < trialNumberOfStim; ++i)
        {
            // Space the stimuli equally along the line defined earlier
            stimuli[i].transform.position = Vector3.Lerp(resetLineStart, resetLineEnd, step);
            step += stepIncrease;
            //Puts the reset position into a vector to be used later to check if the stimuli have been moved
            resetLocations[i] = stimuli[i].transform.position;
            //ADDED - Makes them movable
            stimuli[i].tag = "Movable";
            //ADDED - Makes them opaque
            //stimuli[i].renderer.material.color = new Color(stimuli[i].renderer.material.color.r, stimuli[i].renderer.material.color.g, stimuli[i].renderer.material.color.b, 255);
            stimuli[i].transform.renderer.material = new Material(Shader.Find("Diffuse"));
            stimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            //stimuli[i].transform.renderer.material.color = new Color(stimuli[i].gameObject.transform.renderer.material.color.r, stimuli[i].gameObject.transform.renderer.material.color.g, stimuli[i].gameObject.transform.renderer.material.color.b);
            
        }
    }

    
    int GetNumberOfMovedCubes()
    {
        int movedCubes = 0;
        for (int i = 0; i < trialNumberOfStim; ++i)
        {
            if (stimuli[i].transform.position != resetLocations[i])
                movedCubes += 1;
        }
        return movedCubes;
    }

    bool WaitForResetButtonTouch()
    {
        //Grabs the fingerTouch variable from the DetectTouch script, which has been placed on a smaller sphere collider in the middle of the EndTrail sphere
        return resetButton.transform.GetComponentInChildren<DetectTouch>().fingerTouch;
    }

    void DestroyStimuli()
    {
        for (int i = 0; i < trialNumberOfStim; ++i)
            Destroy(stimuli[i]);
    }

    void ShowLogo()
    {
        GameObject screen = GameObject.Find("BlackScreen");
        screen.renderer.material = Resources.Load("Holodeck Logo") as Material;

    }
}