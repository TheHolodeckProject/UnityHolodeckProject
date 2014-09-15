using UnityEngine;
using System.Collections;
using Leap;

public class LeapHandsManager : MonoBehaviour {

    Controller controller;
	// Use this for initialization
	void Start () {
        controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {
        Frame frame = controller.Frame();
	}
}
