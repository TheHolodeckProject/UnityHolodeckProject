	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
//Must be run on a game object that has a Mesh Filter and a Mesh Renderer
//Running it on an object with a texture will give the created stimulus a texture, too

	public class RandomSquiggles : MonoBehaviour {
		
		public Vector3 walkIntensityRange = new Vector3(1f,1f,1f);
		public int numberOfWalkers = 3;
		public int numberOfWalks = 10;
		
		// Runs once at the start, when you hit play
		void Start () {
		//??? Not sure
			List<CombineInstance> combinedMeshes = new List<CombineInstance> ();
		//Creates a sphere at (0,0,0)	
		GameObject origin = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		//Creates a sphere mesh filter, which gives the object a sphere collider as well
		MeshFilter originMesh = origin.GetComponent<MeshFilter>();
			//Initializes a CombineInstance
			CombineInstance originInst = new CombineInstance();
		//??? Not sure
			originInst.mesh = originMesh.mesh;
			originInst.transform = originMesh.transform.localToWorldMatrix;
			combinedMeshes.Add (originInst);
		//This command gets rid of the sphere object, leaving only the mesh
			DestroyObject (origin);
		//For every walker (every different direction the newly generated meshes move in)
			for(int i = 0; i < numberOfWalkers;i++)
			{
			//??? Not sure
				Vector3 position = this.transform.position;
			//Returns a random position somewhere inside a sphere with a radius of 1 and the center at 0
				Vector3 velocity = Random.insideUnitSphere;
			//For every walk (every time the walkers move)
				for(int j = 0; j < numberOfWalks;j++)
				{
				//Updates the position as the one from the last walk
					Vector3 prevPosition = position;
				//Returns a random position somewhere intisde a sphere with a radius of 1 and the center at 0
				//Basically generatesa  random number
					Vector3 rand = Random.insideUnitSphere;
				//Adds the randomly generated value to the velocity to give it some randomness
					velocity += rand;
				//Defines the coordinates the walker is going to walk to. Walks it from the position of the sphere just walke to
				//??? Why does it normalize the velocity? 
					position.x +=  velocity.normalized.x * walkIntensityRange.x;
					position.y +=  velocity.normalized.y * walkIntensityRange.y;
					position.z +=  velocity.normalized.z * walkIntensityRange.z;
				//Creates a new sphere and a cylinder at the new coordinates 
					GameObject sph = GameObject.CreatePrimitive (PrimitiveType.Sphere);
					GameObject cyl = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
					//Moves the cylinder halfway to the new coordinates so that it connects the old sphere with the new one
					cyl.transform.position = (prevPosition-position)/2.0f + position;
				//??? Not sure what this localScale business is doing
					Vector3 sca = cyl.transform.localScale;
					sca.y = (prevPosition-position).magnitude*0.5f;
					cyl.transform.localScale = sca;
				//Rotates the cylinder so that it actually connects the two spheres
					cyl.transform.rotation = Quaternion.FromToRotation (Vector3.up,prevPosition-position);
				//Moves the sphere to its new position
					sph.transform.position = position;
				//??? Not sure
					sph.transform.parent = this.transform;
				//Creates mesh filters for the newly created sphere and cylinder
					MeshFilter sphMesh = sph.GetComponent<MeshFilter>();
					MeshFilter cylMesh = cyl.GetComponent<MeshFilter>();
				//Initializes a new CombineInstance, which will be used to combine the new sphere to the existing stimulus mesh
				CombineInstance sphInst = new CombineInstance();
				//??? Makes a copy of the spheres mesh?
					sphInst.mesh = sphMesh.mesh;
				//??? Not sure
					sphInst.transform = sph.transform.localToWorldMatrix;
				//Adds the sphere mesh to the existing one
					combinedMeshes.Add (sphInst);
				//Initializes a new CombineInstance, which will be used to combine the new cylinder to the existing stimulus mesh
				CombineInstance cylInst = new CombineInstance();
					cylInst.mesh = cylMesh.mesh;
					cylInst.transform = cyl.transform.localToWorldMatrix;
				//Adds the sphere mesh to an array of existing meshes (combinedMeshes)
				combinedMeshes.Add (cylInst);
//Destroys the creates sphere and cylinder, leaving only the new mesh					
					DestroyObject (sph);
					DestroyObject (cyl);
				}
			}
			// Combines all the meshes in the combinedMeshes array
			transform.GetComponent<MeshFilter> ().mesh.CombineMeshes (combinedMeshes.ToArray ());
		//Optimizes the triangles of the mesh by caching them locally, which I know exactly enough about computers to not understand
		//Basically trade better runtime performance for longer load times
			transform.GetComponent<MeshFilter> ().mesh.Optimize ();
		}

	}