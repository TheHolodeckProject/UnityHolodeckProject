using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//TO DO
// Add fade-in, fade-out animation

public class MentalRotationTask : MonoBehaviour
{
    public GameObject oculusCamera;
    public GameObject regularCamera;
    public GameObject resetButton;
    private bool enableLogging;
    private Logger logger;
    private Vector3[] stimLocations;
    public GameObject blackboard;
    private GameObject[] stimuli;

    enum State { SampleStart, Sample, SampleEnd, PracticeStart, Practice, PracticeEnd, TrialStart, TrialEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        //Grabs player preferences
        //Practice
        if (PlayerPrefs.GetInt("PracticeYesNo") == 1)
            currentState = State.PracticeStart;
        else
        {
            currentState = State.TrialStart;
        }
        // Oculus Camera
        if (PlayerPrefs.GetInt("OculusCamYesNo") == 0)
        {
            oculusCamera.SetActive(false);
            regularCamera.SetActive(true);
        }

        //Logging
        enableLogging = PlayerPrefs.GetInt("LogDataYesNo") == 0 ? false : true;
        if (enableLogging) logger = GameObject.Find("Logger").GetComponent<Logger>();
        if (enableLogging) logger.BeginLogging();
        
        stimLocations = new Vector3[3];
        stimLocations[0] = new Vector3 (2, 3, 4);
        stimLocations[1] = new Vector3(2, 3, 4);
        stimLocations[2] = new Vector3(2, 3, 4);
        stimLocations[4] = new Vector3(2, 3, 4);
    }
    // Update is called once per frame
    void Update()
    {
        //Press Esc to quit
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //Debug.Log(currentState);
        switch (currentState)
        {

            case State.PracticeStart:
                blackboard.GetComponentInChildren<Text>().text = "Welcome to the Holodeck\n\nThese are your practice hands\nThey turn transparent as they go out of range\n\n Press the button to continue";
                break;


            case State.Practice:
                //Grabs the fingerTouch variable from the DetectTouch script, which has been placed on a smaller sphere collider in the middle of the EndTrail sphere
                if (resetButton.transform.GetComponentInChildren<DetectTouch>().fingerTouch)
                    TurnOffbutton();
                break;
        }
    }

          

    private void GenerateStimuli()
    {
        for (var i = 0; i <=3; i++)

            // Stimuli - solid, movable
            //stimuli[i] = (GameObject)Instantiate(cubePrefab);
            //stimuli[i].transform.position = stimLocations[i];
            //stimuli[i].transform.rotation = Random.rotation;
            //stimuli[i].transform.renderer.material = new Material(Shader.Find("Diffuse"));
            //stimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            //stimuli[i].name = "Cube - " + stimuli[i].transform.renderer.material.mainTexture.name;
            ////Makes the stimuli tiny, so they can be poppped in
            //stimuli[i].transform.localScale = new Vector3(0, 0, 0);

            //// Practice cubes - transparent, unmovable
            //transparentStimuli[i] = (GameObject)Instantiate(cubePrefab);
            //transparentStimuli[i].transform.position = stimLocations[i];
            //transparentStimuli[i].transform.rotation = stimuli[i].transform.rotation;
            //transparentStimuli[i].transform.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            //transparentStimuli[i].transform.localScale = new Vector3(0, 0, 0);
            //transparentStimuli[i].transform.renderer.material.mainTexture = colors[colorNums[i]];
            //transparentStimuli[i].name = "TransparentCube - " + stimuli[i].transform.renderer.material.mainTexture.name;
            //transparentStimuli[i].transform.renderer.material.color = new Color(stimuli[i].gameObject.transform.renderer.material.color.r, stimuli[i].gameObject.transform.renderer.material.color.g, stimuli[i].gameObject.transform.renderer.material.color.b, studyTransparency);
            //transparentStimuli[i].tag = "Untagged";

                    if (enableLogging) stimuli[i].AddComponent<SimpleObjectLogger>();
    }


    void TurnOffbutton()
    {
        //Have to reset fingertouch or else it's still true when the button gets reactivated in the next trial
        //button.transform.GetComponentInChildren<DetectTouch>().fingerTouch = false;
        //button.transform.GetComponentInChildren<DetectTouch>().thumbTouch = false;
        // ??? Is GetComponentInChildren more efficient than Find with 2 child objects?
        // ??? What's the proper way to change components of child objects? I'm already loading button, but I need to change the material. Here I'm changing RGB values of the existing material. Should I change to a different material?
        //button.transform.FindChild("ButtonSwitch").transform.renderer.material.color = new UnityEngine.Color(button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.r - .5f, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.g, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.b, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.a);
        //button.transform.FindChild("ButtonSwitch").audio.PlayOneShot(Resources.Load("ButtonSound") as AudioClip);
    }

    void TurnOnbutton()
    {
        //button.transform.FindChild("ButtonSwitch").transform.renderer.material.color = new UnityEngine.Color(button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.r + .5f, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.g, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.b, button.transform.FindChild("ButtonSwitch").transform.renderer.material.color.a);
    }

}