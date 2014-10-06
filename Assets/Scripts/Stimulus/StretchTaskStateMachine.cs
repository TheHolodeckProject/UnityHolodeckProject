using UnityEngine;
using System.Collections;

public class StretchTaskStateMachine : MonoBehaviour
{
    public GameObject StrechableCube;
    public int totalTrials = 3;
    public int numberOfStim = 1;
    private GameObject[] stimuli;
    private int currentTrial;
    enum State { TrialStart = 0, TrialIdle, TrialEnd, TaskEnd };
    private State currentState;

    // Use this for initialization
    void Start()
    {
        currentState = State.TrialStart;
        currentTrial = 0;

    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            // ??? Cases don't need { }? 
            case State.TrialStart:
                GenerateStimuli();
                currentState = State.TrialIdle;
                break;

            case State.TrialIdle:
                Debug.Log("Recalling");
                break;
        }
    }

    private void GenerateStimuli()
    {
        for (int i = 0; i < numberOfStim; i++)
        {
            Debug.Log("Generating Stimuli");
            //  stimuli[i] = Instantiate(Resources.Load("StretchableCube", typeof(GameObject));
            //stimuli[i] = (GameObject)Instantiate(StrechableCube);
  

        }
    }
}