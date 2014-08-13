using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class FullVROverlayUI : MonoBehaviour {

    //Calls the positionSync function every frame
    void Update() { positionSync(); }

    void positionSync()
    {
        //Finds a stable point on the body, near the collarbone
        GameObject center = GameObject.Find("SpineShoulder");
        //Moves the in-game Oculus camera to that position
        GameObject.Find("OVRPlayerController").transform.position = center.transform.position;
    }
}
