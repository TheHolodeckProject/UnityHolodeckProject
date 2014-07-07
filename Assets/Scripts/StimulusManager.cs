using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StimulusManager : MonoBehaviour {

	public static int numberOfStimuli = 5; //Static value representing number of stimuli for task
	private GameObject[] stimuli; //List of game objects (populated on Start())
	private int[] activeStimIndicies = new int[numberOfStimuli]; //List of active stimuli indicies
	//Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
	public Vector3 resetPositionP0; //Position for obj0
	public Vector3 resetPositionP1; //Position for objN
	//Random Bounds determines the 3D box which bounds the possible random positions of each object
	//The point P0 determines one corner of the box while the point P1 determines the opposite corner
	public Vector3 randomBoundsP0; //One corner of the bounding box
	public Vector3 randomBoundsP1; //The opposite corner of the bounding box
	public Vector3 stimuliScale = new Vector3 (0.1f, 0.1f, 0.1f); //Override scale for all stimuli

	// Use this for initialization
	void Start () {
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
		
		//Position stimuli randomly according to settings (no overlaps)
		List<Rect> overlapCheckList = new List<Rect> ();
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
			                       stimuli[activeStimIndicies[i]].transform.localScale.x,
			                       stimuli[activeStimIndicies[i]].transform.localScale.z);
			if(boxesOverlapArray(newBox,overlapCheckList)) i--;
			else overlapCheckList.Add (newBox);
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




	// Update is called once per frame
	void Update () {
		objectTouchedCondition ();
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
