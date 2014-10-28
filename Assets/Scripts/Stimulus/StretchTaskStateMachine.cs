using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// TO DO
   // 1) Get cubes to generate in differnet colors
   // 2) Disable grab/stretch (or remove "Loggable" tag) in study phase
   // 4) Set up logger in post-trial phase
   // 5) Insert "Reset Stimuli" button
public class StretchTaskStateMachine : MonoBehaviour
{
    public GameObject cube;
    private GameObject[] stimuli;
    private Transform handController;
    public int totalTrials = 3;
    public int numberOfStim = 1;
    public float resetCubeSize = .5f;
    public float studyTime = 3f;
    public float minCubeSize = .05f;
    public float maxCubeSize = .15f;
    private float timeLeft;
    private Vector3[] stimLocations;
    private int currentTrial;
    private Controller controller;
    public Vector3 stimLocationSize = new Vector3(.2f, .4f, .2f);
    public Vector3 stimLocationOffset = new Vector3(0f, .4f, 0f);
    public Vector3 stimResetSize = new Vector3(.2f, 0f, 0f);
    public Vector3 stimResetOffset = new Vector3(0f, .4f, 0f);
    private int grabbedCubes;
    enum State { TrialStart, OnStudy, Study, OnRecall, Recall, TrialEnd, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        handController = GameObject.Find("HandController").transform;
        stimLocations = new Vector3[numberOfStim];
        stimuli = new GameObject[numberOfStim];
        currentState = State.TrialStart;
        currentTrial = 0;
        cube.tag = "Loggable";



        
        //InteractionBox iBox = GetInteractionBox();
       //GameObject interactionBoxCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       //interactionBoxCube.transform.position = new Vector3(iBox.Center.ToUnityScaled().x, iBox.Center.ToUnityScaled().y, iBox.Center.ToUnityScaled().z);
       //Vector v = new Vector(iBox.Width, iBox.Height, iBox.Depth);
       //interactionBoxCube.transform.localScale = new Vector3(v.ToUnityScaled().x, v.ToUnityScaled().y, v.ToUnityScaled().z);
       //Debug.Log(" x: " + iBox.Center.ToUnityScaled().x + " y:" + iBox.Center.ToUnityScaled().y + " z:" + iBox.Center.ToUnityScaled().z + " width: " + v.ToUnityScaled().x + " height: " + v.ToUnityScaled().y + " depth: " + v.ToUnityScaled().z);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.TrialStart:
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
                    currentState = State.TrialEnd;
                break;

            case State.TrialEnd:
                break;
        }
    }

    InteractionBox GetInteractionBox()
    {
        return GameObject.Find("HandController").GetComponent<HandController>().latestFrame.InteractionBox;
    }

    void GenerateStimLocations()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = 100;
        for (int i = 0; i < numberOfStim; i++)
        {
            stimLocations[i] = new Vector3(
                Random.Range(handController.position.x - stimLocationSize.x/2, handController.position.x + stimLocationSize.x/2) + stimLocationOffset.x,
                Random.Range(handController.position.y - stimLocationSize.y/2, handController.position.y + stimLocationSize.y/2) + stimLocationOffset.y,
                Random.Range(handController.position.z - stimLocationSize.z/2, handController.position.z + stimLocationSize.z/2) + stimLocationOffset.z);
            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimLocations[i].x, stimLocations[i].z, cube.transform.localScale.x*2, cube.transform.localScale.z*2);
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
        for (int i = 0; i < numberOfStim; i++)
        {
            cube = Instantiate(Resources.Load("StretchableCube")) as GameObject;
            stimuli[i] = cube;
            stimuli[i].transform.position = stimLocations[i];
            stimuli[i].transform.rotation = Random.rotation;
            // !!! Should the total size be the same for every stimulus?
            stimuli[i].transform.localScale = new Vector3(Random.Range(minCubeSize, maxCubeSize), Random.Range(minCubeSize, maxCubeSize), Random.Range(minCubeSize, maxCubeSize));
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
        float step = 1f / (numberOfStim + 1);
        Vector3 resetLineStart = new Vector3(
            handController.position.x - stimResetSize.x/2 + stimResetOffset.x,
            handController.position.y - stimResetSize.y/2 + stimResetOffset.y,
            handController.position.z - stimResetSize.z/2 + stimResetOffset.z);
        Vector3 resetLineEnd = new Vector3(
            handController.position.x + stimResetSize.x/2 + stimResetOffset.x,
            handController.position.y + stimResetSize.y/2 + stimResetOffset.y,
            handController.position.z + stimResetSize.z/2 + stimResetOffset.z);

        for (int i = 0; i < numberOfStim; i++)
        {
            // Space the stimuli equally along the line defined earlier
            stimuli[i].transform.position = Vector3.Lerp(resetLineStart, resetLineEnd, step);
            step += step;
            // Reset the stimulus size
            stimuli[i].transform.localScale = new Vector3 (resetCubeSize, resetCubeSize, resetCubeSize);
        }
    }

    
    int WaitForResponses()
    {
        grabbedCubes = 0;
        for (int i = 0; i < numberOfStim; i++)
        {
            if (stimuli[i].transform.position != stimLocations[i])
                grabbedCubes += 1;
        }
        return grabbedCubes;
    }
}