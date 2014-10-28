using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;
using System.Collections.Generic;

public class KinectGrab : MonoBehaviour
{
    //Initialize variables
    public float minGrabDist = 1.5f;
    private GameObject rhObj = null;
    private GameObject lhObj = null;
    private GameObject leftHand;
    private GameObject rightHand;
    private GameObject[] objs;
    private Body activeBody;
    enum State { Idle, ReadyToGrab, OnGrab, Grabbing, Release };
    private State rightState;
    private State leftState;

    // Use this for initialization
    void Start()
    {
        rightState = State.Idle;
        leftState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Right State: " + rightState);
        GetKinectInfo();
        //Only run the rest of the update function if there's an active body in the scene
        if (activeBody != null)
        {
            GetObjectInfo();
            //RIGHT HAND
            switch (rightState)
            {
                case State.Idle:
                    rightState = CheckHandOpen(rightHand, true);
                    break;

                case State.ReadyToGrab:
                    rightState = DetectGrab(rightHand, true);
                    break;

                case State.Grabbing:
                    rightState = CheckRelease(rightHand, true);
                    break;

                case State.Release:
                    rightState = OnRelease(rightHand, true);
                    break;
            }
        }
    }

    GameObject getClosestObject(Vector3 pos, GameObject[] objs)
    {
        //Initialize variables
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

    void GetKinectInfo()
    {
        activeBody = null;
        leftHand = GameObject.Find("HandLeft");
        rightHand = GameObject.Find("HandRight");
        //Check to make sure the objects exist before continuing
        if (leftHand == null || rightHand == null) return;
        {
            BodySourceManager bodyManager = GameObject.Find("BodyManager").GetComponent<BodySourceManager>();
            //Get the nearest objects to the body
            Body[] bodies = bodyManager.GetData();
            int activeBodies = 0;
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].IsTracked)
                {
                    activeBody = bodies[i];
                    activeBodies++;
                }
            }
        }
    }
    void GetObjectInfo()
    {
        List<Stimulus> output = new List<Stimulus>();
        HelperFunctions.GetScriptObjectsInScene<Stimulus>(out output);
        objs = new GameObject[output.Count];
        for (int i = 0; i < objs.Length; i++)
            objs[i] = output[i].gameObject;
    }
    
    State CheckHandOpen(GameObject hand, bool isRightHand)
    {
        //Depending on which hand it's checking, checks the appropriate handstate of the Kinect skeleton
        if (isRightHand)
        {
            //If the hand is open
            if (activeBody.HandRightState == HandState.Open) return State.ReadyToGrab;
        }
        else
        {
            //Do Nothing
        }

        return State.Idle;
    }

    State DetectGrab(GameObject hand, bool isRightHand)
    {
        //Depending on which hand it's checking, checks the appropriate handstate of the Kinect skeleton
        if (isRightHand)
        {
            //If the hand is closed
            if (activeBody.HandRightState == HandState.Closed)
            {
                //Defines the position of the closest object to the right hand
                rhObj = getClosestObject(rightHand.transform.position, objs);
                //Figure out how far away the hand is from the closest object
                float rightDist = Vector3.Distance(rhObj.transform.position, rightHand.transform.position);
                if (rightDist < minGrabDist)
                {
                    rhObj.transform.parent = rightHand.transform;
                    return State.Grabbing;
                }
            }
            else return State.ReadyToGrab;
        }
        else
        {
            //Do Nothing
        }

        return State.Idle;
    }

    State CheckRelease(GameObject hand, bool isRightHand)
    {
        if (isRightHand)
        {
            if (activeBody.HandRightState == HandState.Open && activeBody.HandRightConfidence == TrackingConfidence.High)
                return State.Release;
        }
        else
        {
            //Do Nothing
        }

        return State.Grabbing;
    }

    State OnRelease(GameObject hand, bool isRightHand)
    {
        //Depending on which hand it's checking, checks the appropriate handstate of the Kinect skeleton
        if (isRightHand)
        {
            //Make the grabbed object no longer be a child of the hand
            rhObj.transform.parent = null;
        }
        else
        {
            //Do Nothing
        }

        return State.Idle;
    }
}

    
    ///******************TOUCH CODE - Start *****************************/
    //private float rhInitialTouchTime;
    //private float lhInitialTouchTime;
    //private GameObject lhPreviousClosestGameObject = null;
    //private GameObject rhPreviousClosestGameObject = null;
    //private bool rhTouching;
    //private bool lhTouching;
    //public float touchTimeRequirement = 1000f; //in ms
    //public float touchDistanceThreshold = 5f;
    /**********************************************************/

        // /******************TOUCH CODE - Update******************************/
        //    rhObj = getClosestObject(rightHand.transform.position, objs);
        //    lhObj = getClosestObject(leftHand.transform.position, objs);
        //    leftDist = Vector3.Distance(lhObj.transform.position, leftHand.transform.position);
        //    rightDist = Vector3.Distance(rhObj.transform.position, rightHand.transform.position);
        //    //Right Hand
        //    bool previousRhTouchState = rhTouching;
        //    //Debug.Log("dist=" + rightDist + " , " + touchDistanceThreshold);
        //    if (rightDist < touchDistanceThreshold)
        //    {
        //        if (rhPreviousClosestGameObject == null || rhPreviousClosestGameObject != rhObj)
        //        {
        //            Debug.Log("first r ");
        //            rhPreviousClosestGameObject = rhObj;
        //            rhInitialTouchTime = Time.time;
        //        }
        //        if (Time.time - rhInitialTouchTime > touchTimeRequirement)
        //        {
        //            Debug.Log("touch r");
        //            rhTouching = true;
        //        }
        //    }
        //    else
        //    {
        //        rhPreviousClosestGameObject = null;
        //        rhTouching = false;
        //    }

        //    if (previousRhTouchState == false && rhTouching == true) stateMachine.ObjectTouched(rhObj);

        //    //Left Hand
        //    bool previousLhTouchState = lhTouching;
        //    if (leftDist < touchDistanceThreshold)
        //    {
        //        Debug.Log("Left close");
        //        if (lhPreviousClosestGameObject == null || lhPreviousClosestGameObject != lhObj)
        //        {
        //            lhPreviousClosestGameObject = lhObj;
        //            lhInitialTouchTime = Time.time;
        //        }
        //        if (Time.time - lhInitialTouchTime > touchTimeRequirement)
        //        {
        //            lhTouching = true;
        //            Debug.Log("touch l");
        //        }
        //    }
        //    else
        //    {
        //        lhPreviousClosestGameObject = null;
        //        lhTouching = false;
        //    }

        //    if (previousLhTouchState == false && lhTouching == true) stateMachine.ObjectTouched(lhObj);
        //    /**********************************************************/
        //}
        //catch (NullReferenceException)
        //    //Debug.Log("Skeleton has left the scene mid processing.");