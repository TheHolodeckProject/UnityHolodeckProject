using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;


public class StretchTaskStateMachine : MonoBehaviour
{
    public GameObject cube;
    private GameObject[] stimuli;
    public int totalTrials = 3;
    public int numberOfStim = 1;
    public float cubeScale = .5f;
    private Vector3[] stimLocations;
    private int currentTrial;
    private Controller controller;
    public Vector3 randomBoundsP0 = new Vector3(-.3f, -42.1f, -.1f);
    public Vector3 randomBoundsP1 = new Vector3(.3f, -42.3f, .1f);
    enum State { TrialStart = 0, TrialIdle, TrialEnd, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        stimLocations = new Vector3[numberOfStim];
        stimuli = new GameObject[numberOfStim];
        currentState = State.TrialStart;
        currentTrial = 0;
        cube.tag = "Loggable";


        // ??? Not working
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
        switch (currentState)
        {
            case State.TrialStart:
                GenerateStimLocations();
                GenerateStimuli();
                currentState = State.TrialIdle;
                break;

            case State.TrialIdle:
                break;
        }
    }

    InteractionBox GetInteractionBox()
    {
        return GameObject.Find("HandController").GetComponent<HandController>().latestFrame.InteractionBox;
    }

    void GenerateStimLocations()
    {
        for (int i = 0; i < numberOfStim; i++)
        {
            stimLocations[i] = new Vector3 (Random.Range(-.3f, .3f), Random.Range(-42.1f, -42.3f), Random.Range(-.1f, .1f));
          //  stimSizes[i] = new Vector3(LeapStretchStateMachine.MinCubeSize LeapStretchStateMachine.MaxCubeSize)

            //    //Position stimuli randomly according to settings (no overlaps)
            //    List<Rect> overlapCheckList = new List<Rect>();
            //    int retries = 100;
            //    for (int i = 0; i < numberOfStim; i++)
            //    {
            //        stimLocations[i] = new Vector3(
            //            Random.Range(randomBoundsP0.x > randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x,
            //                      randomBoundsP0.x < randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x),
            //            Random.Range(randomBoundsP0.y > randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y,
            //                      randomBoundsP0.y < randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y),
            //            Random.Range(randomBoundsP0.z > randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z,
            //                      randomBoundsP0.z < randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z));
            //        //Check for overlapping boxes and regenerate box location if overlap occurs
            //        Rect newBox = new Rect(stimuli[i].x,
            //                               stimuli[i].z,
            //                               stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x * stimuli[i].transform.localScale.x,
            //                               stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z * stimuli[i].transform.localScale.z);
            //        if (boxesOverlapArray(newBox, overlapCheckList))
            //        {
            //            retries--;
            //            if (retries <= 0)
            //            {
            //                //Force quit and overlap to prevent hang
            //               // retries = numRetries;
            //                overlapCheckList.Add(newBox);
            //                Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
            //            }
            //            else i--;
            //        }
            //        else
            //        {
            //         //   retries = numRetries;
            //            overlapCheckList.Add(newBox);
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
        }
    }
    //void generateRandomPositions()
    //{
    //    //Position stimuli randomly according to settings (no overlaps)
    //    List<Rect> overlapCheckList = new List<Rect>();
    //    int retries = 100;
    //    for (int i = 0; i < numberOfStim; i++)
    //    {
    //        stimLocations[i] = new Vector3(
    //            Random.Range(randomBoundsP0.x > randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x,
    //                      randomBoundsP0.x < randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x),
    //            Random.Range(randomBoundsP0.y > randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y,
    //                      randomBoundsP0.y < randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y),
    //            Random.Range(randomBoundsP0.z > randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z,
    //                      randomBoundsP0.z < randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z));
    //        //Check for overlapping boxes and regenerate box location if overlap occurs
    //        Rect newBox = new Rect(stimuli[i].x,
    //                               stimuli[i].z,
    //                               stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x * stimuli[i].transform.localScale.x,
    //                               stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z * stimuli[i].transform.localScale.z);
    //        if (boxesOverlapArray(newBox, overlapCheckList))
    //        {
    //            retries--;
    //            if (retries <= 0)
    //            {
    //                //Force quit and overlap to prevent hang
    //               // retries = numRetries;
    //                overlapCheckList.Add(newBox);
    //                Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
    //            }
    //            else i--;
    //        }
    //        else
    //        {
    //         //   retries = numRetries;
    //            overlapCheckList.Add(newBox);
    //        }
    //    }
    //}

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
}