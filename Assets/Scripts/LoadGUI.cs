﻿using UnityEngine;
using System.Collections;

public class LoadGUI : MonoBehaviour {

	string comPortFieldString = "COM1";
	string subjectIdentifierString = "1234567890";
	int numberOfStimuliInt = 5;
	int numberofTrialsInt = 3;
	string errorString = "";

	void OnGUI() {
		// Make a background box
		GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 500, 500));
		//GUI.Box (new Rect (10,10,500,500), "");
		GUI.Label (new Rect (20, 20, 300, 30), "Haptics Com Port");
		comPortFieldString = GUI.TextField (new Rect (20, 50, 120, 20), comPortFieldString);

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button (new Rect (20,80,120,20), "Test Haptics")) {
			//string comPort = comPortFieldString;
			//Use comPort as needed
			testHaptics();
		}

		GUI.Label (new Rect (200, 20, 300, 30), "Subject Identifier");
		subjectIdentifierString = GUI.TextField (new Rect (200, 50, 120, 20), subjectIdentifierString);

		GUI.Label (new Rect (100, 140, 300, 30), "Number of Stimuli");
		string tmp = GUI.TextField (new Rect (100, 160, 120, 20), numberOfStimuliInt == -1?"":""+numberOfStimuliInt);
		if (tmp == "")
			numberOfStimuliInt = -1;
		else
			try{numberOfStimuliInt = int.Parse (tmp); }catch(UnityException){numberOfStimuliInt = -1;};

		GUI.Label (new Rect (100, 180, 300, 30), "Number of Trials");
		tmp = GUI.TextField (new Rect (100, 200, 120, 20), numberofTrialsInt == -1?"":""+numberofTrialsInt);
		if (tmp == "")
			numberofTrialsInt = -1;
		else
		try{numberofTrialsInt = int.Parse (tmp); }catch(UnityException){numberofTrialsInt = -1;};

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button (new Rect (100,240,120,20), "Enter Simulation")) {
			//Store copy of current values for processing
			string comPort = comPortFieldString;
			string subjectIdentifier = subjectIdentifierString;
			int numberOfStimuli = numberOfStimuliInt;
			int numberOfTrials = numberofTrialsInt;

			//Save to player preferences
			PlayerPrefs.SetString ("COM Port",comPort);
			PlayerPrefs.SetString ("Subject Identifier",subjectIdentifier);
			PlayerPrefs.SetInt ("Number of Stimuli",numberOfStimuli);
			PlayerPrefs.SetInt ("Number of Trials",numberOfTrials);

			if(numberOfTrials != -1 && numberOfStimuli != -1){
				//Pass values
				Application.LoadLevel (2);
			}
			else errorString = "One of the values is invalid. Please confirm entry.";
		}

		GUIStyle style = new GUIStyle ();
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (20, 240, 300, 30), errorString,style);

		GUI.EndGroup ();
	}

	void testHaptics(){
		//INSERT TEST CODE FOR HAPTICS HERE!
	}
}
