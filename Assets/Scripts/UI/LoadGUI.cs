using UnityEngine;
using System.Collections;

public class LoadGUI : MonoBehaviour
{

    string comPortFieldString = "COM1";
    int subjectIdentifierInt = 1234567890;
    int numberOfStimuliInt = 5;
    int numberofTrialsInt = 3;
    string errorString = "";

    //For checkboxes
    bool objectRegionToggle = true;
    GUIContent objectRegionContent = new GUIContent();
    bool vibroGlovesEnabled = false;
    GUIContent vibroGloveContent = new GUIContent();
    public Texture uncheckedTexture;
    public Texture checkedTexture;


    void OnGUI() {
               // Make a background box
               GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 500, 500));
               //GUI.Box (new Rect (10,10,500,500), "");

               //Haptics Section
               GUI.Label (new Rect (20, 20, 300, 30), "Haptics Com Port");
               comPortFieldString = GUI.TextField (new Rect (20, 50, 120, 20), comPortFieldString);

               // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
               if (GUI.Button (new Rect (20,80,120,20), "Test Haptics")) {
                       //string comPort = comPortFieldString;
                       //Use comPort as needed
                       testHaptics();
               }

               //Subject Identifier
               GUI.Label (new Rect (200, 20, 300, 30), "Subject Identifier");
                       
               string tmp = GUI.TextField(new Rect(200, 50, 120, 20), subjectIdentifierInt == -1 ? "" : "" + subjectIdentifierInt);
                       
               if (tmp == "")
                       subjectIdentifierInt = -1;
               else
               try{subjectIdentifierInt = int.Parse (tmp); }catch(UnityException){subjectIdentifierInt = -1;};

               //Stimuli/Trials
               GUI.Label (new Rect (100, 140, 300, 30), "Number of Stimuli");
               tmp = GUI.TextField (new Rect (100, 160, 120, 20), numberOfStimuliInt == -1?"":""+numberOfStimuliInt);
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

               //Object Field Toggle
               GUI.Label(new Rect(250, 140, 300, 30), "Oculus Mounted?");
               if(GUI.Button (new Rect(250, 160, 20, 20), objectRegionContent.image, new GUIStyle()))
                       objectRegionToggle = !objectRegionToggle;

               //Vibro Glove Enable Checkbox
               GUI.Label(new Rect(250, 180, 300, 30), "Vibro Gloves Enabled?");
               if(GUI.Button (new Rect(250, 200, 20, 20), vibroGloveContent.image, new GUIStyle()))
                       vibroGlovesEnabled = !vibroGlovesEnabled;

               // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
               if (GUI.Button (new Rect (100,240,120,20), "Enter Simulation")) {
                       //Store copy of current values for processing
                       string comPort = comPortFieldString;
                       int subjectIdentifier = subjectIdentifierInt;
                       int numberOfStimuli = numberOfStimuliInt;
                       int numberOfTrials = numberofTrialsInt;
                       bool objectRegion = objectRegionToggle;
                       bool vibroGloves = vibroGlovesEnabled;

                       //Save to player preferences
                       PlayerPrefs.SetString ("COM Port",comPort);
                       PlayerPrefs.SetInt ("Subject Identifier",subjectIdentifier);
                       PlayerPrefs.SetInt ("Number of Stimuli",numberOfStimuli);
                       PlayerPrefs.SetInt ("Number of Trials",numberOfTrials);
                       PlayerPrefs.SetInt ("Object Region",objectRegion?1:0);
                       PlayerPrefs.SetInt ("Vibro Gloves",vibroGloves?1:0);

                       if(numberOfTrials != -1 && numberOfStimuli != -1){
                               //Pass values
                               Application.LoadLevel (1);
                       }
                       else errorString = "One of the values is invalid. Please confirm entry.";
               }

               GUIStyle style = new GUIStyle ();
               style.normal.textColor = Color.red;
               GUI.Label (new Rect (20, 240, 300, 30), errorString,style);

               GUI.EndGroup ();
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
