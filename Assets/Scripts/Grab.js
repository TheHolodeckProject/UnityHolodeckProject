//TO DO
	//Make it so you can move objects up and down
	//Make it so objects don't go through walls
	//Transfer the part where it counts stimuli to the Stimuli script
	//Get it to output starting and finish position

//Imports tools to use generic lists
import System.Collections.Generic;

//Initializes variables
var grabRange = 1.0; //Not sure what GrabRange does, as you can grab it from any distance no matter what you set it at
var holdDistance = .2; //How far away the object is held from your character
var grabbedObjects = new List.<String>(); //Initializes a list that will hold the names of grabbed objects
static var resetflag = false; //Initiates a flag that will change to true once all the objects have been touched

//Defines the 2 possible values of GrabState
enum GrabState {Free, Hold};
//Sets private variables to be used within this scipt
private var rigid : Rigidbody = null;
private var grabState : GrabState = 0; //Sets the default grabstate variable
private var stimNumber = 5.0;
var objectforward = Vector3(0,0,0); 

//Runs this function every frame
function FixedUpdate () {
	//If you're NOT holding the object
    if(grabState == GrabState.Free) {
			//If you press the joystick
        if (Input.GetButtonDown("Trigger"))	{
//            Debug.Log("Trigger pressed");
        //Defines the hitInfo variable using the RaycastHit function (?)
        	var hitInfo : RaycastHit;
        	//If the ray hits an object (?)
            if(Physics.Raycast(transform.position, transform.forward, hitInfo)) {
								//If the object the ray hits has a rigidbody (?)
		        if(hitInfo.rigidbody) {
		        	//Defines the rigid variable and puts it in hitInfo.rigidbody
         	     	rigid = hitInfo.rigidbody;
         	     	
//Displays a message if the grabbedObjects list already contains the object
if(grabbedObjects.Contains(hitInfo.collider.gameObject.name)) {
 //Debug.Log(hitInfo.collider.gameObject.name + " is already in the list");
 }
else 
{
        	     	//If grabbedObjects doesn't already contain the item, adds it
         	     	grabbedObjects.Add(hitInfo.collider.gameObject.name);
//Debug.Log("Put " + hitInfo.collider.gameObject.name + " into the list");
 }
 
 //Checks if the list of grabbedObjects contains all the objects
 if (grabbedObjects.Count >= stimNumber && !resetflag) {
 resetflag = true;
Debug.Log("Resetting objects...");
 }
					//Declares that you're holding the variable now
          		 	grabState = GrabState.Hold;
                 	}
            	}
        	}
    	}
    //If you ARE holding the object
    else if(grabState == GrabState.Hold) {
        //Changes the position of the object you're holding to be holdDistance in front of you
        rigid.MovePosition(transform.position + transform.forward * holdDistance);

       //rigid.MovePosition(transform.Translate(transform.position,transform.rotation))

//        rigid.MovePosition(transform.position + this.transform.forward * holdDistance;
        //objectforward = transform.localRotation * new Vector3(0,1,1);
//        rigid.MovePosition(transform.position + (transform.rotation * objectforward); 
        //Vector3 newpos = transform.position + (transform.localRotation)
        //rigid.MovePosition(transform.position + new Vector3(transform.forward.x,transform.forward.y,transform.forward.z));
       //Debug.Log(transform.position);
       //Debug.Log(transform.rotation);
        //If the left mouse is not being pressed
        if(Input.GetButtonUp("Trigger")){
        //You stop holding the object
        grabState = GrabState.Free;
 //       Debug.Log("Trigger released");
    }
	}
}
//Makes the script also execute in edit mode
@script ExecuteInEditMode()