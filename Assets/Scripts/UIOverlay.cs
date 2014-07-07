using UnityEngine;
using System.Collections;

public class UIOverlay : MonoBehaviour {



	// Use this for initialization
	void Start () {
		// Make a background box
		GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 500, 500));
		GUI.Label (new Rect (20, 20, 300, 30), "");
		GUI.EndGroup ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
