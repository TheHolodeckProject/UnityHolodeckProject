using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//TO DO
// Configure Logger
// Transcribe instructions
// Add fade-in, fade-out animation -- ??? how to get the FadeObjectInOut script to work?

public class MentalRotationTask : MonoBehaviour
{
    public GameObject oculusCamera;
    public GameObject regularCamera;
    public GameObject resetButton;
    public GameObject practicePrefab;
    private GameObject[] targetPrefabs;
    private GameObject[] distractorPrefabs;
    private GameObject target;
    private GameObject[] distractors;
    public float stimScale;
    private bool enableLogging;
    private Logger logger;
    private Vector3[] stimLocations;
    public GameObject blackboard;
    private GameObject[] stimuli;
    private float timeRemaining;

    enum State { SampleStart, Sample, SampleEnd, PracticeStart, Practice, PracticeImagine, PracticeSomethingLikeThis, PracticePractice, PracticeEnd, TrialStart, TrialEnd };
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
        //enableLogging = PlayerPrefs.GetInt("LogDataYesNo") == 0 ? false : true;
        //if (enableLogging) logger = GameObject.Find("Logger").GetComponent<Logger>();
        //if (enableLogging) logger.BeginLogging();

        distractors = new GameObject[4];
        stimLocations = new Vector3[5];
        stimLocations[0] = new Vector3(0f, 1.1f, -.2f);
        stimLocations[1] = new Vector3(-.3f, .8f, -.2f);
        stimLocations[2] = new Vector3(-.15f, .8f, -.2f);
        stimLocations[3] = new Vector3(.15f, .8f, -.2f);
        stimLocations[4] = new Vector3(.3f, .8f, -.2f);

        //Define gameobject arrays - ??? More efficient way to do this?
        //practiceStim1 = new GameObject[]
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);

        //Press Esc to quit
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //Debug.Log(currentState);
        switch (currentState)
        {

            case State.PracticeStart:
                blackboard.GetComponentInChildren<Text>().text = "Welcome to the Holodeck\n\n\n\n Press the button to continue";
                if (resetButton.transform.GetComponentInChildren<DetectTouch>().fingerTouch)
                {
                    TurnOffResetButton();
                    currentState = State.PracticeImagine;
                    target = (GameObject)Instantiate(practicePrefab, stimLocations[0], Quaternion.Euler(30, 45, 20));
                    target.transform.localScale = new Vector3 (stimScale, stimScale, stimScale);
                    timeRemaining = 1f;
                }
                    break;


            case State.PracticeImagine:
                blackboard.GetComponentInChildren<Text>().text = "Check out this object \n\n Try to imagine what it would look like if it was rotated";
                timeRemaining = timeRemaining - Time.deltaTime;
                if (timeRemaining < 0)
                {
                    blackboard.GetComponentInChildren<Text>().text = "It might look something like this. \n\n All of these objects are the identical shape \n\n They've just been rotated";
                    
                    distractors[0] = (GameObject)Instantiate(practicePrefab, stimLocations[1], Quaternion.Euler(30, 45, 20));
                    distractors[1] = (GameObject)Instantiate(practicePrefab, stimLocations[2], Quaternion.Euler(120, 45, 90));
                    distractors[2] = (GameObject)Instantiate(practicePrefab, stimLocations[3], Quaternion.Euler(90, 0, 110));
                    distractors[3] = (GameObject)Instantiate(practicePrefab, stimLocations[4], Quaternion.Euler(45, 145, 0));

                    distractors[0].transform.localScale = new Vector3(stimScale, stimScale, stimScale);
                    distractors[1].transform.localScale = new Vector3(stimScale, stimScale, stimScale);
                    distractors[2].transform.localScale = new Vector3(stimScale, stimScale, stimScale);
                    distractors[3].transform.localScale = new Vector3(stimScale, stimScale, stimScale);
                    currentState = State.PracticeSomethingLikeThis;
                }
                break;

            case State.PracticeSomethingLikeThis:
                  break;
        }
    }



    private void GenerateStimuli()
    {
        for (var i = 0; i <= 3; i++)

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

    void TurnOffResetButton()
    {
        //Have to reset fingertouch or else it's still true when the resetButton gets reactivated in the next trial
        resetButton.transform.FindChild("Button").GetComponent<DetectTouch>().fingerTouch = false;
        resetButton.transform.FindChild("Button").transform.renderer.material.color = new UnityEngine.Color(resetButton.transform.FindChild("Button").transform.renderer.material.color.r - .5f, resetButton.transform.FindChild("Button").transform.renderer.material.color.g, resetButton.transform.FindChild("Button").transform.renderer.material.color.b, resetButton.transform.FindChild("Button").transform.renderer.material.color.a);
        resetButton.transform.FindChild("Button").audio.PlayOneShot(Resources.Load("ButtonSound") as AudioClip);
    }

    void TurnOnResetButton()
    {
        resetButton.transform.FindChild("Button").transform.renderer.material.color = new UnityEngine.Color(resetButton.transform.FindChild("Button").transform.renderer.material.color.r + .5f, resetButton.transform.FindChild("Button").transform.renderer.material.color.g, resetButton.transform.FindChild("Button").transform.renderer.material.color.b, resetButton.transform.FindChild("Button").transform.renderer.material.color.a);
    }


    bool MentalRotationFeedback()
    {
        ////If the stimuli have been moved back to their original position
        //if (stimuli[0].transform.position == stimLocations[0])
        //    return true;
        //for (int i = 0; i < trialNumberOfStim; ++i)
        //    stimuli[i].transform.position = Vector3.MoveTowards(stimuli[i].transform.position, stimLocations[i], Time.deltaTime * .1f);
        //return false;
        return false;
    }

    void ShowLogo()
    {
        GameObject screen = GameObject.Find("BlackScreen");
        screen.renderer.material = Resources.Load("Holodeck Logo") as Material;
    }

}