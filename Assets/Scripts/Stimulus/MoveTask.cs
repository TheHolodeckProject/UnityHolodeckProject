using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// TO DO
//     1) Destroy cubes at the end of every trial
// ??? 2) Disable grab/stretch (or remove "Loggable" tag) in study phase
        // What's the best way to do this? 
// 3) Set up logger
// 4) Animate reset button (frivolous)

public class MoveTask : MonoBehaviour
{
    public GameObject cube;
    public GameObject resetMarker;
    private GameObject[] stimuli;
    private Transform handController;
    public int totalTrials = 3;
    public int numberOfStim = 3;
    public float studyTime = 3f;
    public float Transparency = .9f; //How transparent the stimuli are (0 = invisible, 1 = opaque)
    private float timeLeft;
    private Vector3[] stimLocations;
    private Vector3[] resetLocations;
    private int currentTrial;
    private Controller controller;
    public Vector3 stimLocationSize = new Vector3(.2f, .4f, .2f);
    public Vector3 stimLocationOffset = new Vector3(0f, .4f, 0f);
    public Vector3 stimResetSize = new Vector3(.2f, 0f, 0f);
    public Vector3 stimResetOffset = new Vector3(0f, .4f, 0f);
    private int grabbedCubes;
    private bool doneRecall;
    enum State { TrialStart, OnStudy, Study, OnRecall, Recall, RecallFinished, TrialEnd, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        resetMarker.SetActive(false);
        handController = GameObject.Find("HandController").transform;
        stimLocations = new Vector3[numberOfStim];
        resetLocations = new Vector3[numberOfStim];
        stimuli = new GameObject[numberOfStim];
        currentState = State.TrialStart;
        currentTrial = 0;
        cube.tag = "Loggable";
        doneRecall = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.TrialStart:
                currentTrial += 1;
                GenerateStimLocations();
                GenerateStimuli();
                currentState = State.OnStudy;
                break;

            case State.OnStudy:
                timeLeft = studyTime;
                currentState = State.Study;
                break;

            case State.Study:
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                    currentState = State.OnRecall;
                break;

            case State.OnRecall:
                ResetStimuli();
                currentState = State.Recall;
                break;

            case State.Recall:
                grabbedCubes = WaitForResponses();
                if (grabbedCubes >= numberOfStim)
                {
                    resetMarker.SetActive(true);
                    currentState = State.RecallFinished;
                }
                break;

            case State.RecallFinished:
                doneRecall = WaitForSphereTouch();
                if (doneRecall)
                {
                    resetMarker.SetActive(false);
                    currentState = State.TrialEnd;
                }
                break;

            case State.TrialEnd:
                Debug.Log("Current trial = " + currentTrial);
                Debug.Log("Total trials = " + totalTrials);
                if (currentTrial >= totalTrials)
                    currentState = State.TaskEnd;
                else
                {
                    DestroyStimuli();
                    currentState = State.TrialStart;
                }
                break;

            case State.TaskEnd:
                Debug.Log("Task over");
                break;

        }
    }

    void GenerateStimLocations()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = 100;
        for (int i = 0; i < numberOfStim; i++)
        {
            stimLocations[i] = new Vector3(
                Random.Range(handController.position.x - stimLocationSize.x / 2, handController.position.x + stimLocationSize.x / 2) + stimLocationOffset.x,
                Random.Range(handController.position.y - stimLocationSize.y / 2, handController.position.y + stimLocationSize.y / 2) + stimLocationOffset.y,
                Random.Range(handController.position.z - stimLocationSize.z / 2, handController.position.z + stimLocationSize.z / 2) + stimLocationOffset.z);
            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimLocations[i].x, stimLocations[i].z, cube.transform.localScale.x * 2, cube.transform.localScale.z * 2);
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
        //Initializes a list to hold textures created 
        List<Texture> colors = new List<Texture>();
        colors.Add(Resources.Load("Red", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Blue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Yellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Orange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Green", typeof(Texture2D)) as Texture);

        //Create a knuth shuffle index list of random indicies within the range of possible colors
        int[] colorNums = new int[colors.Count];
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
        for (int i = 0; i < numberOfStim; i++)
        {
            stimuli[i] = Resources.Load("StretchableCube") as GameObject;
            stimuli[i].transform.position = stimLocations[i];
            stimuli[i].transform.rotation = Random.rotation;
            stimuli[i].transform.renderer.material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
            stimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            stimuli[i].transform.renderer.material.color = new Color(stimuli[i].gameObject.transform.renderer.material.color.r, stimuli[i].gameObject.transform.renderer.material.color.g, stimuli[i].gameObject.transform.renderer.material.color.b, Transparency);
            //Actually instantiates the stimuli
            stimuli[i] = (GameObject)Instantiate(stimuli[i]);
        }
    }

    bool boxesOverlapArray(Rect box, List<Rect> boxArray)
    {
        for (var i = 0; i < boxArray.Count; i++)
            if (boxesOverlap(box, boxArray[i])) return true;
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
        float stepIncrease = 1f / ((numberOfStim) - 1f);
        Vector3 resetLineStart = new Vector3(
            handController.position.x - stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y - stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z - stimResetSize.z / 2 + stimResetOffset.z);
        Vector3 resetLineEnd = new Vector3(
            handController.position.x + stimResetSize.x / 2 + stimResetOffset.x,
            handController.position.y + stimResetSize.y / 2 + stimResetOffset.y,
            handController.position.z + stimResetSize.z / 2 + stimResetOffset.z);

        for (int i = 0; i < numberOfStim; i++)
        {
            // Space the stimuli equally along the line defined earlier
            stimuli[i].transform.position = Vector3.Lerp(resetLineStart, resetLineEnd, step);
            step += stepIncrease;
            //Puts the reset position into a vector to be used later to check if the stimuli have been moved
            resetLocations[i] = stimuli[i].transform.position;
        }
    }

    // ??? More efficient way of waiting until all 3 have been grabbed?
    int WaitForResponses()
    {
        grabbedCubes = 0;
        for (int i = 0; i < numberOfStim; i++)
        {
            //Debug.Log("transform position = " + stimuli[i].transform.position + " ---- Stimlocation[i] = " + stimLocations[i]);
            if (stimuli[i].transform.position != resetLocations[i])
                grabbedCubes += 1;
        }
        return grabbedCubes;
    }

    bool WaitForSphereTouch()
    {
        //Grabs the fingerTouch variable from the DetectTouch script, which has been placed on a smaller sphere collider in the middle of the EndTrail sphere
        return resetMarker.transform.GetComponentInChildren<DetectTouch>().fingerTouch;
    }

    void DestroyStimuli()
    {
        Debug.Log("Destroying stimuli");
        for (int i = 0; i >= stimuli.Length; i++)
            Destroy(stimuli[i]);
    }
}