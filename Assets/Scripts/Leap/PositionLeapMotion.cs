using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;
using System;

public class PositionLeapMotion : MonoBehaviour {

    //private GameObject oculuscamera;
    private GameObject rightcamera;
    private Quaternion camrotation;
    private bool leftHandLocked = false;
    private bool rightHandLocked = false;

	// Use this for initialization
	void Start () {
       // oculuscamera = GameObject.Find("OVRCameraController");
        rightcamera = GameObject.Find("OVRCameraController/CameraRight");
    }
	
	// Update is called once per frame
	void Update () {
        GameObject leftHand = GameObject.Find("HandLeft");
        GameObject rightHand = GameObject.Find("HandRight");
        List<GameObject> leftHandsLeap = new List<GameObject>();
        List<GameObject> rightHandsLeap = new List<GameObject>();
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (gameObj.name == "LeftHandClone(Clone)")
            {
                leftHandsLeap.Add(gameObj);
            }
            if (gameObj.name == "RightHandClone(Clone)")
            {
                rightHandsLeap.Add(gameObj);
            }
        }
        if (leftHand != null && leftHandsLeap.Count > 0 && !leftHandLocked)
        {
            foreach (GameObject o in leftHandsLeap)
            {
                try
                {
                    o.GetComponent<RigidHand>().positionOverride = leftHand.transform.position;
                    o.GetComponent<RigidHand>().overridePosition = true;
                }
                catch (Exception) { }
                try
                {
                    o.GetComponent<RiggedHand>().positionOverride = leftHand.transform.position;
                    o.GetComponent<RiggedHand>().overridePosition = true;
                }
                catch (Exception) { }
            }
            leftHandLocked = true;
        }
        else
            leftHandLocked = false;

        if (rightHand != null && rightHandsLeap.Count > 0 && !rightHandLocked)
        {
            foreach (GameObject o in rightHandsLeap)
            {
                try
                {
                    o.GetComponent<RigidHand>().positionOverride = rightHand.transform.position;
                    o.GetComponent<RigidHand>().overridePosition = true;
                }
                catch (Exception) { }
                try{
                    o.GetComponent<RiggedHand>().positionOverride = rightHand.transform.position;
                    o.GetComponent<RiggedHand>().overridePosition = true;
                }
                catch (Exception) { }
            }
            rightHandLocked = true;
        }
        else
            rightHandLocked = false;
         
        //Changes the position of the HandController to match the OVRCameraController gameobject
        //this.transform.position = oculuscamera.transform.position;
        //Gets the rotation of the actual camera for the right eye, which is a child of OVRCameraController
        //Unchecked "Tracker Rotates Y" on the OVRCameraController object, as that made it so Y rotation only
        //showed up in the OVRCameraController object, not the rightcamera. This way we don't have to get Y
        //rotation from a separate object
        camrotation = rightcamera.transform.rotation;
        //Applies a weird transformation to get it to face the right way
        camrotation *= Quaternion.Euler(270, 0, -180);
        this.transform.rotation = camrotation;
	}
}
