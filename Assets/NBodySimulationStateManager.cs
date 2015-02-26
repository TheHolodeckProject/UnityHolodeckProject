using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class NBodySimulationStateManager : MonoBehaviour {
    public GameObject NBodySimulationObject;
    public GameObject NBodySimulationPrefab;
    public GameObject PlayerObject;
    public GameObject VelocityMarkerObject;
    public GameObject TargetPrefab;
    public int maxNumberOfRounds = 3;
    public float roundDurationInMilliseconds = 30000;
    public Vector4 targetLocationRangeXYZR;

    public GameObject PlayerPlacementBoundry;
    public GameObject TargetPlacementBoundry;

    public AudioClip[] instructionAudioClips;
    public AudioSource instructionAudioSource;

    //Variables for config viewing only
    public float currentScore;
    public float scoreMin;
    public NBodySimulationState state;

    private GameObject targetInstance;

    public KeyCode nextKey = KeyCode.Space;
    public GameObject nextButtonManagerObject;

    private Vector3 targetLoc;
    private List<float> scoreHistory;
    private int numberOfRoundsCompleted;
    private Stopwatch stopwatch;
    public enum NBodySimulationState { Initialize=0, InitializeNewRound, ResetObjectPositions, WaitForShipPlacementTrigger, WaitForVelocityMarkerPlacementTrigger, WaitForRoundStartTrigger, WaitForRoundToComplete, ScoreRound, WaitForContinueTrigger, Shutdown}

	// Use this for initialization
	void Start () {
        state = NBodySimulationState.Initialize;
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case NBodySimulationState.Initialize:
                scoreHistory = new List<float>();
                numberOfRoundsCompleted = 0;
                targetInstance = null;
                stopwatch = new Stopwatch();
                state = NBodySimulationState.InitializeNewRound;
                break;
            case NBodySimulationState.InitializeNewRound:
                PlayerObject.transform.parent = PlayerPlacementBoundry.transform;
                VelocityMarkerObject.transform.parent = TargetPlacementBoundry.transform;
                VelocityMarkerObject.SetActive(true);
                PlayerPlacementBoundry.SetActive(true);
                TargetPlacementBoundry.SetActive(true);
                PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled= true;
                TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                PlayerObject.GetComponent<TrailRenderer>().enabled = false;
                GameObject.Destroy(NBodySimulationObject);
                NBodySimulationObject = (GameObject)GameObject.Instantiate(NBodySimulationPrefab);
                NBodySimulationObject.GetComponent<NBodySimulation>().playerPrefab = PlayerObject;
                NBodySimulationObject.GetComponent<NBodySimulation>().playerTarget = VelocityMarkerObject;
                instructionAudioSource.clip = instructionAudioClips[0];
                instructionAudioSource.Play();

                UnityEngine.Debug.Log("Playing first instruction audio.");

                if(targetInstance != null)
                    DestroyObject(targetInstance);
                targetLoc = (UnityEngine.Random.insideUnitSphere * targetLocationRangeXYZR.w) + new Vector3(targetLocationRangeXYZR.x, targetLocationRangeXYZR.y, targetLocationRangeXYZR.z);
                targetInstance = (GameObject)GameObject.Instantiate(TargetPrefab);
                targetInstance.transform.parent = NBodySimulationObject.transform;
                targetInstance.transform.localPosition = targetLoc;
                scoreMin = float.MaxValue;

                state = NBodySimulationState.ResetObjectPositions;
                break;
            case NBodySimulationState.ResetObjectPositions:
                PlayerObject.transform.localPosition = Vector3.zero;
                VelocityMarkerObject.transform.localPosition = new Vector3(0f, 0f, 0.2f);
                state = NBodySimulationState.WaitForShipPlacementTrigger;
                break;
            case NBodySimulationState.WaitForShipPlacementTrigger:
                if (userStateAdvance())
                {
                    PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                    TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = true;

                    instructionAudioSource.clip = instructionAudioClips[1];
                    instructionAudioSource.Play();

                    UnityEngine.Debug.Log("Playing second instruction audio.");

                    state = NBodySimulationState.WaitForVelocityMarkerPlacementTrigger;
                }
               break;
            case NBodySimulationState.WaitForVelocityMarkerPlacementTrigger:
               if (userStateAdvance())
               {
                   PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                   TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;

                   instructionAudioSource.clip = instructionAudioClips[2];
                   instructionAudioSource.Play();

                   UnityEngine.Debug.Log("Playing third instruction audio.");

                   state = NBodySimulationState.WaitForRoundStartTrigger;
               }
               break;
            case NBodySimulationState.WaitForRoundStartTrigger:
                if (userStateAdvance())
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    PlayerPlacementBoundry.SetActive(false);
                    TargetPlacementBoundry.SetActive(false);

                    TrainRendererExtensions.Reset(PlayerObject.GetComponent<TrailRenderer>(), this);
                    PlayerObject.GetComponent<TrailRenderer>().enabled = true;
                    NBodySimulationObject.GetComponent<NBodySimulation>().StartSimulation();
                    state = NBodySimulationState.WaitForRoundToComplete;
                }
                break;
            case NBodySimulationState.WaitForRoundToComplete:
                currentScore = Vector3.Distance(PlayerObject.transform.localPosition, targetLoc);
                if (currentScore < scoreMin) scoreMin = currentScore;
                if (stopwatch.ElapsedMilliseconds >= roundDurationInMilliseconds)
                {
                    stopwatch.Stop();
                    state = NBodySimulationState.ScoreRound;
                }
                break;
            case NBodySimulationState.ScoreRound:
                scoreHistory.Add(scoreMin);
                numberOfRoundsCompleted++;
                NBodySimulationObject.GetComponent<NBodySimulation>().pauseSimulation = true;
                UnityEngine.Debug.Log("Round: " + numberOfRoundsCompleted + ", Score: " + scoreMin);

                instructionAudioSource.clip = instructionAudioClips[3];
                instructionAudioSource.Play();

                state = NBodySimulationState.WaitForContinueTrigger;
                break;
            case NBodySimulationState.WaitForContinueTrigger:
                if (userStateAdvance()) state = NBodySimulationState.InitializeNewRound;
                break;
            case NBodySimulationState.Shutdown:
                //Do whatever shutdown procedure may be appropriate
                break;
        }
	}

    private bool userStateAdvance()
    {
        if (Input.GetKeyUp(nextKey)) return true;
        //TODO: Add Button Conditional Code
        return false;
    }

    //TODO: Add Button State Change Code (and integrate into state machine)
    private void SetButtonState(bool on)
    {
        if (on)
        {

        }
        else
        {

        }
    }
}

public static class TrainRendererExtensions
{
    /// <summary>
    /// Reset the trail so it can be moved without streaking
    /// </summary>
    public static void Reset(this TrailRenderer trail, MonoBehaviour instance)
    {
        instance.StartCoroutine(ResetTrail(trail));
    }

    /// <summary>
    /// Coroutine to reset a trail renderer trail
    /// </summary>
    /// <param name="trail"></param>
    /// <returns></returns>
    static IEnumerator ResetTrail(TrailRenderer trail)
    {
        var trailTime = trail.time;
        trail.time = 0;
        yield return 0;
        trail.time = trailTime;
    }
}
