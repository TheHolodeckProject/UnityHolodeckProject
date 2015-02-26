using UnityEngine;
using System.Collections;

public class DetectTouch : MonoBehaviour
{
    public bool fingerTouch;
    public bool thumbTouch;
    public string activeFinger;
    public string activeHand;

    void Start()
    {
        fingerTouch = false;
        thumbTouch = false;
    }

    //Whenever something starts colliding with the cube, checks if it's the thumb or the index finger.
    void OnCollisionEnter(Collision col)
    {
        //Checks the list of all the things colliding with the cube to see if it contains the index fingertip and the thumb fingertip
        foreach (ContactPoint contact in col.contacts)
        {
            //If the tip of the index finger is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "index")
                fingerTouch = true;
            // If the thumb tip is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbTouch = true;
                activeHand = contact.otherCollider.transform.parent.parent.name;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        //When something stops touching the collider, check if it's the index or the thumb
        foreach (ContactPoint contact in col.contacts)
        {
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "index")
                fingerTouch = false;
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
                thumbTouch = false;
        }
    }
}