//Copied/edited from LoadGUI.cs for IPostionTask

using UnityEngine;
using System.Collections;

public class LoadGuiTreasureHunt : MonoBehaviour
{

    string comPortFieldString = "COM1";
    int subjectIdentifierInt = 000001;
    int numberOfStimuliInt = 2;
    int numberofTrialsInt = 3;
    string errorString = "";
    int difficulty = 4;
    bool togglePtrn = false;
    bool toggleTimed = false;
    bool ptrnBool = false;
    int selMode = 0;
    string[] selModeStrings = {  "Practice", "Trial" };
  

   

    //For checkboxes
    bool objectRegionToggle = true;
    GUIContent objectRegionContent = new GUIContent();
    bool vibroGlovesEnabled = false;
    GUIContent vibroGloveContent = new GUIContent();
    public Texture uncheckedTexture;
    public Texture checkedTexture;


    void OnGUI()
    {
        // Make a background box
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 850, 850));
        //GUI.Box (new Rect (10,10,500,500), "");

        

        //Haptics Section
        GUI.Label(new Rect(20, 20, 300, 30), "Haptics Com Port");
        comPortFieldString = GUI.TextField(new Rect(20, 50, 120, 20), comPortFieldString);

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 80, 120, 20), "Test Haptics"))
        {
            //string comPort = comPortFieldString;
            //Use comPort as needed
            testHaptics();
        }

        //Subject Identifier
        GUI.Label(new Rect(200, 20, 300, 30), "Subject Identifier");
        
        string tmp = GUI.TextField(new Rect(200, 50, 120, 20), subjectIdentifierInt == -1 ? "" : "" + subjectIdentifierInt);
        
        if (tmp == "")
            subjectIdentifierInt = -1;
        else
            try { subjectIdentifierInt = int.Parse(tmp); }
            catch (UnityException) { subjectIdentifierInt = -1; };

        //Stimuli/Trials
        GUI.Label(new Rect(100, 140, 300, 30), "Number of Stimuli");
        tmp = GUI.TextField(new Rect(100, 160, 120, 20), numberOfStimuliInt == -1 ? "" : "" + numberOfStimuliInt);
        if (tmp == "")
            numberOfStimuliInt = -1;
        else
            try { numberOfStimuliInt = int.Parse(tmp); }
            catch (UnityException) { numberOfStimuliInt = -1; };

        
        togglePtrn = GUI.Toggle(new Rect(-1, 140, 300, 30), togglePtrn, " Pattern");
        toggleTimed = GUI.Toggle(new Rect(-1, 180, 300, 30), toggleTimed, " Timed");
        

        selMode = GUI.SelectionGrid(new Rect(415, 140, 150, 90), selMode, selModeStrings, 1);
        //print(selMode);   /* For debugging */

        GUI.Label(new Rect(100, 180, 300, 30), "Number of Trials");
        tmp = GUI.TextField(new Rect(100, 200, 120, 20), numberofTrialsInt == -1 ? "" : "" + numberofTrialsInt);
        if (tmp == "")
            numberofTrialsInt = -1;
        else
            try { numberofTrialsInt = int.Parse(tmp); }
            catch (UnityException) { numberofTrialsInt = -1; };

        GUI.Label(new Rect(100, 220, 300, 30), "Difficulty");
        tmp = GUI.TextField(new Rect(100, 240, 120, 20), difficulty == -1 ? "" : "" + difficulty);
        if (tmp == "")
            difficulty = -1;
        else
            try { difficulty = int.Parse(tmp); }
            catch (UnityException) { difficulty = -1; };

        //Object Field Toggle
        GUI.Label(new Rect(250, 140, 300, 30), "Oculus Mounted?");
        if (GUI.Button(new Rect(250, 160, 20, 20), objectRegionContent.image, new GUIStyle()))
            objectRegionToggle = !objectRegionToggle;

        //Vibro Glove Enable Checkbox
        GUI.Label(new Rect(250, 180, 300, 30), "Vibro Gloves Enabled?");
        if (GUI.Button(new Rect(250, 200, 20, 20), vibroGloveContent.image, new GUIStyle()))
            vibroGlovesEnabled = !vibroGlovesEnabled;

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(100, 280, 120, 20), "Enter Simulation"))
        {
            //Store copy of current values for processing
            string comPort = comPortFieldString;
            int subjectIdentifier = subjectIdentifierInt;
            int numberOfStimuli = numberOfStimuliInt;
            int numberOfTrials = numberofTrialsInt;
            bool objectRegion = objectRegionToggle;
            bool vibroGloves = vibroGlovesEnabled;
            int temp;
            //Save to player preferences
            PlayerPrefs.SetString("COM Port", comPort);
            PlayerPrefs.SetInt("Subject Identifier", subjectIdentifier);
            PlayerPrefs.SetInt("Number of Stimuli", numberOfStimuli);
            PlayerPrefs.SetInt("Number of Trials", numberOfTrials);
            PlayerPrefs.SetInt("Object Region", objectRegion ? 1 : 0);
            PlayerPrefs.SetInt("Vibro Gloves", vibroGloves ? 1 : 0);
            PlayerPrefs.SetInt("Difficulty", difficulty);
            temp = (togglePtrn ) ? 1 : 0;
            if (temp == 1) ptrnBool = true;
            else ptrnBool = false;
            PlayerPrefs.SetInt("Pattern", temp);
            temp = (toggleTimed) ? 1 : 0;
            PlayerPrefs.SetInt("Timed", temp);
            PlayerPrefs.SetString("Mode", selModeStrings[selMode]);

            if (numberOfTrials != -1 && numberOfStimuli != -1 && (ptrnBool && numberOfStimuli <= 8 || !ptrnBool))
            {
                //Pass values
                Application.LoadLevel("TreasureHunt");
            }
            else if ((ptrnBool && numberOfStimuli > 8))
            {
                errorString = "Cannot perform pattern trial with more than 8 stimuli.";
                ptrnBool = false;
            }
            else
            {
                errorString = "One of the values is invalid. Please confirm entry.";
            }

        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        GUI.Label(new Rect(230, 310, 300, 30), errorString, style);

        GUI.EndGroup();
    }

    void Update()
    {
        //For checkbox display state
        if (objectRegionToggle)
            objectRegionContent.image = checkedTexture;
        else
            objectRegionContent.image = uncheckedTexture;

        //For checkbox display state
        if (vibroGlovesEnabled)
            vibroGloveContent.image = checkedTexture;
        else
            vibroGloveContent.image = uncheckedTexture;
    }

    void testHaptics()
    {
        //INSERT TEST CODE FOR HAPTICS HERE!
    }


}