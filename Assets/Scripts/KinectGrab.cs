using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;

public class KinectGrab : MonoBehaviour
{
    public GameObject cube;
    public Vector3 StimOffset;
    private GameObject neck;
    private bool ranOnce;
    private Vector3 spinePosition;
    private Vector3 cubePosition;

    /// Radius of drawn hand circles
    private const double HandSize = 30;

    // Use this for initialization
    void Start()
    {
        ranOnce = false;
        GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>().SetSkipMouseRotation(true);
    }

    // Update is called once per frame
    void Update()
    {
        //If the stimulus hasn't been generated
        if (!ranOnce)
        {
            //If the kinect-generated body exists
            if (GameObject.Find("SpineMid") != null)
            {
                //Grabs the position of the spine
                GameObject spine = GameObject.Find("SpineMid");
                //Debug.Log("Spine Position: " + spine.transform.position);
                //Defines the position of the cube relative to the player
                cubePosition = new Vector3(spine.transform.position.x + 5, spine.transform.position.y, spine.transform.position.z - 13);
                //Creates the cube and puts it at that position 
                //Instantiate(cube, cubePosition, Quaternion.identity);
                //Debug.Log("Cube Position: " + cubePosition);
                //Changes the flag to true so it doesn't run again
                ranOnce = true;
            }
        }
        detectGrabs();
    }


    const float minGrabDist = 3.0f;
    private GameObject rhgObj = null;
    private GameObject lhgObj = null;

    void detectGrabs()
    {
        //Get body objects (hands) and grabbable objects for manipulation
        BodySourceManager bodyManager = GameObject.Find("BodyManager").GetComponent<BodySourceManager>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Grabbable"); ;
        GameObject leftHand = GameObject.Find("HandLeft");
        GameObject rightHand = GameObject.Find("HandRight");
        //Check to make sure the objects exist before continuing
        if (leftHand == null || rightHand == null) return;
        try
        {
            //Get the nearest objects to the body
            Body[] bodies = bodyManager.GetData();
            Body activeBody = null;
            for (int i = 0; i < bodies.Length; i++)
                if (bodies[i].IsTracked)
                {
                    activeBody = bodies[i];
                }
            GameObject lhObj = getClosestObject(leftHand.transform.position, objs);
            GameObject rhObj = getClosestObject(rightHand.transform.position, objs);

            //Check the hand state to activate a grab
            //Changes in the condition need to be made so that instead of just checking the state, we determine if there was a "rising edge" (going from open to closed where the new closed state has high confidence)
            Debug.Log(activeBody.HandLeftConfidence+"");
            if (activeBody.HandLeftState == HandState.Closed)
            {

                float dist = Vector3.Distance(lhObj.transform.position, leftHand.transform.position);
                Debug.Log("Left Hand Closed " + dist);
                if (dist < minGrabDist)
                {
                    lhgObj = lhObj;
                    //ADDED
                    Debug.Log("Closing hand on object");
                    lhgObj.transform.parent = leftHand.transform;
                }
            }
            else if (activeBody.HandLeftState != HandState.Closed)
            {
                if (lhgObj != null)
                {
                    lhgObj.transform.parent = null;
                    lhgObj = null;
                }
            }
            if (activeBody.HandRightState == HandState.Closed)
            {
                Debug.Log("Right Hand Closed");
                if (Vector3.Distance(rhObj.transform.position, rightHand.transform.position) < minGrabDist)
                {
                    rhgObj = rhObj;
                    rhgObj.transform.parent = rightHand.transform;
                }
            }
            else if (activeBody.HandRightState != HandState.Closed)
            {
                if (rhgObj != null)
                {
                    rhgObj.transform.parent = null;
                    rhgObj = null;
                }
            }
        }catch(NullReferenceException){Debug.Log("Skeleton has left the scene mid processing.");}
    }

    GameObject getClosestObject(Vector3 pos, GameObject[] objs)
    {
        float minDist = float.MaxValue;
        GameObject minObj = null;
        for (int i = 0; i < objs.Length; i++)
        {
            float dist = Vector3.Distance(objs[i].transform.position, pos);
            if (dist < minDist)
            {
                minDist = dist;
                minObj = objs[i];
            }
        }
        return minObj;
    }
}