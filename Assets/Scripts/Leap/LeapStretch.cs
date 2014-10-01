using UnityEngine;
using System.Collections;
using Leap;

//TO ADD
//1) Moving & Rotating - If the thumb and index of a single hand grab different colliders on the cube, then you can move and rotate - how to do?
//3) Generate random transparent cube within certain parameters.
//4) Add visual cue to let you know when you're touching a collider

public class LeapStretch : MonoBehaviour
{
    public float MinCubeSize = .05f;
    public float MaxCubeSize = .3f;
    public float CubeSizeBuffer = .005f;
    public bool fingertouch;
    public bool thumbtouch;
    private Vector3 cubeCurrentPosition;
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

    // !!! ADDED
        private string stretchPoint;
        private GameObject indexTouchObject;

    void Start()
    {
        fingertouch = false;
        thumbtouch = false;
        pinching = false;
        activeHand = null;
        stretching = false;
        wasPinching = false;
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
                stretchPoint = contact.thisCollider.gameObject.name;
                // ??? This command is ridiculous. Is this ok?
                activeHand = contact.otherCollider.gameObject.transform.parent.parent.gameObject; 
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

    void OnStretch()
    {
      //  Debug.Log("Starting stretch");
        stretching = true;
        //Gets pinch position from the LeapDetectPinch script
        pinchPositionPrevious = activeHand.GetComponent<LeapDetectPinch>().thumbPosition;
    }
    void WhileStretching()
    {
   thumbPosition = activeHand.GetComponent<LeapDetectPinch>().thumbPosition;
   if (this.transform.localScale.x >= MaxCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too big on X axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x - CubeSizeBuffer, this.transform.localScale.y, this.transform.localScale.z);
       stretching = false;
       return;
   }
   else if (this.transform.localScale.x <= MinCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too small on X axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x + CubeSizeBuffer, this.transform.localScale.y, this.transform.localScale.z);
       stretching = false;
       return;
   }
   //Y axis size check
   if (this.transform.localScale.y >= MaxCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too big on Y axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y - CubeSizeBuffer, this.transform.localScale.z);
       stretching = false;
       return;
   }
   else if (this.transform.localScale.x <= MinCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too small on Y axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y + CubeSizeBuffer, this.transform.localScale.z);
       stretching = false;
       return;
   }
   //Z axis size check
   if (this.transform.localScale.y >= MaxCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too big on Y axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z - CubeSizeBuffer);
       stretching = false;
       return;
   }
   else if (this.transform.localScale.z <= MinCubeSize)
   {
       // !!! INSERT VIBROBUMP
       Debug.Log("Cube is too small on Z axis");
       this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z + CubeSizeBuffer);
       stretching = false;
       return;
   }
        //Defines how much the thumb has moved since the last frame. This position difference will be used to stretch the cube      
        thumbDifference = thumbPosition - pinchPositionPrevious;
        pinchPositionPrevious = thumbPosition;
        //Moves the cube's position based on how the thumb's moved
        this.transform.position = new Vector3(this.transform.position.x - thumbDifference.x / 2, this.transform.position.y + thumbDifference.y / 2, this.transform.position.z + thumbDifference.z / 2);
        //Applies different transformations to scale depending on which corner is being grabbed
        //I'm sure there's a beautiful elegant way to do this, but this way seems to work for now
        switch (stretchPoint)
        {
            case "CornerColliderFrontBottomLeft":
                    this.transform.localScale = new Vector3(this.transform.localScale.x - thumbDifference.x, this.transform.localScale.y - thumbDifference.y, this.transform.localScale.z + thumbDifference.z); 
                                        break;
            case "CornerColliderFrontBottomRight":
                this.transform.localScale = new Vector3(this.transform.localScale.x + thumbDifference.x, this.transform.localScale.y - thumbDifference.y, this.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopLeft":
                this.transform.localScale = new Vector3(this.transform.localScale.x - thumbDifference.x, this.transform.localScale.y + thumbDifference.y, this.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopRight":
                this.transform.localScale = new Vector3(this.transform.localScale.x + thumbDifference.x, this.transform.localScale.y + thumbDifference.y, this.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderBackBottomLeft":
                this.transform.localScale = new Vector3(this.transform.localScale.x - thumbDifference.x, this.transform.localScale.y - thumbDifference.y, this.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackBottomRight":
                this.transform.localScale = new Vector3(this.transform.localScale.x + thumbDifference.x, this.transform.localScale.y - thumbDifference.y, this.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopLeft":
                this.transform.localScale = new Vector3(this.transform.localScale.x - thumbDifference.x, this.transform.localScale.y + thumbDifference.y, this.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopRight":
                this.transform.localScale = new Vector3(this.transform.localScale.x + thumbDifference.x, this.transform.localScale.y + thumbDifference.y, this.transform.localScale.z - thumbDifference.z);
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