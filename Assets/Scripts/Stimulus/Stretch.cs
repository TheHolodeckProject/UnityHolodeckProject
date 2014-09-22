using UnityEngine;
using System.Collections;
using Leap;

public class Stretch : MonoBehaviour
{
    private bool fingertouch = false;
    private bool thumbtouch = false;
    private Vector3 thumbposition;
    private Vector3 prevposition;
    private GameObject cube;
    private float prevx;
    private Vector3 startposition;
    
    void Start()
    {
        cube = GameObject.Find("StrechableCube");
    }

    //Whenever something starts colliding with the cube, check if it's the thumb or the index finger.
    void OnCollisionEnter(Collision col)
    {
        //For everything colliding with the cube collider
        foreach (ContactPoint contact in col.contacts)
        {
            //Checks the name of the thing colliding with it
            switch (contact.otherCollider.gameObject.transform.parent.name)
            {
                case "index":
                    fingertouch = true;
                    Debug.Log("Index touched");
                    break;
                case "thumb":
                    thumbtouch = true;
                    Debug.Log("Thumb touched");
                    Vector3 startposition = GameObject.Find("MagneticPinchHand(Clone)/thumb/bone1").transform.position;
                    break;

                   
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            switch (contact.otherCollider.gameObject.transform.parent.name)
            {
                case "index":
                    fingertouch = false;
                    //Debug.Log("Index no longer touching");
                    break;
                case "thumb":
                    thumbtouch = false;
                    //Debug.Log("Thumb no longer touching");
                    break;
            }
        }
    }

    void Update()
    {
        Vector3 thumbposition = GameObject.Find("MagneticPinchHand(Clone)/thumb/bone1").transform.position;
       
        //If the finger and thumb are grabbing
        if (fingertouch == true && thumbtouch == true)
        {
            Debug.Log("Pinching");
            // Finds the position of the thumb.
                //Makes the actual cube a child of the thumb
            thumbposition = GameObject.Find("MagneticPinchHand(Clone)/thumb/bone1").transform.position;
            Debug.Log(thumbposition);
        }
        Vector3 difference = startposition - thumbposition;

        //Debug.Log("Difference = " + difference);
        //Debug.Log();
//        Debug.Log("Difference = " + difference);



        cube.transform.position = cube.transform.position + difference;


    }
        //Stores the previous position so we can track if it's changed
        //prevposition = thumbposition;
}
