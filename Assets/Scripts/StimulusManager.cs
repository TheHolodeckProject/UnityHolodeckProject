using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StimulusManager : MonoBehaviour {

	private int numberOfStimuli; //Static value representing number of stimuli for task
	private GameObject[] stimuli; //List of game objects (populated on Start())
	private int[] activeStimIndicies; //List of active stimuli indicies
	//Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
	public Vector3 resetPositionP0; //Position for obj0
	public Vector3 resetPositionP1; //Position for objN
	//Random Bounds determines the 3D box which bounds the possible random positions of each object
	//The point P0 determines one corner of the box while the point P1 determines the opposite corner
	public Vector3 randomBoundsP0; //One corner of the bounding box
	public Vector3 randomBoundsP1; //The opposite corner of the bounding box
	public Vector3 stimuliScale = new Vector3 (0.1f, 0.1f, 0.1f); //Override scale for all stimuli
	public Vector3 overlapPaddingFactor = new Vector3(0.5f,0.5f,0.5f);

	// Use this for initialization
	void Start () {
		numberOfStimuli = PlayerPrefs.GetInt("Number of Stimuli");
		activeStimIndicies = new int[numberOfStimuli];
		expectedNumberOfTrials = PlayerPrefs.GetInt ("Number of Trials");

		//Generate list of stimuli which are available
		List<GameObject> objs = new List<GameObject> ();
		foreach (Transform child in transform) {
			//Set each stimulus scale to be the override setting the stimulus manager contains
					child.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
					child.localScale = stimuliScale;
					objs.Add (child.gameObject);
				}
		stimuli = objs.ToArray ();

		//Create a knuth shuffle index list of random indicies within the range of possible stimuli
		int[] knuthShuffleList = new int[stimuli.Length];
		for (int i = 0; i < knuthShuffleList.Length; i++)
						knuthShuffleList [i] = i;
		for (int i = 0; i < stimuli.Length; i++) {
			int index = Random.Range(i,stimuli.Length-1);
			int tmp = knuthShuffleList[index];
			knuthShuffleList[index] = knuthShuffleList[i];
			knuthShuffleList[i] = tmp;
				}

		//Choose random stimuli to be the active ones
		for (int i = 0; i < numberOfStimuli; i++)
						activeStimIndicies [i] = knuthShuffleList [i];

		//Set activeated stimuli to be active game objects (all others are inactive by default)
		for (int i = 0; i < activeStimIndicies.Length; i++)
			stimuli [activeStimIndicies [i]].GetComponentInChildren<MeshRenderer>().enabled = true;
		
		generateRandomPositions ();

	}
	private int numRetries = 10;
	void generateRandomPositions(){
		//Position stimuli randomly according to settings (no overlaps)
		List<Rect> overlapCheckList = new List<Rect> ();
		int retries = numRetries;
		for (int i = 0; i < activeStimIndicies.Length; i++) {
			stimuli [activeStimIndicies [i]].transform.localPosition = new Vector3 (
				Random.Range (randomBoundsP0.x > randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x,
			              randomBoundsP0.x < randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x),
				Random.Range (randomBoundsP0.y > randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y,
			              randomBoundsP0.y < randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y),
				Random.Range (randomBoundsP0.z > randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z,
			              randomBoundsP0.z < randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z));
			//Check for overlapping boxes and regenerate box location if overlap occurs
			Rect newBox = new Rect(stimuli[activeStimIndicies[i]].transform.localPosition.x,
			                       stimuli[activeStimIndicies[i]].transform.localPosition.z,
			                       stimuli[activeStimIndicies[i]].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x*stimuli[activeStimIndicies[i]].transform.localScale.x,
			                       stimuli[activeStimIndicies[i]].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z*stimuli[activeStimIndicies[i]].transform.localScale.z);
			if(boxesOverlapArray(newBox,overlapCheckList)){
				retries--;
				if(retries <= 0){
					//Force quit and overlap to prevent hang
					retries = numRetries;
					overlapCheckList.Add (newBox);
					Debug.Log ("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
				}
				else i--;
			}
			else{
				retries = numRetries;
				overlapCheckList.Add (newBox);
			}
		}
	}

	bool boxesOverlapArray(Rect box, List<Rect> boxArray) {
		for (var i = 0; i < boxArray.Count; i++)
			if (boxesOverlap(box, boxArray[i])) return true;
		return false;
	}
	
	//Reference: http://gamemath.com/2011/09/detecting-whether-two-boxes-overlap/
	bool boxesOverlap(Rect box0, Rect box1) {
		if (box0.x + box0.width < box1.x) return false;
		if (box0.x > box1.x + box1.width) return false;
		if (box0.y + box0.height < box1.y) return false;
		if (box0.y > box1.y + box1.height) return false;
		return true;
	}



	private int phase = 0;
	private float phaseWaitTime = 0.500f; //in s
	private float phaseWaitTimeStart;
	private bool phaseInit = false;
	private float timerStart;
	private float testTime = 5.000f; //in s
	private float restTime = 3.000f; //in s
	private int numberOfCompletedTrials = 0;
	private int expectedNumberOfTrials;
	// Update is called once per frame
	void Update () {
				if (phase == 0) {
						objectTouchedCondition ();

						int numObjectsTouched = 0;
						for (int i = 0; i < activeStimIndicies.Length; i++)
								if (stimuli [activeStimIndicies [i]].GetComponent <StimuliBehavior> ().touched)
										numObjectsTouched++;

						if (numObjectsTouched == activeStimIndicies.Length){
								phase = 1;
								phaseWaitTimeStart = Time.time;
								phaseInit = true;
						}
				}
				if (phase == 1) {
					if(Time.time - phaseWaitTimeStart>=phaseWaitTime){
						if(phaseInit){
							//Reset stimuli positions
							for(int i = 0; i < activeStimIndicies.Length;i++)
								stimuli[activeStimIndicies[i]].transform.localPosition = Vector3.Lerp (resetPositionP0,resetPositionP1,(((float)i)*(1f/((float)activeStimIndicies.Length))));
							timerStart = Time.time;
							phaseInit = false;
						}
						if(Time.time-timerStart<testTime){
							//Test is going
							labelString = (testTime - (Time.time-timerStart))+"";
						}
						else{
							labelString = "Test Over";
							phase = 2;
							phaseWaitTimeStart = Time.time;
							phaseInit = true;
						}
					}
				}
				if (phase == 2) {
					if(Time.time - phaseWaitTimeStart>=phaseWaitTime){
						if(phaseInit){
							timerStart = Time.time;
							phaseInit = false;
						}
						if(Time.time-timerStart<restTime){
							//Test is going
							labelString = "Resetting in " + (restTime - (Time.time-timerStart));
						}
						else{
							numberOfCompletedTrials++;
							if(numberOfCompletedTrials<expectedNumberOfTrials){
								labelString = "Click Objects";
								phase = 0;
								for (int i = 0; i < activeStimIndicies.Length; i++)
									stimuli [activeStimIndicies [i]].GetComponent <StimuliBehavior> ().touched = false;
								generateRandomPositions();
								phaseWaitTimeStart = Time.time;
								phaseInit = true;
							}
							else{
								//Trials all complete
								Application.LoadLevel (1);
							}
						}
					}
				}
		}

	private string labelString = "Click Objects";
	void OnGUI () {
		// Make a background box
		GUI.Label(new Rect(10,10,100,90), labelString);
	}

	void objectTouchedCondition(){
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				for(int i = 0; i < activeStimIndicies.Length;i++){
					if(hit.collider.gameObject == stimuli[activeStimIndicies[i]]){
						stimuli[activeStimIndicies[i]].GetComponent<StimuliBehavior>().touched = true;
					}
				}
			}
		}
	}
}
