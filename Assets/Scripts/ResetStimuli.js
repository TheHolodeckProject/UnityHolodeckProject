//TO DO
	//Figure out how to position objects around the circumference of a circle in the center of the maze
	//Animate the reset so that objects travel to their reset points
#pragma strict

//Sets a flag so that this script will check if it should run in every frame but only run once
private var onetime = false;

function Update () {
//If the resetflag set by the Grab script is set to true and this script hasn't run yet
if (Grab.resetflag && !onetime){

//transform.Find identifies children (each stimulus object is a child of the Stimulus parent object)
//transform.position moves an object instantly
transform.Find("BlueSphere").position = Vector3(2.76, -4.0, 1.58);
transform.Find("GreenCube").position = Vector3(4.56, -4.0, 1.75);
transform.Find("OrangeCylinder").position = Vector3(3.65, -4.0, -0.9);
transform.Find("YellowDiamond").position = Vector3(5.09, -4.0, 0.02);
transform.Find("YellowEgg").position = Vector3(2.08, -4.0, -0.11);

//ALTERNATE WAY OF DOING THIS: Loop through stimuli and grab vector3 positions from this list
//Defines a list of coordinates for where the objects will reset to
//private var resetPositions = new List.<Vector3>();
//resetPositions.Add(Vector3(2.8,-4.0,-0.15));
//resetPositions.Add(Vector3(2.87,-4.0,-1.5));
//resetPositions.Add(Vector3(4.84,-4.0,-1.8));
//resetPositions.Add(Vector3(5.87,-4.0,0.15));
//resetPositions.Add(Vector3(4.5,-4.0,-1.5));

//Changes the onetime flag so it will not run again
onetime = true;

}
}