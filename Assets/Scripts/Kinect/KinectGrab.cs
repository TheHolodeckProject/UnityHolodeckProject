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
    private bool grabReadyRight;
    private bool grabReadyLeft;
    //ADDED - Made these public so they can be accessed by other scripts
    private bool grabStateRight;
    private bool grabStateLeft;
    private GameObject neck;
    private bool ranOnce;
    private Vector3 spinePosition;
    private Vector3 cubePosition;

    /******************TOUCH CODE******************************/
    private float rhInitialTouchTime;
    private float lhInitialTouchTime;
    private GameObject lhPreviousClosestGameObject;
    private GameObject rhPreviousClosestGameObject;
    private bool rhTouching;
    private bool lhTouching;
    public float touchTimeRequirement = 1000f; //in ms
    public float touchDistanceThreshold = 1f;
    /**********************************************************/

    private HolodeckStateMachine stateMachine;
    // Use this for initialization
    void Start()
    {
        stateMachine = this.gameObject.GetComponent<HolodeckStateMachine>();
        grabStateRight = false;
        grabStateLeft = false;
        grabReadyRight = false;
        grabReadyLeft = false;
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
            int activeBodies = 0;
            /// ??? What's this doing? Is it saying "if there's more than one skeleton in the scene, use the first one"?
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].IsTracked)
                {
                    activeBody = bodies[i];
                    activeBodies++;
                }
            }

            //RIGHT HAND
            //Define the positions of the closest object to the right hand
            GameObject rhObj = getClosestObject(rightHand.transform.position, objs);
            //Figure out how far away the hand is from the closest object
            float rightDist = Vector3.Distance(rhObj.transform.position, rightHand.transform.position);

            if (activeBody.HandRightState == HandState.Open && activeBody.HandRightConfidence == TrackingConfidence.High)
                grabReadyRight = true;
            //If nothing is currently being grabbed, an object is ready to be grabbed, and the Kinect is confident the hand is closed
            //if (rightDist < minGrabDist && grabStateRight == false && grabReadyRight == true && activeBody.HandRightState == HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High)
            if(!grabStateRight && activeBody.HandRightState == HandState.Closed)
            {
                Debug.Log("Grabbed!");
                //Assign the closest object to the grabbed object variable
                rhgObj = rhObj;
                //Assign the grabbed object as a child of the right hand
                rhgObj.transform.parent = rightHand.transform;
                //Notify the state machine that this object was "moved"
                //stateMachine.ObjectMoved(rhgObj);
                //Change the grab state to true
                grabStateRight = true;
                //grabReadyRight = false;
            }
            //If you had been holding something but Kinect is confident your hand is no longer closed
            else if (grabStateRight == true && activeBody.HandRightState != HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High)
            {
                Debug.Log("Ungrabbed!");
                //Make the grabbed object no longer be a child of the hand
                rhgObj.transform.parent = null;
                rhgObj = null;
                grabStateRight = false;
            }

            //LEFT HAND
            /*//Define the positions of the closest object to the right hand
            GameObject lhObj = getClosestObject(leftHand.transform.position, objs);
            //Figure out how far away the hand is from the closest object
            float leftDist = Vector3.Distance(lhObj.transform.position, leftHand.transform.position);
            //If the closest object IS NOT in the grabbable distance
            if (leftDist > minGrabDist)
            {
                grabReadyLeft = false;
            }
            //If the closest object IS in the grabbable distance and the hand is open
            else if (activeBody.HandLeftState == HandState.Open && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Establishes that it's possible to grab something now
                grabReadyLeft = true;
            }
            //If nothing is currently being grabbed, an object is ready to be grabbed, and the Kinect is confident the hand is closed
            else if (grabStateLeft == false && grabReadyLeft == true && activeBody.HandLeftState == HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Assign the closest object to the grabbed object variable
                lhgObj = lhObj;
                //Assign the grabbed object as a child of the right hand
                lhgObj.transform.parent = leftHand.transform;
                //Notify the state machine that this object was "moved"
                stateMachine.ObjectMoved(rhgObj);
                //Change the grab state to true
                grabStateLeft = true;
                grabReadyLeft = false;
            }
            //If you had been holding something but Kinect is confident your hand is no longer closed
            if (grabStateLeft == true && activeBody.HandLeftState != HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                //Make the grabbed object no longer be a child of the hand
                lhgObj.transform.parent = null;
                lhgObj = null;
                grabStateLeft = false;
            }*/
            //Define the positions of the closest object to the right hand
            GameObject lhObj = getClosestObject(leftHand.transform.position, objs);
            //Figure out how far away the hand is from the closest object
            float leftDist = Vector3.Distance(lhObj.transform.position, leftHand.transform.position);

            if (activeBody.HandLeftState == HandState.Open && activeBody.HandLeftConfidence == TrackingConfidence.High)
                grabReadyLeft = true;
            //If nothing is currently being grabbed, an object is ready to be grabbed, and the Kinect is confident the hand is closed
            //if (rightDist < minGrabDist && grabStateRight == false && grabReadyRight == true && activeBody.HandRightState == HandState.Closed && activeBody.HandRightConfidence == TrackingConfidence.High)
            if (!grabStateLeft && activeBody.HandLeftState == HandState.Closed)
            {
                Debug.Log("Grabbed!");
                //Assign the closest object to the grabbed object variable
                lhgObj = lhObj;
                //Assign the grabbed object as a child of the right hand
                lhgObj.transform.parent = leftHand.transform;
                //Notify the state machine that this object was "moved"
                //stateMachine.ObjectMoved(rhgObj);
                //Change the grab state to true
                grabStateLeft = true;
                //grabReadyRight = false;
            }
            //If you had been holding something but Kinect is confident your hand is no longer closed
            else if (grabStateLeft == true && activeBody.HandLeftState != HandState.Closed && activeBody.HandLeftConfidence == TrackingConfidence.High)
            {
                Debug.Log("Ungrabbed!");
                //Make the grabbed object no longer be a child of the hand
                lhgObj.transform.parent = null;
                lhgObj = null;
                grabStateLeft = false;
            }
            /******************TOUCH CODE******************************/
            //Right Hand
            bool previousRhTouchState = rhTouching;
            if (rightDist < touchDistanceThreshold)
            {
                if (rhPreviousClosestGameObject == null || rhPreviousClosestGameObject != rhgObj)
                {
                    rhPreviousClosestGameObject = rhgObj;
                    rhInitialTouchTime = Time.time;
                }
                if (Time.time - rhInitialTouchTime > touchTimeRequirement)
                {
                    rhTouching = true;
                }
            }
            else
            {
                rhPreviousClosestGameObject = null;
                rhTouching = false;
            }

            if (previousRhTouchState == false && rhTouching == true) stateMachine.ObjectTouched(rhgObj);

            //Left Hand
            bool previousLhTouchState = lhTouching;
            if (leftDist < touchDistanceThreshold)
            {
                if (lhPreviousClosestGameObject == null || lhPreviousClosestGameObject != lhgObj)
                {
                    lhPreviousClosestGameObject = lhgObj;
                    lhInitialTouchTime = Time.time;
                }
                if (Time.time - lhInitialTouchTime > touchTimeRequirement)
                {
                    lhTouching = true;
                }
            }
            else
            {
                lhPreviousClosestGameObject = null;
                lhTouching = false;
            }

            if (previousLhTouchState == false && lhTouching == true) stateMachine.ObjectTouched(lhgObj);
            /**********************************************************/
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