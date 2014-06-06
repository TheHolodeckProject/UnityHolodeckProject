//TO DO
	//Make it so you can move objects up and down
	//Make it so objects don't go through walls
	//Transfer the part where it counts stimuli to the Stimuli script
	//Get it to output starting and finish position

#pragma strict

//Imports tools to use generic lists
import System.Collections.Generic;

//Initializes variables
var grabRange = 1.0; //Not sure what GrabRange does, as you can grab it from any distance no matter what you set it at
var holdDistance = 0.1; //How far away the object is held from your character
var grabKeys : List.<KeyCode> = new List.<KeyCode>();
private var resetParent = new GameObject(); //How far away the object is held from your character
private var grabbedObject = new GameObject();

static var resetflag = false; //Initiates a flag that will change to true once all the objects have been touched

//Defines the 3 possible values of GrabState
enum GrabState {Free, Hold};

//Sets private variables to be used within this scipt
private var rigid : Rigidbody = null;
private var grabState : GrabState = 0; //Sets the default grabstate variable
private var stimNumber = 5.0;
 
//Runs this function every frame
function FixedUpdate () {
	//If you're NOT holding the object		
	var grabEventDown = false;
	var grabEventUp = false;
	for(var i = 0; i < grabKeys.Count;i++){
		grabEventUp = grabEventUp || Input.GetKeyUp(grabKeys[i]);
		grabEventDown = grabEventDown || Input.GetKeyDown(grabKeys[i]);
	}
	
	
	if(grabbedObject==null&&grabEventDown) {
		//Defines the hitInfo variable using the RaycastHit function (?)
		var hitInfo : RaycastHit;
		//If the ray hits an object (?)
		if(Physics.Raycast(transform.position, transform.forward, hitInfo, grabRange)) {
			grabbedObject = hitInfo.rigidbody.gameObject;
			resetParent = grabbedObject.transform.parent.gameObject;
			grabbedObject.transform.parent = this.transform;
		}
	}
    //If you ARE holding the object
    if(grabbedObject!=null && grabEventUp){
        grabbedObject.transform.parent = resetParent.transform;
        //DEBUGGING
//        Debug.Log("Player Rotation = " + this.transform.rotation);
  //      Debug.Log("OVRCamera rotation=  " + GameObject.Find("OVRCameraController").transform.rotation);
   		grabbedObject = null;
    }
}
//Makes the script also execute in edit mode
@script ExecuteInEditMode()