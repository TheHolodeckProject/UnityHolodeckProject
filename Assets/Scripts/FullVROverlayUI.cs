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
        if (center != null)
        {
            Vector3 position = new Vector3(center.transform.position.x, center.transform.position.y + 0.5f, center.transform.position.z + 0.6f);
            GameObject.Find("OVRPlayerController").transform.position = position;
        }
    }
}
