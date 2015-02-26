using UnityEngine;
using System.Collections;
using Leap;
public class LeapStretch : MonoBehaviour
{
    public static bool stretching;
    public static int stretchHands = 0;
    public static float MinCubeSize = .05f;
    public static float MaxCubeSize = 1f;
    public float CubeSizeBuffer = .025f;
    private bool pinching;
    private bool triggerPinch;
    private bool onStretch;
    private string stretchCorner;
    private Vector3 thumbTipPrevPosition;
    private Vector3 thumbTipPosition;
    private HandModel handModel;
    private Hand leapHand;
    private bool moving;
    enum State { Idle, StartStretch, Stretch, ExitStretch };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        pinching = false;
        stretching = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        //If currently moving something, don't bother trying to stretch
        if (this.GetComponent<LeapMove>().moving == true)
            return;
        else
        {
            GetHandInfo();

            switch (currentState)
            {
                case State.Idle:
                    triggerPinch = LeapDetectPinch();
                    //On a rising edge, check if the pinch happened in a strechablecube collider
                    if (triggerPinch && !pinching)
                        currentState = OnPinch();
                    else if (!triggerPinch && pinching)
                        OnPinchRelease();
                    break;
                case State.StartStretch:
                    currentState = StartStretch();
                    break;
                case State.Stretch:
                    triggerPinch = LeapDetectPinch();
                    if (triggerPinch)
                    {
                        Stretching();
                        CheckCubeSize();
                    }
                    else
                        currentState = State.ExitStretch;
                    break;
                case State.ExitStretch:
                    currentState = StopStretch();
                    break;
            }
        }
    }

    private void GetHandInfo()
    {
        //Get hand info and if there's no hand in the scene, don't run the rest of the update
        handModel = GetComponent<HandModel>();
        leapHand = handModel.GetLeapHand();
        if (leapHand == null)
            return;
    }

    private const float pinchDistanceRatio = 0.7f;
    bool LeapDetectPinch()
    {
        triggerPinch = false;
        //Scales the pinch distance threshold to the size of the hand
        float proximalLength = leapHand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
        float triggerDistance = proximalLength * pinchDistanceRatio;
        // Check thumb tip distance to joints on all other fingers.
        for (int i = 1; i < HandModel.NUM_FINGERS; ++i)
        {
            //Checks if the fingertip is close enough to count as a pinch
            Vector fingerTipPosition = leapHand.Fingers[i].TipPosition;
            if (fingerTipPosition.DistanceTo(leapHand.Fingers[0].TipPosition) < triggerDistance)
                return true;
        }
        return false;
    }

    private GameObject stretchCube;
    State OnPinch()
    {
        pinching = true;
        //Defines how far away something has to be from the thumb tip to be stretched when pinched
        float stretchRadius = .01f;
        //Converts the pinchPosition to Vector3 absolute position coordinates, which have to be converted from the local coordinates of the HandController
        thumbTipPrevPosition = handModel.transform.TransformPoint(leapHand.Fingers[0].TipPosition.ToUnityScaled());
        // Checks if you pinched a movable object and grab the closest one that's not part of the hand.
        Collider[] closeThings = Physics.OverlapSphere(thumbTipPrevPosition, stretchRadius);
        //For everything in the radius around the thumbtip
        for (int j = 0; j < closeThings.Length; ++j)
        {
            //If it's colliding with one of the child colliders of the StretchableCube
            if (closeThings[j].transform.parent.name == "StretchableCube(Clone)")
            {
                stretchCube = closeThings[j].transform.parent.gameObject;
                stretchCorner = closeThings[j].name;
                return State.StartStretch;
            }
        }
        //If it didn't start a stretch, change the state to idle
        return State.Idle;
    }

    void OnPinchRelease()
    {
        pinching = false;
    }

    State StartStretch()
    {
        stretching = true;
        stretchHands = stretchHands + 1;
        return State.Stretch;
    }

    private void CheckCubeSize()
    {
        //If the cube has reached max size on any axis, quit stretching and make it a tiny bit smaller on that axis so the user can fix it
        if (stretchCube.transform.localScale.x >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on X axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x - CubeSizeBuffer, stretchCube.transform.localScale.y, stretchCube.transform.localScale.z);
            currentState = State.ExitStretch;
        }
        else if (stretchCube.transform.localScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on X axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x + CubeSizeBuffer, stretchCube.transform.localScale.y, stretchCube.transform.localScale.z);
            currentState = State.ExitStretch;
        }
        //Y axis size check
        if (stretchCube.transform.localScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x, stretchCube.transform.localScale.y - CubeSizeBuffer, stretchCube.transform.localScale.z);
            currentState = State.ExitStretch;
        }
        else if (stretchCube.transform.localScale.x <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Y axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x, stretchCube.transform.localScale.y + CubeSizeBuffer, stretchCube.transform.localScale.z);
            currentState = State.ExitStretch;
            return;
        }
        //Z axis size check
        if (stretchCube.transform.localScale.y >= MaxCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too big on Y axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x, stretchCube.transform.localScale.y, stretchCube.transform.localScale.z - CubeSizeBuffer);
            currentState = State.ExitStretch;
        }
        else if (stretchCube.transform.localScale.z <= MinCubeSize)
        {
            // !!! INSERT VIBROBUMP
            Debug.Log("Cube is too small on Z axis");
            stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x, stretchCube.transform.localScale.y, stretchCube.transform.localScale.z + CubeSizeBuffer);
            currentState = State.ExitStretch;
        }
    }
    private void Stretching()
    {
        thumbTipPosition = handModel.transform.TransformPoint(leapHand.Fingers[0].TipPosition.ToUnityScaled());
        //Defines how much the thumb has moved since the last frame. This position difference will be used to stretch the cube      
        Vector3 thumbDifference = thumbTipPosition - thumbTipPrevPosition;
        thumbDifference = stretchCube.transform.InverseTransformDirection(thumbDifference);
        thumbTipPrevPosition = thumbTipPosition;
        //Moves the cube's position based on how the thumb's moved
        stretchCube.transform.position = stretchCube.transform.position + stretchCube.transform.TransformDirection(thumbDifference / 2);//new Vector3(stretchCube.transform.position.x - thumbDifference.x / 2, stretchCube.transform.position.y - thumbDifference.y / 2, stretchCube.transform.position.z - thumbDifference.z / 2);
        //Applies different transformations to scale depending on which corner is being grabbed
        switch (stretchCorner)
        {
            case "CornerColliderFrontBottomLeft":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x + thumbDifference.x, stretchCube.transform.localScale.y - thumbDifference.y, stretchCube.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontBottomRight":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x - thumbDifference.x, stretchCube.transform.localScale.y - thumbDifference.y, stretchCube.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopLeft":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x + thumbDifference.x, stretchCube.transform.localScale.y + thumbDifference.y, stretchCube.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderFrontTopRight":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x - thumbDifference.x, stretchCube.transform.localScale.y + thumbDifference.y, stretchCube.transform.localScale.z + thumbDifference.z);
                break;
            case "CornerColliderBackBottomLeft":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x + thumbDifference.x, stretchCube.transform.localScale.y - thumbDifference.y, stretchCube.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackBottomRight":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x - thumbDifference.x, stretchCube.transform.localScale.y - thumbDifference.y, stretchCube.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopLeft":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x + thumbDifference.x, stretchCube.transform.localScale.y + thumbDifference.y, stretchCube.transform.localScale.z - thumbDifference.z);
                break;
            case "CornerColliderBackTopRight":
                stretchCube.transform.localScale = new Vector3(stretchCube.transform.localScale.x - thumbDifference.x, stretchCube.transform.localScale.y + thumbDifference.y, stretchCube.transform.localScale.z - thumbDifference.z);
                break;
        }
    }

    State StopStretch()
    {
        stretchHands = stretchHands - 1;
        //Debug.Log(this.name + " exiting Stretch state. Stretchhands is now " + stretchHands);
        return State.Idle;
    }
}