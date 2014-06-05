using UnityEngine;
using System.Collections;

public class LoadGUI : MonoBehaviour {

	string comPortFieldString = "COM1";
	string subjectIdentifierString = "1234567890";

	void OnGUI() {
		// Make a background box
		GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 500, 500));
		//GUI.Box (new Rect (10,10,500,500), "");
		GUI.Label (new Rect (20, 20, 300, 30), "Haptics Com Port");
		comPortFieldString = GUI.TextField (new Rect (20, 50, 120, 20), comPortFieldString);

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button (new Rect (20,80,120,20), "Test Haptics")) {
			string comPort = comPortFieldString;
			testHaptics();
		}

		GUI.Label (new Rect (200, 20, 300, 30), "Subject Identifier");
		subjectIdentifierString = GUI.TextField (new Rect (200, 50, 120, 20), subjectIdentifierString);

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button (new Rect (200,80,120,20), "Enter Simulation")) {
			string comPort = comPortFieldString;
			Application.LoadLevel (1);
		}
		GUI.EndGroup ();
	}

	void testHaptics(){
		//INSERT TEST CODE FOR HAPTICS HERE!
	}
}
