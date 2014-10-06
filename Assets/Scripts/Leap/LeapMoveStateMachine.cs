using UnityEngine;
using System.Collections;

//!!! Make work with fingers other than index
public class LeapMoveStateMachine : MonoBehaviour
{

    enum State { Idle, Moving }
    private State currentState;
    private GameObject cube;
    private string activeHand;
    private bool triggerMove;
    public static bool moving;
    private bool fingerTouch;
    private bool thumbTouch;
    private Vector3 middlePosition;
    private Vector3 middlePrevPosition;
    private GameObject moveManager;
    // Use this for initialization
    void Start()
    {
        currentState = State.Idle;
        moveManager = GameObject.Find("MoveManager");
        cube = GameObject.Find("StretchableCube");
        triggerMove = false;
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        //If something is being stretched, don't allow moves
        if (LeapStretchStateMachine.stretching == true)
            return;

        switch (currentState)
        {
            case State.Idle:
                    DetectFingerCollisions();
                    if (triggerMove)
                    {
                        OnMove();
                        currentState = State.Moving;
                    }
                    break;
            
            case State.Moving:
                Debug.Log("Moving");
                DetectFingerCollisions();
                if (triggerMove)
                    Move();
                else
                {
                    EndMove();
                    currentState = State.Idle;
                }
                break;
        }
    }

    void DetectFingerCollisions()
    {
        fingerTouch = false;
        thumbTouch = false;
        foreach (Transform child in cube.transform)
        {
            if (child.gameObject.GetComponent<DetectTouch>().thumbTouch)
            {
                thumbTouch = true;
                activeHand = child.gameObject.GetComponent<DetectTouch>().activeHand;
                //Debug.Log("Thumb touching" + child.name);
                continue;
            }
            if (child.gameObject.GetComponent<DetectTouch>().fingerTouch)
            {
                fingerTouch = true;
                //Debug.Log("Finger touching" + child.name);
                continue;
            }

        }
        if (fingerTouch && thumbTouch)
            triggerMove = true;
        else
            triggerMove = false;
    }

    void Move()
    {
        // Sets the orientation of the MoveManager to that of the thumb bone
        moveManager.transform.rotation = GameObject.Find(activeHand + "/thumb/bone3").gameObject.transform.rotation;
        //Calculates midpoint between thumb and index finger and uses that as the position of the moveManager
        Vector3 thumbPosition = GameObject.Find(activeHand + "/thumb/bone3").gameObject.transform.position;
        Vector3 fingerPosiiton = GameObject.Find(activeHand + "/index/bone3").gameObject.transform.position;
        Vector3 middlePoint = Vector3.Lerp(thumbPosition, fingerPosiiton, .5f);
        moveManager.transform.position = middlePoint;
    }

    void OnMove()
    {
        Debug.Log("Starting move");
        // Sets the orientation of the MoveManager to that of the thumb bone
        moveManager.transform.rotation = GameObject.Find(activeHand + "/thumb/bone3").gameObject.transform.rotation;
        //Calculates midpoint between thumb and index finger and uses that as the position of the moveManager
        Vector3 thumbPosition = GameObject.Find(activeHand + "/thumb/bone3").gameObject.transform.position;
        Vector3 fingerPosiiton = GameObject.Find(activeHand + "/index/bone3").gameObject.transform.position;
        Vector3 middlePoint = Vector3.Lerp(thumbPosition, fingerPosiiton, .5f);
        moveManager.transform.position = middlePoint;
        //Makes the cube a child of the MoveManager object
        cube.transform.parent = moveManager.gameObject.transform;
    }

    void EndMove()
    {
        cube.transform.parent = null;
        moving = false;
        if (!fingerTouch)
            Debug.Log("Ending move because the finger stopped touching");
        else if (!thumbTouch)
            Debug.Log("Ending move because the thumb stopped touching");
    }
}