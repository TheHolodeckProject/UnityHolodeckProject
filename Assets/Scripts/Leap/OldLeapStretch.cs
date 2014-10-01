using UnityEngine;
using System.Collections;
using System;
using Leap;

//TO ADD
//1) Moving & Rotating - If the thumb and index of a single hand grab different colliders on the cube, then you can move and rotate - how to do?
//3) Generate random transparent cube within certain parameters.
//4) Add visual cue to let you know when you're touching a collider

public class OldLeapStretch : MonoBehaviour
{
    public float MinCubeSize = .05f;
    public float MaxCubeSize = .3f;
    public float CubeSizeBuffer = .005f;
    public bool fingertouch;
    public bool thumbtouch;
    private GameObject cube;
    private Vector3 cubeCurrentPosition;
    private Vector3 cubeCurrentScale;
    private Vector3 pinchPositionPrevious;
    public Vector3 thumbPosition;
    private Vector3 thumbDifference;
    public Vector3 indexPosition;
    //??? Is it more efficient to import the bool variable than the entire component/game object?
    //    private LeapDetectPinch rightPinch;
    //    private LeapDetectPinch leftPinch;
    private bool pinching;
    private bool wasPinching;
    public GameObject activeHand;
    private bool stretching;
    public bool isRight;

    void Start()
    {
        fingertouch = false;
        thumbtouch = false;
        cube = GameObject.Find("StrechableCube");
        pinching = false;
        activeHand = null;
        stretching = false;
        wasPinching = false;
        isRight = false;
    }

    //Whenever something starts colliding with the cube, checks if it's the thumb or the index finger.
    void OnCollisionEnter(Collision col)
    {
        //Checks the list of all the things colliding with the cube to see if it contains the index fingertip and the thumb fingertip
        foreach (ContactPoint contact in col.contacts)
        {
            // !!! Make this work with fingers
            //If the tip of the index finger is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "index")
                fingertouch = true;
            // If the thumb tip is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbtouch = true;
                // ??? This command is ridiculous. Is this ok?
                activeHand = contact.otherCollider.gameObject.transform.parent.parent.gameObject;

                //Debug.Log("activeHand is " + activeHand.name);
                //// !!! ADDED
                //if (string.Compare(activeHand.name,"RigidHandRight(Clone)") == 1)
                //    isRight = true;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        //Whenever something STOPS colliding with the cube, checks if it's the thumb or the index finger.
        foreach (ContactPoint contact in col.contacts)
        {
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "index")
                fingertouch = false;
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
                thumbtouch = false;
              //!!! ADDED
              isRight = false;
        }
    }

    void OnStretch()
    {
        Debug.Log("Starting stretch");
        stretching = true;
        //Gets pinch position from the LeapDetectPinch script
        pinchPositionPrevious = activeHand.GetComponent<LeapDetectPinch>().thumbPosition;
    }

    void WhileStretching()
    {
        //Defines the current position and scale of the cube
        cubeCurrentPosition = cube.transform.position;
        cubeCurrentScale = cube.transform.localScale;
        thumbPosition = activeHand.GetComponent<LeapDetectPinch>().thumbPosition;
        //Debug.Log("Stretch says thumbposition is " + thumbPosition);
        indexPosition = activeHand.GetComponent<LeapDetectPinch>().indexPosition;
        //If the cube has reached max size on any axis, quit stretching and make it a tiny bit smaller on that axis so the user can fix it
        if (cubeCurrentScale.x >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on X axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x - CubeSizeBuffer, cubeCurrentScale.y, cubeCurrentScale.z);
            stretching = false;
            return;
        }
        else if (cubeCurrentScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on X axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x + CubeSizeBuffer, cubeCurrentScale.y, cubeCurrentScale.z);
            stretching = false;
            return;
        }
        //Y axis size check
        if (cubeCurrentScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x, cubeCurrentScale.y - CubeSizeBuffer, cubeCurrentScale.z);
            stretching = false;
            return;
        }
        else if (cubeCurrentScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Y axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x, cubeCurrentScale.y + CubeSizeBuffer, cubeCurrentScale.z);
            stretching = false;
            return;
        }
        //Z axis size check
        if (cubeCurrentScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x, cubeCurrentScale.y, cubeCurrentScale.z - CubeSizeBuffer);
            stretching = false;
            return;
        }
        else if (cubeCurrentScale.z <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Z axis");
            cube.transform.localScale = new Vector3(cubeCurrentScale.x, cubeCurrentScale.y, cubeCurrentScale.z + CubeSizeBuffer);
            stretching = false;
            return;
        }
        //Defines how much the thumb has moved since the last frame. This position difference will be used to stretch the cube      
        thumbDifference = thumbPosition - pinchPositionPrevious;
        pinchPositionPrevious = thumbPosition;
        //Moves the cube's position based on how the thumb's moved
        cube.transform.position = new Vector3(cubeCurrentPosition.x - thumbDifference.x / 2, cubeCurrentPosition.y + thumbDifference.y / 2, cubeCurrentPosition.z + thumbDifference.z / 2);
        //Applies different transformations to scale depending on which corner is being grabbed
        //I'm sure there's a beautiful elegant way to do this, but this way seems to work for now
        switch (this.gameObject.transform.name)
        {
            case "CornerColliderFrontBottomLeft":
                    cube.transform.localScale = new Vector3(cubeCurrentScale.x - thumbDifference.x, cubeCurrentScale.y - thumbDifference.y, cubeCurrentScale.z + thumbDifference.z); 
                                        break;
            case "CornerColliderFrontBottomRight":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x + thumbDifference.x, cubeCurrentScale.y - thumbDifference.y, cubeCurrentScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopLeft":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x - thumbDifference.x, cubeCurrentScale.y + thumbDifference.y, cubeCurrentScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopRight":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x + thumbDifference.x, cubeCurrentScale.y + thumbDifference.y, cubeCurrentScale.z + thumbDifference.z);
                break;
            case "CornerColliderBackBottomLeft":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x - thumbDifference.x, cubeCurrentScale.y - thumbDifference.y, cubeCurrentScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackBottomRight":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x + thumbDifference.x, cubeCurrentScale.y - thumbDifference.y, cubeCurrentScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopLeft":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x - thumbDifference.x, cubeCurrentScale.y + thumbDifference.y, cubeCurrentScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopRight":
                cube.transform.localScale = new Vector3(cubeCurrentScale.x + thumbDifference.x, cubeCurrentScale.y + thumbDifference.y, cubeCurrentScale.z - thumbDifference.z);
                break;
        }
    }

    void ExitStretch()
    {
        stretching = false;
        Debug.Log("Stopped pinching. Releasing stretch");
    }

    void Update()
    {
        //If the fingers are hitting the collider, check for a pinch
        if (activeHand != null)
            //Checks if the hand is currently pinching
            pinching = activeHand.GetComponent<LeapDetectPinch>().pinching;
      
            //If a pinch was just started and the fingers are colliding
            if (wasPinching == false && pinching == true && fingertouch == true && thumbtouch == true)
                OnStretch();

            //If you stop pinching while stretching
            if (stretching == true && pinching == false)
                ExitStretch();

            if (stretching == true)
                WhileStretching();

            wasPinching = pinching;
    }
}