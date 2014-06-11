//Figure out how to apply mesh collider
//Figure out how to combine meshes OR combine the scripts into one
//Get the walks to turn

using UnityEngine;
using System.Collections;

public class GenerateTube : MonoBehaviour
{
	//Defines how many times the tube will curve
	public int walks = 1;
	public int walklength = 1;
    //Defines the radius of the circle
    public float radius = 1f;
    // Defines how many squares go around the middle of the sphere. Basically, the resolution of the sphere.
    public int StimRes = 5;

	//Defines the texture to be used for the sphere
	Texture stimTexture;

    void Start()
    {
        //Adds a mesh filter and renderer to the empty gameobject
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        //??? Creates a mesh from the meshfilter?
        Mesh mesh = filter.mesh;
        //??? Clears the mesh?
        mesh.Clear();
        //Assigns the texture to the mesh
        renderer.material.mainTexture = stimTexture;
		renderer.material.SetColor("_SpecColor", Color.white);
        //Initializes a vector3 array that will hold the vectors of the tube mesh
        Vector3[] vertices = new Vector3[((StimRes + 1) * (walks+1))];
        //Debug.Log("Length of vertices is " + vertices.Length);
        //Defines pi
        float pi = Mathf.PI; 
        //For every walk
        for (int walk = 0; walk <= walks; walk++)
        {
			//For every side of the tube
            for (int lon = 0; lon <= StimRes; lon++)
            {
				//Chops up a circle around the top of the circle into slices,then finds the Sin and Cos of each angle
				float a1 = (2 * pi) * (float)(lon == StimRes ? 0 : lon) / StimRes;
			    float sin1 = Mathf.Sin(a1);
		        float cos1 = Mathf.Cos(a1);

				//Defines which vertex we're defining
				int vertexindex = lon + walk * (StimRes+1);
				//Adds the vertices of that triangle to the vertices array
               	vertices[vertexindex] = new Vector3(cos1*radius, walklength/(walk+1), sin1*radius);
			}
        }

        //Creates the normals of the mesh. Normals are like poles sticking out of every vertex
        //Initializes a new vector3 array to hold the normals
        Vector3[] normales = new Vector3[vertices.Length];
        //For every vertex
        for (int n = 0; n < vertices.Length; n++)
            //??? Not sure whty this is normalized
            normales[n] = vertices[n].normalized;
        //Creates the triangles
        //Defines the number of faces
        int nbFaces = vertices.Length;
        //Defines the number of triangles there'll be
        int nbTriangles = nbFaces * 2;
        //Defines the number of indexes there will be
        int nbIndexes = nbTriangles * 3;
        //??? Not sure what this is doing
        int[] triangles = new int[nbIndexes];
  
		//Initializes a counter
        int i = 0;
          //For every walk
          for (int walk = 0; walk < walks; walk++)
          {
              //For every face of the tube
              for (int lon = 0; lon < StimRes; lon++)
              {
				int current = lon + walk * (StimRes + 1);
				int next = current + StimRes + 1;
                  //Defines the triangles
                  triangles[i++] = current;
                  triangles[i++] = current + 1;
                  triangles[i++] = next + 1;

                  triangles[i++] = current;
                  triangles[i++] = next + 1;
                  triangles[i++] = next;
              }
          }
		//Assigns all the things we created to the mesh we created earlier
        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}