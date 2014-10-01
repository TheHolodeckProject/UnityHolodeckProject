using UnityEngine;
using System.Collections;
using Leap;

public class PositionLeapMotion : MonoBehaviour {

    //private GameObject oculuscamera;
    private GameObject rightcamera;
    private Quaternion camrotation;
//    private bool leftHandLocked = false;
//    private bool rightHandLocked = false;

	// Use this for initialization
	void Start () {
       // oculuscamera = GameObject.Find("OVRCameraController");
        rightcamera = GameObject.Find("OVRCameraController/CameraRight");
    }
	
	// Update is called once per frame
	void Update () {
        /*GameObject leftHand = GameObject.Find("HandLeft");
        GameObject rightHand = GameObject.Find("HandRight");
        GameObject leftRiggedHand = GameObject.Find("RiggedLeftHand(Clone)");
        GameObject rightRiggedHand = GameObject.Find("RiggedRightHand(Clone)");
        GameObject leftRigidHand = GameObject.Find("RigidHandLeft(Clone)");
        GameObject rightRigidHand = GameObject.Find("RigidHandRight(Clone)");

        if (leftHand != null && leftRiggedHand != null && leftRigidHand != null && !leftHandLocked)
        {
            leftRiggedHand.transform.parent = leftHand.transform;
            leftRigidHand.transform.parent = leftHand.transform;
            leftHandLocked = true;
        }
        else
            leftHandLocked = false;

        if (rightHand != null && rightRiggedHand != null && rightRigidHand != null && !rightHandLocked)
        {
            rightRiggedHand.transform.parent = rightHand.transform;
            rightRigidHand.transform.parent = rightHand.transform;
            rightHandLocked = true;
        }
        else
            rightHandLocked = false;
         */
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

        // Have to decide whether to just stick the hands onto the Kinect wrists or try to get them to actually line up
        //If the right hand exists
        if (GameObject.Find("RiggedRightHand(Clone)") != null)
        {
        }

	}
}
