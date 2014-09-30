using UnityEngine;
using System.Collections;
using Leap;

// !!! Sometimes, while pinching, the position of thumb3 will increase or decrease seemingly randomly
// Maybe try getting thumb position from the LeapDetectPinch script?

//TO ADD
// MINIMUM CUBE SIZE
//1) Moving & Rotating - If the thumb and index of a single hand grab different colliders on the cube, then you can move and rotate - how to do?
//3) Generate random transparent cube within certain parameters.
//4) Add visual cue to let you know when you're touching a collider

public class Stretch : MonoBehaviour
{
    public float MinCubeSize = .05f;
    public float MaxCubeSize = .3f;
    private bool fingertouch;
    private bool thumbtouch;
    private GameObject cube;
    private GameObject thumb;
    private Vector3 cubeCurrentPosition;
    private Vector3 cubeCurrentScale;
    private Vector3 thumbPreviousPosition;
    //??? Is it more efficient to import the bool variable than the entire component/game object?
    //    private LeapDetectPinch rightPinch;
    //    private LeapDetectPinch leftPinch;
    private bool rightPinch;
    private bool leftPinch;
    private bool triggerStretch;
    private bool pinching;
    private GameObject stretchHand;
    private bool prevPinching;
    private bool stretching;
    private bool prevStretching;
    private bool triggerPinch;

    void Start()
    {
        fingertouch = false;
        thumbtouch = false;
        cube = GameObject.Find("StrechableCube");
        rightPinch = false;
        leftPinch = false;
        triggerStretch = false;
        pinching = false;
        stretchHand = null;
        prevPinching = false;
        stretching = false;
        prevStretching = false;
        triggerPinch = false;
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
            {
                // Debug.Log("FingerTip Touching" + this.gameObject.name);
                fingertouch = true;
            }
            // If the thumb tip is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbtouch = true;
                //Defines the thumb game object. Doing it this way means it works with any hand
                thumb = contact.otherCollider.gameObject;
                //  Debug.Log("Thumb Touching" + this.gameObject.name);
                // ??? This command is ridiculous. Is this ok?
                stretchHand = contact.otherCollider.gameObject.transform.parent.parent.gameObject; 
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
            {
                thumbtouch = false;
                stretchHand = null;
            }
     
        }
    }

    void OnStretch()
    {
        Debug.Log("Starting stretch");
        stretching = true;
        pinching = true;
        // ??? Why does adding Vector3 to the start of the next line cause an error?
        thumbPreviousPosition = thumb.transform.position;
    }

    void WhileStretching()
    {
        //Defines the current position and scale of the cube
        Vector3 currentCubePosition = cube.transform.position;
        Vector3 currentCubeScale = cube.transform.localScale;

                        //If the cube has reached max size on any axis, quit stretching and make it a tiny bit smaller on that axis so the user can fix it
        if (currentCubeScale.x >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on X axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x - .001f, currentCubeScale.y, currentCubeScale.z);
            stretching = false;
            return;
        }
        else if (currentCubeScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on X axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x + .001f, currentCubeScale.y, currentCubeScale.z);
            stretching = false;
            return;
        }
        //Y axis size check
        if (currentCubeScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x, currentCubeScale.y - .001f, currentCubeScale.z);
            stretching = false;
            return;
        }
        else if (currentCubeScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Y axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x, currentCubeScale.y + .001f, currentCubeScale.z);
            stretching = false;
            return;
        }
        //Z axis size check
        if (currentCubeScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x, currentCubeScale.y, currentCubeScale.z - .001f);
            stretching = false;
            return;
        }
        else if (currentCubeScale.z <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Z axis");
            cube.transform.localScale = new Vector3(currentCubeScale.x, currentCubeScale.y, currentCubeScale.z + .001f);
            stretching = false;
            return;
        }

        //Defines how much the thumb has moved since the last frame. This position difference will be used to stretch the cube      
        Vector3 thumbDifference = thumb.transform.position - thumbPreviousPosition;
        // ??? Why does adding Vector3 to the start of the next line cause an error?
        thumbPreviousPosition = thumb.transform.position;
      //Moves the cube's position based on how the thumb's moved
        cube.transform.position = new Vector3(currentCubePosition.x + thumbDifference.x / 2, currentCubePosition.y + thumbDifference.y / 2, currentCubePosition.z + thumbDifference.z / 2);
        //Applies different transformations to scale depending on which corner is being grabbed
        //I'm sure there's a beautiful elegant way to do this, but this way seems to work for now
        switch (this.gameObject.transform.name)
        {
            case "CornerColliderFrontBottomLeft":
                    cube.transform.localScale = new Vector3(currentCubeScale.x + thumbDifference.x, currentCubeScale.y - thumbDifference.y, currentCubeScale.z + thumbDifference.z); 
                                        break;
            case "CornerColliderFrontBottomRight":
                cube.transform.localScale = new Vector3(currentCubeScale.x - thumbDifference.x, currentCubeScale.y - thumbDifference.y, currentCubeScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopLeft":
                cube.transform.localScale = new Vector3(currentCubeScale.x + thumbDifference.x, currentCubeScale.y + thumbDifference.y, currentCubeScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopRight":
                cube.transform.localScale = new Vector3(currentCubeScale.x - thumbDifference.x, currentCubeScale.y + thumbDifference.y, currentCubeScale.z + thumbDifference.z);
                break;
            case "CornerColliderBackBottomLeft":
                cube.transform.localScale = new Vector3(currentCubeScale.x + thumbDifference.x, currentCubeScale.y - thumbDifference.y, currentCubeScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackBottomRight":
                cube.transform.localScale = new Vector3(currentCubeScale.x - thumbDifference.x, currentCubeScale.y - thumbDifference.y, currentCubeScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopLeft":
                cube.transform.localScale = new Vector3(currentCubeScale.x + thumbDifference.x, currentCubeScale.y + thumbDifference.y, currentCubeScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopRight":
                cube.transform.localScale = new Vector3(currentCubeScale.x - thumbDifference.x, currentCubeScale.y + thumbDifference.y, currentCubeScale.z - thumbDifference.z);
                break;
        }
    }

    void ExitStretch()
    {
        stretching = false;
        Debug.Log("Stopped pinching. Releasing stretch");
    }

    void FixedUpdate()
    {
        //If the fingers are hitting the collider, check for a pinch
        if (stretchHand != null)
            //Checks if the hand is currently pinching
            pinching = stretchHand.GetComponent<LeapDetectPinch>().pinching;
      
            //If a pinch was just started and the fingers are colliding
            if (prevPinching == false && pinching == true && fingertouch == true && thumbtouch == true)
                OnStretch();

            //If you stop pinching while stretching
            if (stretching == true && pinching == false)
                ExitStretch();

            if (stretching == true)
                WhileStretching();


        //    //If the finger and thumb are grabbing
        //    if ()
        //    {
        //                  triggerStretch = true;
        //        stretching = true;


        //    }

        //    //On rising edge
        //    if (triggerStretch == true)
        //    {

        //        stretching = true;
        //        triggerStretch = false;
        //    }

        //    //On falling edge
        //    if (prevStretching == true & stretching == false)
        //        if (fingertouch == false)
        //            Debug.Log("Finger stopped touching");
        //        else if (thumbtouch == false)
        //            Debug.Log("Thumb stupped touching");
        //        else if (pinching == false)
        //            Debug.Log("No longer pinching");

        //    if (stretching == true)
        //    {

            prevPinching = pinching;
        //    prevStretching = stretching;
        //}
    }
}