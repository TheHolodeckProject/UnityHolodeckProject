using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class NBodySimulationStateManager : MonoBehaviour {
    public GameObject NBodySimulationObject;
    public GameObject PlayerObject;
    public GameObject VelocityMarkerObject;
    public GameObject TargetPrefab;
    public int maxNumberOfRounds = 3;
    public float roundDurationInMilliseconds = 30000;
    public Vector4 targetLocationRangeXYZR;

    //Variables for config viewing only
    public float currentScore;
    public float scoreMin;
    public NBodySimulationState state;

    private GameObject targetInstance;

    private Vector3 targetLoc;
    private List<float> scoreHistory;
    private int numberOfRoundsCompleted;
    private Stopwatch stopwatch;
    public enum NBodySimulationState { Initialize=0, InitializeNewRound, WaitForRoundStartTrigger, WaitForRoundToComplete, ScoreRound, WaitForContinueTrigger, Shutdown}

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
                if(targetInstance != null)
                    DestroyObject(targetInstance);
                targetLoc = (UnityEngine.Random.insideUnitSphere * targetLocationRangeXYZR.w) + new Vector3(targetLocationRangeXYZR.x, targetLocationRangeXYZR.y, targetLocationRangeXYZR.z);
                targetInstance = (GameObject)GameObject.Instantiate(TargetPrefab);
                targetInstance.transform.parent = NBodySimulationObject.transform;
                targetInstance.transform.localPosition = targetLoc;
                scoreMin = float.MaxValue;
                state = NBodySimulationState.WaitForRoundStartTrigger;
                break;
            case NBodySimulationState.WaitForRoundStartTrigger:
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    stopwatch.Reset();
                    stopwatch.Start();
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
                UnityEngine.Debug.Log("Round: " + numberOfRoundsCompleted + ", Score: " + scoreMin);
                state = NBodySimulationState.WaitForContinueTrigger;
                break;
            case NBodySimulationState.WaitForContinueTrigger:
                if (Input.GetKeyUp(KeyCode.Space)) state = NBodySimulationState.InitializeNewRound;
                break;
            case NBodySimulationState.Shutdown:
                //Do whatever shutdown procedure may be appropriate
                break;
        }
	}
}
