using UnityEngine;
using System.Collections;
using Leap;

//TO ADD
//1) Moving & Rotating - If the thumb and index of a single hand grab different colliders on the cube, then you can move and rotate - how to do?
//2) Using the Leap pinch gesture from the MagneticPinch script instead of colliders. Might be more stable?
// Would also make so you cant pinch and wipe your hand through an object to grab. We had the same issue with KinectGrab and solved it by detecting a grab and then looking for a rising edge.
//3) Generate random transparent cube within certain parameters.
//4) Add visual cue to let you know when you're touching a collider

public class Stretch : MonoBehaviour
{
    private bool fingertouch;
    private bool thumbtouch;
    private GameObject cube;
    private GameObject thumb;
    private Vector3 cubeCurrentPosition;
    private Vector3 cubeCurrentScale;
    private Vector3 thumbPreviousPosition;

    void Start()
    {
        fingertouch = false;
        thumbtouch = false;
        cube = GameObject.Find("StrechableCube");
    }

    //Whenever something starts colliding with the cube, checks if it's the thumb or the index finger.
    void OnCollisionEnter(Collision col)
    {
        //Checks the list of all the things colliding with the cube to see if it contains the index fingertip and the thumb fingertip
        foreach (ContactPoint contact in col.contacts)
        {
            //If the tip of the index finger is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "index")
            {
                Debug.Log("FingerTip Touching" + this.gameObject.name);
                fingertouch = true;
            }
            // If the thumb tip is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbtouch = true;
                //Defines the thumb game object. Doing it this way means it works with any hand
                thumb = contact.otherCollider.gameObject;
                Debug.Log("Thumb Touching" + this.gameObject.name);
                //Initializes a variable to simplify things later on in the script
                thumbPreviousPosition = thumb.transform.position;
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
        }
    }

    void Update()
    {
        //If the finger and thumb are grabbing
        if (fingertouch == true && thumbtouch == true)
        {
            //Defines the current position and scale of the cube
            Vector3 currentCubePosition = cube.transform.position;
            Vector3 currentCubeScale = cube.transform.localScale;
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
    }
}

