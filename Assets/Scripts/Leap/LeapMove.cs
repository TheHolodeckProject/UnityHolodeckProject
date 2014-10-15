using UnityEngine;
using System.Collections;
using Leap;

public class LeapMove : MonoBehaviour
{
    enum State { Idle, Moving, EndMove }
    private State currentState;
    private bool triggerMove;
    public bool moving;
    private bool fingerTouch;
    private bool thumbTouch;
    private Vector3 middlePosition;
    private Vector3 middlePrevPosition;
    private GameObject moveManager;
    // ??? For what I'm using this for, should I be using a transform or a gameobject
    private Transform cube;
    // Use this for initialization
    void Start()
    {
        currentState = State.Idle;
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
                DetectFingerCollisions();
                if (triggerMove)
                    Move();
                else
                    currentState = EndMove();
                break;
        }
    }

    void DetectFingerCollisions()
    // ??? Would it be more efficient to just find the closest cube like we did in iPosition?
    {
        cube = null;
        fingerTouch = false;
        thumbTouch = false;
        //Gets the thumb position
        Vector3 thumbTipPosition = GameObject.Find(this.name + "/thumb/bone3").transform.position;
        // Checks a tiny sphere collider around the thumb
        Collider[] thumbTouching = Physics.OverlapSphere(thumbTipPosition, .01f);
        //For everything in the radius around the thumbtip
        for (int j = 0; j < thumbTouching.Length; ++j)
        {
            //If it's colliding with one of the child colliders of the StretchableCube
            if (thumbTouching[j].transform.parent.name == "StretchableCube(Clone)")
            {
                cube = thumbTouching[j].transform.parent.transform;
                break;
            }
        }
        //If it found a cube    
        if (cube != null)
        {
            //For everything the cube is touching
            foreach (Transform child in cube)
            {
                //Check if the thumb and index finger are touching different colliders
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
            cube.position = cube.transform.position - diffPosition;
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
        //Moves the grabbed object a little closer to the middle position when you grab it
        cube.position = Vector3.MoveTowards(cube.position, middlePrevPosition, .005f);
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