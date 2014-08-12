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
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = head.transform.position;
	}
}
