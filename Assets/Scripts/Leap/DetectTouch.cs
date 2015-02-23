using UnityEngine;
using System.Collections;

public class DetectTouch : MonoBehaviour
{
    public bool fingerTouch;
    public bool thumbTouch;
    public string activeFinger;
    public string activeThumb;
    public bool isAButton = false;
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
            //If the thumb is touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbTouch = true;
                activeThumb = contact.otherCollider.gameObject.transform.parent.name;
            }
            //If another finger is also touching
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name != "thumb")
                fingerTouch = true;
                activeFinger = contact.otherCollider.gameObject.transform.parent.name;

        }

        if (isAButton && (thumbTouch || fingerTouch))
        {
            gameObject.GetComponent<ButtonManager>().ToggleButtonState();
            fingerTouch = false;
            thumbTouch = false;
        }

    }

    void OnCollisionExit(Collision col)
    {
        //When something stops touching the collider, check if it's the index or the thumb
        foreach (ContactPoint contact in col.contacts)
        {
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name == "thumb")
            {
                thumbTouch = false;
                activeThumb = null;
            }
            if (contact.otherCollider.gameObject.transform.name == "bone3" && contact.otherCollider.gameObject.transform.parent.name != "thumb")
            {
                fingerTouch = false;
                activeFinger = null;
            }
        }
    }
}