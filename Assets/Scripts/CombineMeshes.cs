using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshes : MonoBehaviour
{


    // Runs once at the start, when you hit play
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;
            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.active = true;
        //Instantiates a list of CombineInstance objects, which will each represent a mesh that should be combined.
        //We're going to populate this list with each mesh we want to merge
        //List<CombineInstance> combinedMeshes = new List<CombineInstance>();
        //Adds the sphere and the tube to the list of combined meshes
        //GameObject newsphere = GameObject.Find("HalfSphere");
        //MeshFilter spheremesh = GameObject.Find("HalfSphere").GetComponent("MeshFilter");
        //combinedMeshes.Add(GameObject.Find("HalfSphere").GetComponent("MeshFilter"));

        //    //Creates a sphere
        //        GameObject origin = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        //        //ADDED - Scales the created sphere down to the walkintensity scale 
        //        origin.transform.localScale = walkIntensityRange;
        //        //ADDED - noves the sphere to 0,0,0
        //        origin.transform.position = new Vector3(0, 0, 0);
        //        //Creates a sphere mesh filter, which gives the object a sphere collider as well
        //        MeshFilter originMesh = origin.GetComponent<MeshFilter> ();
        //        //Initializes a CombineInstance
        //        CombineInstance originInst = new CombineInstance ();
        //        //Populating the same object type with a mesh and a transform. We're transforming the mesh to be relative to the world matrix of the original mesh
        //        //Basically telling Unity we want all of the mesh coordinates to be transformed to absolute coordinates instead of relative ones
        //        originInst.mesh = originMesh.mesh;
        //        originInst.transform = originMesh.transform.localToWorldMatrix;
        //        combinedMeshes.Add (originInst);
        //        //This command gets rid of the sphere object, leavi


        ////This command gets rid of the sphere object, leaving only the mesh
        //DestroyObject(origin);
        ////For every walker (every different direction the newly generated meshes move in)
        //for (int i = 0; i < numberOfWalkers; i++)
        //{
        //    //Creates a position vector that is initialized to the current position of the parent object (the object the script is attached to)
        //    Vector3 position = this.transform.position;
        //    //Returns a random position somewhere inside a sphere with a radius of 1 and the center at 0
        //    Vector3 directionvector = Random.insideUnitSphere;
        //    //For every walk (every time the walkers move)
        //    Debug.Log("Original position is " + position);
        //    for (int j = 0; j < numberOfWalks; j++)
        //    {
        //        //Updates the position as the one from the last walk
        //        Vector3 prevPosition = position;
        //        //Debug.Log("prevPosition is now " + prevPosition);
        //        //Returns a random position somewhere intisde a sphere with a radius of 1 and the center at 0
        //        //Basically generatesa  random number
        //        Vector3 rand = Random.insideUnitSphere;
        //        //Adds the randomly generated value to the directionvector to give it some randomness
        //        directionvector += rand;
        //        //Defines the coordinates the walker is going to walk to. Walks it from the position of the sphere just walke to
        //        //Normalizes the directionvector so that the intensity is taken care of by walkIntensityRange
        //        position.x += directionvector.normalized.x * walkIntensityRange.x;
        //        position.y += directionvector.normalized.y * walkIntensityRange.y;
        //        position.z += directionvector.normalized.z * walkIntensityRange.z;
        //        //Creates a new sphere and a cylinder at the new coordinates 
        //        GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //        //ADDED - trying to scale the created cylinder and sphere to the walk intensity range 
        //        sph.transform.localScale = walkIntensityRange;
        //        cyl.transform.localScale = walkIntensityRange;

        //        //Moves the cylinder halfway to the new coordinates so that it connects the old sphere with the new one
        //        cyl.transform.position = (prevPosition - position) / 2.0f + position;
        //        //Copies the localScale and other information to the cylinder connnecting the two spheres
        //        //Currently not doing anything because the scale is (1,1,1)
        //        Vector3 sca = cyl.transform.localScale;
        //        sca.y = (prevPosition - position).magnitude * 0.5f;
        //        cyl.transform.localScale = sca;
        //        //Rotates the cylinder so that it actually connects the two spheres
        //        cyl.transform.rotation = Quaternion.FromToRotation(Vector3.up, prevPosition - position);
        //        //Moves the sphere to its new position
        //        Debug.Log("position of sph is " + position);
        //        sph.transform.position = position;
        //        //As we make each object, sets it as a child of the parent object
        //        //Currently unnecessary because we later merge and delete the objects
        //        sph.transform.parent = this.transform;
        //        //Creates mesh filters for the newly created sphere and cylinder
        //        MeshFilter sphMesh = sph.GetComponent<MeshFilter>();
        //        MeshFilter cylMesh = cyl.GetComponent<MeshFilter>();
        //        //Initializes a new CombineInstance, which will be used to combine the new sphere to the existing stimulus mesh
        //        CombineInstance sphInst = new CombineInstance();
        //        //??? Makes a copy of the spheres mesh?
        //        sphInst.mesh = sphMesh.mesh;
        //        //Just like earlier, changes from relative to absolute coordinates
        //        sphInst.transform = sph.transform.localToWorldMatrix;
        //        //Adds the sphere mesh to the existing one
        //        combinedMeshes.Add(sphInst);
        //        //Initializes a new CombineInstance, which will be used to combine the new cylinder to the existing stimulus mesh
        //        CombineInstance cylInst = new CombineInstance();
        //        cylInst.mesh = cylMesh.mesh;
        //        cylInst.transform = cyl.transform.localToWorldMatrix;
        //        //Adds the sphere mesh to an array of existing meshes (combinedMeshes)
        //        combinedMeshes.Add(cylInst);
        //        //Destroys the creates sphere and cylinder, leaving only the new mesh					
        //        DestroyObject(sph);
        //        DestroyObject(cyl);

        //    }
        //}
        //// Combines all the meshes in the combinedMeshes array
        //transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combinedMeshes.ToArray());
        ////Optimizes the triangles of the mesh by caching them locally, which I know exactly enough about computers to not understand
        ////Basically trade better runtime performance for longer load times
        //transform.GetComponent<MeshFilter>().mesh.Optimize();
        ////Adds a texture to the gameobject
        //gameObject.renderer.material.mainTexture = myTexture;
        ////Changes the shader
        ////gameObject.renderer.material.shader = Shader.Find ("Self-Illumin/Diffuse");
        //gameObject.renderer.material.shader = Shader.Find("Specular");

        ////??? - Couldn't figure out how to add a mesh collider to the object
        ////MeshCollider stimMesh = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        ////Moves the object to a random position on the tabletop
        //transform.position = new Vector3(Random.Range(-.33F, .33F), Random.Range(-.1F, .1F), Random.Range(-.33F, .33F));
        ////Debug.Log (myTexture);
    }
}