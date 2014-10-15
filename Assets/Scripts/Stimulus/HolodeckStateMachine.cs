using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;


// ??? namespace vs public class? 
//namespace StateMachineDemoSharp
public class HolodeckStateMachine : MonoBehaviour
{

    // ??? Should all of this be here or in TaskStart?
    public int numberOfStimuli = 3; //Static value representing number of stimuli for task
    public float Transparency = .9f; //How transparent the stimuli are (0 = invisible, 1 = opaque)
    private int[] colorNums; //A to-be-randomized list of numbers
    private Texture[] colors;
    private int stimNum;
    private bool[] stimuliTouchedState;
    private bool[] stimuliMovedState;
    private GameObject[] stimuli;
    public int expectedNumberOfTrials = 3;
    private int currentTrialNumber;
    ////Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
    public Vector3 resetPositionP0 = new Vector3(-4f, 0f, -8f); //Position for obj0
    public Vector3 resetPositionP1 = new Vector3(4f, 0f, -8f); //Position for objN
    //Random Bounds determines the 3D box which bounds the possible random positions of each object
    //The point P0 determines one corner of the box while the point P1 determines the opposite corner
    public Vector3 randomBoundsP0 = new Vector3(-2f, 1f, -10f); //One corner of the bounding box being used
    public Vector3 randomBoundsP1 = new Vector3(1f, 4f, -7f); //The opposite corner of the bounding box being used


    //public partial class Form1 : Form

    enum State { TaskStart = 0, BeginStudy, IdleStudy, BeginRecall, IdleRecall, Evaluate, BeginTaskEnd, IdleTaskEnd };
    private State currentState;

    void Start()
    {
        currentState = State.TaskStart;
        currentTrialNumber = 0;
    }

    void Update()
    {
        switch (currentState)
        {
            //??? This is where I put all the things that need to happen as soon as the task starts? Like, void Start()?
            case State.TaskStart:

                //Removes the "grabbable" tag from the meshes. Because the meshes are now centered around parent objects, you grab the parents instead.
                //For every stimulus
                //Generates the blobs with appropriate shaders and colors
                generateRandomStimuli();

                // ??? How do I tell it that it's ready to go to the next state? Do I set Transition.Next = true or something?

                currentState = State.BeginStudy;
                break;

            // ??? Would it make sense to have a separate state for "Set up stimuli"?
            case State.BeginStudy:
                Debug.Log("Study Phase");
                //Generates random locations within specified bounds, so that they never overlap
                generateRandomPositions();

                currentState = State.IdleStudy;
                break;
            case State.IdleStudy:
                if (idleStudyConditionIsMet())
                    currentState = State.BeginRecall;
                break;
            case State.BeginRecall:
                Debug.Log("Recall Phase");
                //For every stimulus
                for (int i = 0; i < stimuli.Length; i++)
                {
                    // ??? Should resetting the object positions to a line be at the start of Recall, at the end of Study, or in its own separate Phase? Does it matter?
                    //Resets the object positions
                    stimuli[i].transform.localPosition = Vector3.Lerp(resetPositionP0, resetPositionP1, (((float)i) * (1f / ((float)stimuli.Length))));
                    //Makes the objects grabbable
                    stimuli[i].gameObject.tag = "Loggable";

                }
                currentState = State.IdleRecall;
                break;
            case State.IdleRecall:
                if (idleRecallConditionIsMet())
                    currentState = State.Evaluate;
                break;
            case State.Evaluate:
                //output information to logger
                //generate summary file
                //etc
                currentTrialNumber++;
                if (currentTrialNumber == expectedNumberOfTrials) //End
                    currentState = State.BeginTaskEnd;
                else //Reset
                    currentState = State.TaskStart;
                break;
            case State.BeginTaskEnd:
                //Put the system into a safe state
                currentState = State.IdleTaskEnd;
                break;
            case State.IdleTaskEnd:
                //Do nothing unless influenced from outside
                break;
        }
    }
    
    private bool idleRecallConditionIsMet()
    {
        bool allItemsMoved = true;
        for (int i = 0; i < stimuliMovedState.Length; i++)
            allItemsMoved = allItemsMoved && stimuliMovedState[i];

        return allItemsMoved;
    }

    private bool idleStudyConditionIsMet()
    {
        bool allItemsTouched = true;
        for (int i = 0; i < stimuliTouchedState.Length; i++)
            allItemsTouched = allItemsTouched && stimuliTouchedState[i];

        return allItemsTouched;
    }

    public void ObjectTouched(GameObject g)
    {
        for (int i = 0; i < stimuli.Length; i++)
        {
            if (g == stimuli[i]) stimuliTouchedState[i] = true;
            return;
        }
        throw new KeyNotFoundException("No object found in stimuli array that matches touched object.");
    }

    public void ObjectMoved(GameObject g)
    {
        for (int i = 0; i < stimuli.Length; i++)
        {
            if (g == stimuli[i]) stimuliMovedState[i] = true;
            return;
        }
        throw new KeyNotFoundException("No object found in stimuli array that matches touched object.");
    }

    // ??? Where do the functions get defined? Within the relevant state?
    private void generateRandomStimuli()
    {
        if (stimuli != null)
            for (int i = 0; i < stimuli.Length; i++)
            {
                try
                {
                    DestroyImmediate(stimuli[i]);
                }
                catch (UnityException)
                {
                }
            }

        stimuli = new GameObject[numberOfStimuli];
        stimuliTouchedState = new bool[stimuli.Length];
        stimuliMovedState = new bool[stimuli.Length];
        for (int i = 0; i < stimuliTouchedState.Length; i++)
        {
            stimuliTouchedState[i] = false;
            stimuliMovedState[i] = false;
        }
        //Creates a list of integers, 1 for each color
        //Create a knuth shuffle index list of random indicies within the range of possible colors
        int[] stimNums = new int[100];
        for (int i = 0; i < 100; i++)
            stimNums[i] = i;
        for (int i = 0; i < stimuli.Length; i++)
        {
            int index = UnityEngine.Random.Range(i, 100 - 1);
            int tmp = stimNums[index];
            stimNums[index] = stimNums[i];
            stimNums[i] = tmp;
        }
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
        
        //Actually creates the stimuli
        for (int i = 0; i < stimuli.Length; i++)
        {
            try
            {
                ////Creates the material to be used for the blobs
                stimuli[i] = Resources.Load("Blob" + stimNums[i]) as GameObject;
                stimuli[i].gameObject.transform.Find("Meshes").renderer.material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
                stimuli[i].gameObject.transform.Find("Meshes").renderer.material.mainTexture = colors[colorNums[i]];
                stimuli[i].gameObject.transform.Find("Meshes").renderer.material.color = new Color(stimuli[i].gameObject.transform.Find("Meshes").renderer.material.color.r, stimuli[i].gameObject.transform.Find("Meshes").renderer.material.color.g, stimuli[i].gameObject.transform.Find("Meshes").renderer.material.color.b, Transparency);
                //stimuli[i].gameObject.transform.Find("Meshes").tag = "Untagged";
                stimuli[i] = (GameObject)Instantiate(stimuli[i]);
            }
            catch (NullReferenceException) { Debug.Log("NullReferenceException while attempting to generate stimuli."); /* If there's a null reference exception it means we're missing some dependent object. This shouldn't happen, but as at least once. Keep an eye on it. */ }
        }
    }

    private int numRetries = 100;
    private void generateRandomPositions()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = numRetries;
        for (int i = 0; i < stimuli.Length; i++)
        {
            stimuli[i].transform.localPosition = new Vector3(
            UnityEngine.Random.Range(randomBoundsP0.x, randomBoundsP0.y),
            UnityEngine.Random.Range(randomBoundsP0.y, randomBoundsP1.y),
            UnityEngine.Random.Range(randomBoundsP0.z, randomBoundsP1.z));

            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimuli[i].transform.localPosition.x,
                                   stimuli[i].transform.localPosition.z,
                                   stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x * stimuli[i].transform.localScale.x,
                                   stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z * stimuli[i].transform.localScale.z);
            if (boxesOverlapArray(newBox, overlapCheckList))
            {
                retries--;
                if (retries <= 0)
                {
                    //Force quit and overlap to prevent hang
                    retries = numRetries;
                    overlapCheckList.Add(newBox);
                    Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
                }
                else i--;
            }
            else
            {
                retries = numRetries;
                overlapCheckList.Add(newBox);
            }
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
}