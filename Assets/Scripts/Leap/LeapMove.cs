using UnityEngine;
using System.Collections;

public class LeapMove : MonoBehaviour
{

    Collision col;
    private bool fingerTouch;
    private bool thumbTouch;
    private bool moving;
    private string fingerTouchObject;
    private string thumbTouchObject;
    private Vector3 thumbPosition;
    private Quaternion thumbRotation;
    private Vector3 indexPosition;
    private Quaternion indexRotation;
    private Vector3 movePosition;
    private Quaternion moveRotation;
    private bool wasMoving;
    private Vector3 previousCubePosition;
    private Vector3 middlePosition;
    private Quaternion middleRotation;
    private Vector3 middlePrevPosition;
    private Quaternion middlePrevRotation;
    private Quaternion rotationDifference;
    private Vector3 positionDifference;
    private Vector3 previousMiddlePosition;
    private bool readyToMove;
    private GameObject activeHand;
    private bool isRight;
    void Start()
    {
        readyToMove = false;
        wasMoving = false;
        fingerTouch = false;
        thumbTouch = false;
        moving = false;
        fingerTouchObject = null;
        thumbTouchObject = null;
        isRight = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (moving)
        {
            //Calculates middle position and compares it to a frame ago
            thumbPosition = GameObject.Find(activeHand.name + "thumb/bone3").transform.position;
            indexPosition = GameObject.Find(activeHand.name + "index/bone3").transform.position;
            middlePosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);

            positionDifference =  middlePosition - middlePrevPosition;
            this.transform.position += positionDifference;
            middlePrevPosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);


            //Same deal for rotation
            //thumbRotation = GameObject.Find(activeHand.name + "thumb/bone3").transform.rotation;
            //indexRotation = GameObject.Find(activeHand.name + "index/bone3").transform.rotation;
            //middleRotation = Quaternion.Lerp(thumbRotation, indexRotation, .5f);

            //rotationDifference = middleRotation * Quaternion.Inverse(middlePrevRotation);
            //Debug.Log("rotationDifference = ");
            //this.transform.rotation = this.transform.rotation * rotationDifference;
            //middlePrevRotation = Quaternion.Lerp(thumbRotation, indexRotation, .5f);

            //this.transform.position = new Vector3(this.transform.position.x - difference.x, this.transform.position.y + difference.y / 2, this.transform.position.z + difference.z / 2);

        }

            DetectFingerCollisions();

        //Defines the starting middle point on rising edge
        if (readyToMove)
        {
            thumbPosition = GameObject.Find(activeHand.name + "thumb/bone3").transform.position;
            indexPosition = GameObject.Find(activeHand.name + "index/bone3").transform.position;
            middlePrevPosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);

            thumbRotation = GameObject.Find(activeHand.name + "thumb/bone3").transform.rotation;
            indexRotation = GameObject.Find(activeHand.name + "index/bone3").transform.rotation;
            middlePrevRotation = Quaternion.Lerp(thumbRotation, indexRotation, .5f);
            moving = true;
        }
    }

    void DetectFingerCollisions()
    {
        fingerTouch = false;
        thumbTouch = false;
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<OldLeapStretch>().fingertouch)
            {
                thumbTouch = true;
                continue;
            }

            if (child.gameObject.GetComponent<OldLeapStretch>().thumbtouch)
            {
                fingerTouch = true;
                //Checks if it's the right hand, so it knows what to look for
              activeHand = child.gameObject.GetComponent<OldLeapStretch>().activeHand;
                continue;
            }
        }
        if (fingerTouch && thumbTouch)
        {
            Debug.Log("Ready to move");
            readyToMove = true;
        }
        else
            readyToMove = false;
        moving = false;


    }
}


  
