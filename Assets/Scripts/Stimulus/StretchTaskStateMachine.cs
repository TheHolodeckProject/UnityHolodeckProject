using UnityEngine;
using System.Collections;
using Leap;

public class StretchTaskStateMachine : MonoBehaviour
{
    public Transform cube;
    public int totalTrials = 3;
    public int numberOfStim = 1;
    private GameObject[] stimuli;
    private Vector3[] stimLocations;
    private int currentTrial;
    private Controller controller;
    enum State { TrialStart = 0, TrialIdle, TrialEnd, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        stimLocations = new Vector3[numberOfStim];
        stimuli = new GameObject[numberOfStim];
        currentState = State.TrialStart;
        currentTrial = 0;
        //ADDED
       controller = new Controller();

       InteractionBox iBox = GetInteractionBox();
       GameObject interactionBoxCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       interactionBoxCube.transform.position = new Vector3(iBox.Center.ToUnityScaled().x, iBox.Center.ToUnityScaled().y, iBox.Center.ToUnityScaled().z);
       Vector v = new Vector(iBox.Width, iBox.Height, iBox.Depth);
       interactionBoxCube.transform.localScale = new Vector3(v.ToUnityScaled().x, v.ToUnityScaled().y, v.ToUnityScaled().z);
       
       Debug.Log(" x: " + iBox.Center.ToUnityScaled().x + " y:" + iBox.Center.ToUnityScaled().y + " z:" + iBox.Center.ToUnityScaled().z + " width: " + v.ToUnityScaled().x + " height: " + v.ToUnityScaled().y + " depth: " + v.ToUnityScaled().z);
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
                Debug.Log("Recalling");
                break;
        }
    }

    InteractionBox GetInteractionBox()
    {
        return GameObject.Find("HandController").GetComponent<HandController>().latestFrame.InteractionBox;
    }

    // ??? How to access frame.LeapInteractionBox? I want to use that as the limits of where to generate stimuli
    void GenerateStimLocations()
    {
        for (int i = 0; i < numberOfStim; i++)
        {
            stimLocations[i] = new Vector3 (Random.Range(-.3f, .3f), Random.Range(-42f, -42.4f), Random.Range(-.15f, .15f));
          //  stimSizes[i] = new Vector3(LeapStretchStateMachine.MinCubeSize LeapStretchStateMachine.MaxCubeSize)
        }
    }
    private void GenerateStimuli()
    {
        for (int i = 0; i < numberOfStim; i++)
        {
            Debug.Log("Generating Stimuli");
            stimuli[i] = Instantiate(cube, stimLocations[i],Random.rotation) as GameObject;
        }
    }
}