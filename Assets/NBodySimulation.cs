using UnityEngine;
using System.Collections;

public class NBodySimulation : MonoBehaviour {
    public float gravityDistanceThreshold = 0.0f; //km
    public float G = 667384f; //((km^(3))*(kg^(-1))*(s^(-2))) 6.67384e-20f
    public float timestep = 0.01f;
    public GameObject[] templateObjects;
    public Vector3[] initialPositions;
    public Vector3[] initialVelocities;
    public float[] masses;
    public float renderScale = 0.1f;

    public bool pauseSimulation = false;

    private GBody[] bodies; //Body field controls all the gravitational bodies
    private NumericalMethod numericalMethod = euler;
    private bool isValidSimulation = false;

	// Use this for initialization
	void Start () {
        //Validate that the nbody configuration is valid (just requirest initial condition arrays to be same length)
        if ((templateObjects.Length != initialPositions.Length) || (templateObjects.Length != initialVelocities.Length) || (templateObjects.Length != masses.Length))
        {
            Debug.LogError("NBody Simulation Requires that template objects, initial positions, initial velocities, and masses all be the same length.");
            return;
        }

        bodies = new GBody[templateObjects.Length];
        for (int i = 0; i < bodies.Length; i++){
            GameObject renderObject = (GameObject)GameObject.Instantiate(templateObjects[i]);
            renderObject.transform.parent = this.transform;
            bodies[i] = new GBody(initialPositions[i], initialVelocities[i], masses[i], renderObject, renderScale);
        }

        isValidSimulation = true;
	}

    //Update given a dt (this is where the main computation for the simulation happens)
	void FixedUpdate () {
        if (isValidSimulation && !pauseSimulation)
        {
            Vector3[][] forceArray = new Vector3[this.bodies.Length][]; //Array of computed forces
            for (var i = 0; i < this.bodies.Length; i++)
            { //Compute each force
                forceArray[i] = new Vector3[this.bodies.Length];
                var netForce = new Vector3(0, 0, 0); //Generate a net force for an object
                for (var j = 0; j < this.bodies.Length; j++)
                { //For each object
                    forceArray[i][j] = this.bodies[i].calculateForce(this.bodies[j], gravityDistanceThreshold, G);
                    netForce.x += forceArray[i][j].x;
                    netForce.y += forceArray[i][j].y;
                    netForce.z += forceArray[i][j].z;
                }
                var new_x_vals = numericalMethod(netForce.x / this.bodies[i].Mass, this.bodies[i].Position.x, this.bodies[i].Velocity.x, this.timestep);
                var new_y_vals = numericalMethod(netForce.y / this.bodies[i].Mass, this.bodies[i].Position.y, this.bodies[i].Velocity.y, this.timestep);
                var new_z_vals = numericalMethod(netForce.z / this.bodies[i].Mass, this.bodies[i].Position.z, this.bodies[i].Velocity.z, this.timestep);
                this.bodies[i].Position = new Vector3(new_x_vals.x, new_y_vals.x, new_z_vals.x);
                this.bodies[i].Velocity = new Vector3(new_x_vals.y, new_y_vals.y, new_z_vals.y);
            }
        }
	}

    private delegate Vector2 NumericalMethod(float a, float r, float v, float dt);

    private static Vector2 rk4(float a, float r, float v, float dt){
	    float a0    = a* r;
	    float a1    = a*(r + 0.5f*dt*v + 0.125f*dt*dt*a0);
	    float a2    = a*(r +      dt*v + 0.500f*dt*dt*a1);
	    float new_r =    r +      dt*v + ((a0+2f*a1)*dt*dt)/6f;
	    return new Vector2(new_r, v);
    }

    private static Vector2 euler(float a, float r, float v, float dt){
	    float new_v = v+a*dt;
	    float new_r = r+v*dt+0.5f*a*dt*dt;
	    return new Vector2(new_r, new_v);
    }

    private static Vector2 heun(float a, float r, float v, float dt){
	    float new_r = r+2*v*dt;
	    return new Vector2(new_r, v);
    }
}

//An object representing a single gravitational object
class GBody{
    private Vector3 pos;
    private Vector3 vel;
    private float m;

    private GameObject obj;

    public GBody(Vector3 position, Vector3 velocity, float mass, GameObject renderObject, float renderScale){
        pos = position;
        vel = velocity;
        m = mass;

        obj = renderObject;
        float logMass = Mathf.Log(mass+1);
        obj.transform.localScale = new Vector3(logMass * renderScale, logMass * renderScale, logMass * renderScale);
    }

    public Vector3 calculateForce(GBody body, float gravityDistanceThreshold, float G){ //Simple force calculation function
        //Calculate the changes in x and y direction and distance
        float dx = body.pos.x-this.pos.x;
		float dy = body.pos.y-this.pos.y;
		float dz = body.pos.z-this.pos.z;
		float d = Mathf.Sqrt(dx*dx+dy*dy+dz*dz);
		//If the distances is too close, do not calculate force (prevents singularities)
		if(d<gravityDistanceThreshold) return new Vector3(0,0,0);
		//Create a force vector and return it
		float Fmodified = (this.m*body.m*G)/(d*d*d); //third 'd' is for unit vector
		dx*=Fmodified;
		dy*=Fmodified;
		dz*=Fmodified;
		return new Vector3(dx,dy,dz);
    }

    public Vector3 Position{
        get{return this.pos;}
        set{this.pos = value;
            this.obj.transform.localPosition = value;
        }
    }

    public Vector3 Velocity{
        get{return this.vel;}
        set{this.vel = value;}
    }

    public float Mass{
        get{return this.m;}
    }
}