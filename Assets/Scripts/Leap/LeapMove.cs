//using UnityEngine;
//using System.Collections;

//public class LeapMove : MonoBehaviour
//{
//    private bool fingerTouch;
//    private bool thumbTouch;
//    private bool moving;
//    private string fingerTouchObject;
//    private string thumbTouchObject;
//    private Vector3 thumbPosition;
//    private Vector3 indexPosition;
//    private Vector3 indexRotation;
//    private Vector3 thumbRotation;

//    private Vector3 movePosition;
//    private Quaternion moveRotation;
//    private Vector3 thumbPrevRotation;
//    private bool wasMoving;
//    private Vector3 previousCubePosition;
//    private Vector3 middlePosition;
//    private Vector3 middleRotation;
//    private Vector3 middlePrevPosition;
//    private Vector3 middlePrevRotation;
//    private Vector3 rotationDifference;
//    private Vector3 positionDifference;
//    private Vector3 previousMiddlePosition;
//    private bool readyToMove;
//    private GameObject activeHand;
//    private bool isRight;
//    void Start()
//    {
//        readyToMove = false;
//        wasMoving = false;
//        fingerTouch = false;
//        thumbTouch = false;
//        moving = false;
//        fingerTouchObject = null;
//        thumbTouchObject = null;
//        isRight = false;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (moving)
//        {
//            //If there's no hand in the scene, don't bother
//            if (activeHand == null)
//            {
//                Debug.Log("Lost the hand");
//                moving = false;
//                readyToMove = false;
//                return;
//            }
//            //POSITION
//            thumbPosition = activeHand.GetComponent<LeapDetectPinch>().thumbPosition;
//            indexPosition = activeHand.GetComponent<LeapDetectPinch>().indexPosition;
//            //Alternate version of getting finger position - doesn't seem to make much different
//            // ??? Is one more efficient than the other?
//            //thumbPosition = GameObject.Find(activeHand.name + "thumb/bone3").transform.position;
//            //indexPosition = GameObject.Find(activeHand.name + "index/bone3").transform.position;
//            middlePosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);
//            Debug.Log("Difference = " + positionDifference);
//            positionDifference = middlePosition - middlePrevPosition;
//            middlePrevPosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);
//            //For some reason, have to reverse the x axis when using the Leap tip position
//            //this.transform.position +=positionDifference;
//            this.transform.position += new Vector3(-positionDifference.x, positionDifference.y, positionDifference.z);

//            // ROTATION
//            //thumbRotation = GameObject.Find(activeHand.name + "thumb/bone3").transform.rotation;
//            //indexRotation = GameObject.Find(activeHand.name + "index/bone3").transform.rotation;
//            thumbRotation = activeHand.GetComponent<LeapDetectPinch>().thumbRotation;
//            indexRotation = activeHand.GetComponent<LeapDetectPinch>().indexRotation;
//            middleRotation = Vector3.Lerp(thumbRotation, indexRotation, .5f);
//            Debug.Log("ThumbRotation = " + thumbRotation);
//            Debug.Log("IndexRotation = " + indexRotation);
//            Debug.Log("Middle rotation = " + middleRotation);
//            rotationDifference = (middlePrevRotation - middleRotation);
//            //Axes are messed up
//            rotationDifference = new Vector3(-rotationDifference.x, -rotationDifference.z, -rotationDifference.y);

//            //thumbPrevRotation = thumbRotation;
//            middlePrevRotation = middleRotation;
//            this.transform.rotation = this.transform.rotation * Quaternion.Euler(rotationDifference);
//            //Have to reverse the x axis, for some reason
//            //this.transform.rotation = Quaternion.Euler(this.transform.rotation.x + rotationDifference.x, this.transform.rotation.y + rotationDifference.y, this.transform.rotation.z + rotationDifference.z);
//        }

//        DetectFingerCollisions();

//        //Defines the starting middle point on rising edge
//        if (readyToMove)
//        {


//            thumbPosition = activeHand.GetComponent<DetectTouch>().thumbPosition;
//            indexPosition = activeHand.GetComponent<DetectTouch>().indexPosition;
//            //thumbPosition = GameObject.Find(activeHand.name + "thumb/bone3").transform.position;
//            //indexPosition = GameObject.Find(activeHand.name + "index/bone3").transform.position;
//            middlePrevPosition = Vector3.Lerp(thumbPosition, indexPosition, .5f);

//            //thumbRotation = GameObject.Find(activeHand.name + "thumb/bone3").transform.rotation;
//            //indexRotation = GameObject.Find(activeHand.name + "index/bone3").transform.rotation;
//            thumbRotation = activeHand.GetComponent<LeapDetectPinch>().thumbRotation;
//            indexRotation = activeHand.GetComponent<LeapDetectPinch>().indexRotation;
//            middlePrevRotation = Vector3.Lerp(thumbRotation, indexRotation, .5f);
//            // thumbPrevRotation = thumbRotation;
//            moving = true;
//        }
//    }

//    void DetectFingerCollisions()
//    {
//        fingerTouch = false;
//        thumbTouch = false;
//        foreach (Transform child in transform)
//        {
//            if (child.gameObject.GetComponent<DetectTouch>().fingertouch)
//            {
//                thumbTouch = true;
//                continue;
//            }

//            if (child.gameObject.GetComponent<DetectTouch>().thumbtouch)
//            {
//                fingerTouch = true;
//                //Checks if it's the right hand, so it knows what to look for
//                activeHand = child.gameObject.GetComponent<DetectTouch>().activeHand;
//                continue;
//            }
//        }
//        if (fingerTouch && thumbTouch)
//        {
//            Debug.Log("Ready to move");
//            readyToMove = true;
//        }
//        else
//            readyToMove = false;
//        moving = false;


//    }
//}


  
