//TO DO
//Make it so you can move objects up and down
//Make it so objects don't go through walls
//Transfer the part where it counts stimuli to the Stimuli script
//Get it to output starting and finish position

#pragma strict

//Imports tools to use generic lists
import System.Collections.Generic;

//Sets the range at which you can grab
var grabRange = 2.0;
var holdDistance = 1.0;
//Sets private variables to be used within this scipt
private var grabbedObject : GameObject = null;
private var grabrotation = Vector3(0,0,0);
private var grabflag = false;
private var newx = float;
private var newy = float;
private var newz = float;
private var origRotation = Vector3(0,0,0);
private var playerRotation = Quaternion.identity;

private var playerDirection = Vector3(0,0,0);
private var playerRotx = float;
private var playerRoty = float;
private var playerRotz = float;
//ADDED
private var grabHolder : GameObject = null;
private var tempRot = Quaternion.identity;

static var resetflag = false; //Initiates a flag that will change to true once all the objects have been touched

//Runs this function every frame
function FixedUpdate () {
    
//If you're NOT holding the object and the trigger is being pressed
if(grabflag==false && Input.GetButton("Fire1")) {
	//Defines the hitInfo variable using the RaycastHit function
	var hitInfo : RaycastHit;
	//If the raycast hits an object in range that is a child of Stimuli
	if(Physics.Raycast(transform.position, transform.forward, hitInfo, grabRange) && hitInfo.transform.parent.gameObject.name == "Stimuli") {
	    Debug.Log("Object grabbed");
		//Sets the grabflag to true
	 	grabflag = true;
			//Identifies the grabbed object as the thing the raycast hit
		grabbedObject = hitInfo.transform.gameObject;
		//Gets the euler angles (XYZ) of the grabbed object
	    origRotation = grabbedObject.transform.eulerAngles;
	    //Initializes an empty transform to hold position and rotation info
	    grabHolder = hitInfo.transform.gameObject;
	    }
    }
    
          if (grabflag==true) {
          //Debug.Log("Moving Object");
			//Assigns the player's position to the object holder
			grabHolder.transform.position = transform.position;
			//Assigns the rotation of the various Oculus cameras to the object holder
			grabHolder.transform.rotation = Quaternion.Euler(GameObject.Find("CameraRight").transform.eulerAngles.x,GameObject.Find("OVRCameraController").transform.eulerAngles.y,GameObject.Find("CameraRight").transform.eulerAngles.z);
			//Moves the grabbed object to a position grabRange in front of the player, in the direction of the camera
            grabbedObject.transform.position = grabHolder.transform.position + grabHolder.transform.forward * grabRange;
			//Adds the relevant rotation angles to the object's original rotation
            var newx = origRotation.x + GameObject.Find("CameraLeft").transform.eulerAngles.x;
            var newy = origRotation.y + GameObject.Find("OVRCameraController").transform.eulerAngles.y;         
            var newz = origRotation.z + GameObject.Find("CameraLeft").transform.eulerAngles.z;
            //Sets the grabbed objects rotation
            grabbedObject.transform.eulerAngles = Vector3(newx,newy,newz);	
        }
        
    //If you ARE holding the object and you release the trigger
    if (grabflag==true && !Input.GetButton("Fire1")) {
            Debug.Log("Object Released");
        //Changes the grabflag to flase
        grabflag = false;
    }
    }
//Makes the script also execute in edit mode
@script ExecuteInEditMode()