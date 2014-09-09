using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;

public class KinectGrab : MonoBehaviour
{
    //Initialize variables
    const float minGrabDist = 1.5f;
    private GameObject rhgObj = null;
    private GameObject lhgObj = null;
    private bool grabReadyRight = false;
    private bool grabReadyLeft = false;
    //ADDED - Made these public so they can be accessed by other scripts
    public bool grabStateRight = false;
    public bool grabStateLeft = false;
    private GameObject neck;
    private bool ranOnce;
    private Vector3 spinePosition;
    private Vector3 cubePosition;
    private bool previousResetCueState = false;
    private int state = 0;
    // Use this for initialization
    void Start()
    {
        GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>().SetSkipMouseRotation(true);
    }

    // Update is called once per frame
    void Update()
    {        
        //Get body objects (hands) and grabbable objects for manipulation
        BodySourceManager bodyManager = GameObject.Find("BodyManager").GetComponent<BodySourceManager>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Grabbable");
        GameObject leftHand = GameObject.Find("HandLeft");
        GameObject rightHand = GameObject.Find("HandRight");
        //Check to make sure the objects exist before continuing
        if (leftHand == null || rightHand == null) return;
        try
        {
            //Get the nearest objects to the body
            Body[] bodies = bodyManager.GetData();
            Body activeBody = null;
            /// ??? What's this doing? Is it saying "if there's more than one skeleton in the scene, use the first one"?
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].IsTracked)
                {
                    activeBody = bodies[i];
                }
            }

            //RIGHT HAND
            //Define the positions of the closest object to the right hand
            GameObject rhObj = getClosestObject(rightHand.transform.position, objs);
            //Figure out how far away the hand is from the closest object
            float rightDist = Vector3.Distance(rhObj.transform.position, rightHand.transform.position);
            //If the closest object IS NOT in the grabbable distance
            if (rightDist > minGrabDist)
            {
                grabReadyRight = false;
            }
            //If the closest object IS in the grabbable distance and the hand is open
            //CHANGED FROM ELSE IF TO IF
            if (activeBody.HandRightState == HandState.Open && activeBody.HandRightConfidence == TrackingConfidence.High)
            {
                //Establishes that it's possible to grab something now
                grabReadyRight = true;
            }
            // ??? You mentioned that boolean checks were super efficent to do in updates
            // ??? Would it be more efficient to do the boolean checks for grabReady and grabState in a separate if statement
            // ??? and only do the handstate checks if they're true?
            //If nothing is currently being grabbed, an object is ready to be grabbed, and the Kinect is confident the hand is closed
            if (grabStateRight == false && grabReadyRight == true && activeBody.HandRightState == HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High)
            {
                //Assign the closest object to the grabbed object variable
                rhgObj = rhObj;
                //Assign the grabbed object as a child of the right hand
                rhgObj.transform.parent = rightHand.transform;
                //Change the grab state to true
                grabStateRight = true;
            }
            //If you had been holding something but Kinect is confident your hand is no longer closed
            if (grabStateRight == true && activeBody.HandRightState != HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High)
            {
                //Make the grabbed object no longer be a child of the hand
                rhgObj.transform.parent = null;
                rhgObj = null;
                grabStateRight = false;
            }

            //LEFT HAND
            //Define the positions of the closest object to the right hand
            GameObject lhObj = getClosestObject(leftHand.transform.position, objs);
            //Figure out how far away the hand is from the closest object
            float leftDist = Vector3.Distance(lhObj.transform.position, leftHand.transform.position);
            //If the closest object IS NOT in the grabbable distance
            if (leftDist > minGrabDist)
            {
                grabReadyLeft = false;
            }
            //If the closest object IS in the grabbable distance and the hand is open
            if (activeBody.HandLeftState == HandState.Open && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Establishes that it's possible to grab something now
                grabReadyLeft = true;
            }
            //If nothing is currently being grabbed, an object is ready to be grabbed, and the Kinect is confident the hand is closed
            if (grabStateLeft == false && grabReadyLeft == true && activeBody.HandLeftState == HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Assign the closest object to the grabbed object variable
                lhgObj = lhObj;
                //Assign the grabbed object as a child of the right hand
                lhgObj.transform.parent = leftHand.transform;
                //Change the grab state to true
                grabStateLeft = true;
            }
            //If you had been holding something but Kinect is confident your hand is no longer closed
            if (grabStateLeft == true && activeBody.HandLeftState != HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Make the grabbed object no longer be a child of the hand
                lhgObj.transform.parent = null;
                lhgObj = null;
                grabStateLeft = false;
            }

            // ??? This happens in any state. The simplest thing would be to call the phase variable from the stimulus manager, but I seem to remember you saying that was a bad practice

            bool currentResetCueState = grabStateRight == false
                                     && grabStateLeft == false
                                     && activeBody.HandRightState == HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High
                                     && activeBody.HandLeftState == HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High;
            bool risingEdge = false;
            bool fallingEdge = false;
            bool high = false;
            bool low = false;
            if (previousResetCueState == false && currentResetCueState == true) risingEdge = true;
            else if (previousResetCueState == true && currentResetCueState == false) fallingEdge = true;
            else if (previousResetCueState == true && currentResetCueState == true) high = true;
            else if (previousResetCueState == false && currentResetCueState == false) low = true;
            //Stimulus Reset Cue
            if (state == 0 && risingEdge)
            {
                Debug.Log("Resetting Stimuli");
                GameObject.Find("StimulusManager").GetComponent<HolodeckStimulusManager>().HolodeckStimulusReset();
                state = 1;
            }
            
            // ??? Because it uses the same hand state (both closed, not grabbing anything), it keeps quickly switching between resetting stimulus and trial. How to fix?

            ////Trial Reset Cue
            else if(state == 1 && risingEdge)
            {
                Debug.Log("Resetting Trial");
                GameObject.Find("StimulusManager").GetComponent<HolodeckStimulusManager>().HolodeckTrialReset();
                state = 0;
            }
            //Debug.Log("Current State: " + (risingEdge ? "Rising" : fallingEdge ? "Falling" : high ? "High" : low ? "Low" : "Error"));
            previousResetCueState = currentResetCueState;
        }
        catch (NullReferenceException)
        {
            //Debug.Log("Skeleton has left the scene mid processing.");
        }
    }

    GameObject getClosestObject(Vector3 pos, GameObject[] objs)
    {
        //Initialize variables
        // ??? What's the rationale for using MaxValue? 
        float minDist = float.MaxValue;
        GameObject minObj = null;
        //For every grabbable object
        for (int i = 0; i < objs.Length; i++)
        {
            //Figure out how far away it is
           float dist = Vector3.Distance(objs[i].transform.position, pos);
            //If that object's distance is smaller than the current smallest distance
            if (dist < minDist)
            {
                //Assign that distance as the new smallest distance
                minDist = dist;
                //Assign that object as the closest object
                minObj = objs[i];
            }
        }
        return minObj;
    }
}