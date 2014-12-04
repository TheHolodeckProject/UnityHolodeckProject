using UnityEngine;
using System.Collections;
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
        GameObject kinectLeftHand = GameObject.Find("HandLeft");
        GameObject kinectRightHand = GameObject.Find("HandRight");
        GameObject leapLeftHand = GameObject.Find("RiggedLeftHand(Clone)");
        GameObject leapRightHand = GameObject.Find("RiggedRightHand(Clone)");
      
        if (leapLeftHand != null && kinectLeftHand != null && !leftHandLocked)
        {
            //!!! Had to comment out after Leap Motion update
            //leapLeftHand.GetComponent<RiggedHand>().positionOverride = kinectLeftHand.transform.position;
            //leapLeftHand.GetComponent<RiggedHand>().overridePosition = true;
            leftHandLocked = true;
        }
         else leftHandLocked = false;

        if (leapRightHand != null && kinectRightHand != null && !rightHandLocked)
        {
            //leapRightHand.GetComponent<RiggedHand>().positionOverride = kinectRightHand.transform.position;
            //leapRightHand.GetComponent<RiggedHand>().overridePosition = true;
            rightHandLocked = true;
        }
        else rightHandLocked = false;
       
            
         
        //Changes the position of the HandController to match the OVRCameraController gameobject
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
