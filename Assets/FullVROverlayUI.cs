using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class FullVROverlayUI : MonoBehaviour {
	void OnGUI() {
		if (GUI.Button (new Rect (10, 10, 100, 30), "Sync Position"))
			positionSync ();
	}

	void positionSync(){
		GameObject head = GameObject.Find ("Head");
        GameObject.Find("OVRPlayerController").transform.position = head.transform.position;
	}

    void Update() { positionSync(); }
}
