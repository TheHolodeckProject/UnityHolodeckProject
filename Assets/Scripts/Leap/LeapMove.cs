using UnityEngine;
using System.Collections;

public class LeapMove : MonoBehaviour
{
    enum State { Idle, Moving, EndMove }
    private State currentState;
    private GameObject cube;
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
        cube = GameObject.Find("StretchableCube");
        triggerMove = false;
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                    DetectFingerCollisions();
                    if (triggerMove)
                        currentState = OnMove();
                    break;
            
            case State.Moving:
                Debug.Log("Moving");
                DetectFingerCollisions();
                if (triggerMove)
                    Move();
                else                
                    currentState = EndMove();
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
        //Sometimes the hand drops out for a frame. This doesn't update the object's position if the hands aren't there.
        if (GameObject.Find(this.name + "/thumb/bone3").gameObject.transform != null)
        {
            //Calculates midpoint between thumb and index finger and uses that as the position of the moveManager
            Vector3 thumbPosition = GameObject.Find(this.name + "/thumb/bone3").gameObject.transform.position;
            Vector3 fingerPosition = GameObject.Find(this.name + "/index/bone3").gameObject.transform.position;
            middlePosition = Vector3.Lerp(thumbPosition, fingerPosition, .5f);
            //Calculates the difference between where the middle point is now and where it was a frame ago
            Vector3 diffPosition = middlePrevPosition - middlePosition;
            //Applies that difference to the grabbed cube
            cube.transform.position = cube.transform.position - diffPosition;
            //Logs the middle position to be used as a reference next frame
            middlePrevPosition = middlePosition;
        }
    }

    State OnMove()
    {
        Debug.Log("Starting move");
        //Changes the global variable to true, so other scripts can access it
        moving = true;
        //Calculates midpoint between thumb and index finger and uses that as the position of the moveManager
        Vector3 thumbPosition = GameObject.Find(this.name + "/thumb/bone3").gameObject.transform.position;
        Vector3 fingerPosition = GameObject.Find(this.name + "/index/bone3").gameObject.transform.position;
        //Previous position gets subtracted from current position in later frames, so we need a previous position to start with
        // ??? Why does adding Vector3 to this make is inacessible by Move?
        middlePrevPosition = Vector3.Lerp(thumbPosition, fingerPosition, .5f);
        return State.Moving;
    }

    State EndMove()
    {  
        moving = false;
        if (!fingerTouch)
            Debug.Log("Ending move because the finger stopped touching");
        else if (!thumbTouch)
            Debug.Log("Ending move because the thumb stopped touching");
        return State.Idle;
    }
}