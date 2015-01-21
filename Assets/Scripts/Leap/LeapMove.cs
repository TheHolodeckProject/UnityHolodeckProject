// ??? It's much more responsive now, but it's a little jumpy. Any way to fix?

using UnityEngine;
using System.Collections;
using Leap;

public class LeapMove : MonoBehaviour
{
    public float grabRadius;
    private bool triggerMove;
    public AudioClip grabSound;
    public AudioClip releaseSound;
    private HandModel hand_model;
    private Hand leap_hand;
    private Collider grabbed_;
    private GameObject cube;
    private GameObject handcontroller;
    private GameObject middle;
    private Vector3 thumbTipPosition;
    private Vector3 fingerTipPosition;
    private int fingerInt;
    enum State { Idle, Moving, EndMove }
    private State currentState;

    void Start()
    {
        currentState = State.Idle;
        triggerMove = false;
        handcontroller = GameObject.Find("HandController");
    }

    // Update is called once per frame
    void Update()
    {
        //Gets Leap hand info.
        hand_model = GetComponent<HandModel>();
        leap_hand = hand_model.GetLeapHand();

        //If there's no hand in the frame, don't bother running the rest of the update
        if (leap_hand == null)
            return;

        //Debug.Log(currentState);
        switch (currentState)
        {
            case State.Idle:
                DetectFingerCollisions();
                if (triggerMove)
                    currentState = OnMove();
                break;

            case State.Moving:
                currentState = Move();
                break;
        }
    }

    void DetectFingerCollisions()
    {
        cube = null;
        bool thumbTouch = false;
        int thumbInt = 0;
        //Gets the thumb position
        thumbTipPosition = leap_hand.Fingers[0].TipPosition.ToUnityScaled() + handcontroller.transform.position;
        // Checks a tiny sphere collider around the thumb
        Collider[] thumbTouching = Physics.OverlapSphere(thumbTipPosition, grabRadius);
        //For everything in the radius around the thumbtip
        for (int j = 0; j < thumbTouching.Length; ++j)
        {
            // ??? Should I avoid checking the tag?
            if (thumbTouching[j].transform.parent.tag == "Movable")
            {
                thumbInt = j;
                thumbTouch = true;
                break;
            }
        }
        //If the thumb was touching a grabbable object
        if (thumbTouch)
        {
            //Check if any another finger is also colliding with a grabbable object
            for (int f = 1; f < 5; ++f)
            {
                if (!triggerMove)
                {
                    fingerTipPosition = leap_hand.Fingers[f].TipPosition.ToUnityScaled() + handcontroller.transform.position;
                    Collider[] fingerTouching = Physics.OverlapSphere(fingerTipPosition, grabRadius);
                    for (int k = 0; k < fingerTouching.Length; ++k)
                    {
                        //Checks if the finger is touching the same object as the thumb
                        if (thumbTouching[thumbInt].transform.parent.gameObject == fingerTouching[k].transform.parent.gameObject)
                        {
                            cube = thumbTouching[k].transform.parent.gameObject;
                            triggerMove = true;
                            fingerInt = f;
                            break;
                        }
                    }
                }
            }
        }
    }

    State Move()
    {
        bool thumbTouch = false;
        bool fingerTouch = false;
        //Checks if thumb is still colliding with the cube
        thumbTipPosition = leap_hand.Fingers[0].TipPosition.ToUnityScaled() + handcontroller.transform.position;
        Collider[] thumbTouching = Physics.OverlapSphere(thumbTipPosition, grabRadius);
        for (int j = 0; j < thumbTouching.Length; ++j)
            if (thumbTouching[j].transform.parent.gameObject == cube)
            {
                thumbTouch = true;
                break;
            }
        //Checks if finger is still colliding with the cube

        fingerTipPosition = leap_hand.Fingers[fingerInt].TipPosition.ToUnityScaled() + handcontroller.transform.position;
        Collider[] fingerTouching = Physics.OverlapSphere(fingerTipPosition, grabRadius);
        for (int k = 0; k < fingerTouching.Length; ++k)
            if (fingerTouching[k].transform.parent.gameObject == cube)
            {
                fingerTouch = true;
                break;
            }
        //If thumb and finger are still touching, we're still moving
        if (thumbTouch && fingerTouch)
        {
            //Updates the position of the middle point
            middle.transform.position = Vector3.Lerp(thumbTipPosition, fingerTipPosition, .5f);
            return State.Moving;
        }
        else
        {
            //Makes the cube no longer a transform of the middle object
            cube.transform.parent = null;
            //Destroys the Middle object
            DestroyObject(middle);
            triggerMove = false;
            //Plays the release sound
            audio.PlayOneShot(releaseSound);
            return State.Idle;
        }
    }

    State OnMove()
    {
        //Creates a new game object
        middle = new GameObject("Middle");
        middle.transform.position = Vector3.Lerp(thumbTipPosition, fingerTipPosition, .5f);
        //Makes the cube a child of the middle point
        cube.transform.parent = middle.transform;
        //Moves the grabbed object a little closer to the middle position when you grab it
        cube.transform.position = Vector3.MoveTowards(cube.transform.position, middle.transform.position, .005f);
        //ADDED - Plays a sound
        audio.PlayOneShot(grabSound);
        return State.Moving;
    }

    //Added by kevin to show how to use scripts instead of tags
    //Doable[] getDoables()
    //{
    //    System.Collections.Generic.List<Doable> objsList = new System.Collections.Generic.List<Doable>();
    //    HelperFunctions.GetScriptObjectsInScene<Doable>(out objsList);
    //    return objsList.ToArray();
    //}

    //void testfunc()
    //{
    //    Doable[] objs = getDoables();
    //    //objs[0].gameObject;
    //}

}