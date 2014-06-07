//TO DO
//Make it so you can move objects up and down
//Make it so objects don't go through walls
//Transfer the part where it counts stimuli to the Stimuli script
//Get it to output starting and finish position

#pragma strict

//Imports tools to use generic lists
import System.Collections.Generic;

//Initializes variables
var grabRange = 3.0; //Not sure what GrabRange does, as you can grab it from any distance no matter what you set it at

//Sets private variables to be used within this scipt
private var resetParent : GameObject = null; 
private var grabbedObject : GameObject = null;
private var rigid : Rigidbody = null;
private var stimNumber = 5.0;

static var resetflag = false; //Initiates a flag that will change to true once all the objects have been touched

//Runs this function every frame
function FixedUpdate () {
    //If you're NOT holding the object and the trigger is being pressed
    if(grabbedObject==null&&Input.GetButton("Fire1")) {
        //Defines the hitInfo variable using the RaycastHit function
        var hitInfo : RaycastHit;
        //If the raycast hits an object in range
        if(Physics.Raycast(transform.position, transform.forward, hitInfo, grabRange)) {
            Debug.Log("Object grabbed");
            //Assigns the properties of the thing the raycast hit to grabbedObject
            grabbedObject = hitInfo.rigidbody.gameObject;
            //Identifies the current parent of the grabbed object, so it knows what to reset it to later (?)
            resetParent = grabbedObject.transform.parent.gameObject;
            //Makes the grabbed object a child of the player controller
            grabbedObject.transform.parent = this.transform;
            
        }
    }
    //If you ARE holding the object and you release the trigger
    if(grabbedObject!=null && !Input.GetButton("Fire1")) {
        Debug.Log("Object Released");
        //Changes the parent of the grabbed object back to its original parent
        grabbedObject.transform.parent = resetParent.transform;
        //Resets the grabbedObject variable
        grabbedObject = null;
    }
}
//Makes the script also execute in edit mode
@script ExecuteInEditMode()