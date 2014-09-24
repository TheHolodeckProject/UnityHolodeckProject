using UnityEngine;
using System.Collections;
using Leap;

//THINGS TO TRY
//1) Using the Leap pinch gesture from the MagneticPinch script instead of colliders. Might be more stable?
// Would also make so you cant pinch and wipe your hand through an object to grab. We had the same issue with KinectGrab and solved it by detecting a grab and then looking for a rising edge.

//2) Instead of comparing to the position and scale when grabbed, try manipulating based on position and scale from the previous frame?

// !!! While stretching, the thumb can leave the area of the collider. Not sure why. 
public class Stretch : MonoBehaviour
{
    private bool fingertouch;
    private bool thumbtouch;
    private GameObject cube;
    private GameObject thumb;
    private Vector3 thumbCurrentPosition;
    private Vector3 thumbStartPosition;
    private Vector3 cubeStartScale;
    private Vector3 cubeStartPosition;

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
                //Grabs the position and scale of the thumb when it entered the object
                thumbStartPosition = thumb.transform.position;
                //Also grabs the scale and position of the cube when it entered the object
                cubeStartScale = cube.transform.localScale;
                cubeStartPosition = cube.transform.position;
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
            //Gets the position of the thumb
            Vector3 thumbCurrentPosition = thumb.transform.position;
            //Calculates differences between current position and starting position. This will be used to modify the cube's scale and position
            Vector3 difference = thumbCurrentPosition - thumbStartPosition;
            //Debug.Log("Cube Position: " + cube.transform.position);
            //Debug.Log("Cube Scale: " + cube.transform.localScale);
            //Debug.Log("Difference: " + difference);

            //Applies different transformations depending on which corner is being grabbed
            switch (this.gameObject.transform.name)
            {
                case "CornerColliderBackBottomLeft":
                    cube.transform.localScale = new Vector3(cubeStartScale.x + difference.x, cubeStartScale.y - difference.y, cubeStartScale.z);
                    cube.transform.position = new Vector3(cubeStartPosition.x + difference.x / 2, cubeStartPosition.y + difference.y / 2, cubeStartPosition.z + difference.z / 2);
                    break;
            }
        }
    }
}

