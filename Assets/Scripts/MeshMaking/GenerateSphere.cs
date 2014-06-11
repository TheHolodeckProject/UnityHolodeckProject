using UnityEngine;
using System.Collections;

//FIGURE OUT HOW TO CHANGE LIGHTING OF OBJECT TO SELF-ILLUM

public class GenerateSphere : MonoBehaviour {

//Defines the texture to be used for the sphere
public Texture stimTexture;
//Defines the radius of the circle
public float radius = 1f;
	public float modradius = 2f;
// Defines how many squares go around the middle of the sphere. Basically, the resolution of the sphere.
	//I couldn't make it go any higher than 255
public int StimRes = 255;

void Start () {

		//Adds a mesh filter and renderer to the empty gameobject
		MeshFilter filter = gameObject.AddComponent< MeshFilter > ();
		MeshRenderer renderer = gameObject.AddComponent< MeshRenderer > ();
		//??? Creates a mesh from the meshfilter?
		Mesh mesh = filter.mesh;
		//??? Clears the mesh?
		mesh.Clear();
		//Assigns the texture to the mesh
		renderer.material.mainTexture = stimTexture;

		//Initializes a vector3 array that will hold the vectors of this sphere
		Vector3[] vertices = new Vector3[(StimRes+1) * StimRes + 2];
        Debug.Log("Length of vertices is " + vertices.Length);
		//Defines pi
		float pi = Mathf.PI;
		//Defines the first vertex as the top of the sphere
		vertices[0] = Vector3.up * radius;
		//For every square
		for( int lat = 0; lat < StimRes; lat++ )
		{
			//Chops up a circle around the top of the circle into 256 slices,then finds the Sin and Cos of each angle
			float a1 = pi * (float)(lat+1) / (StimRes+1);
			float sin1 = Mathf.Sin(a1);
			float cos1 = Mathf.Cos(a1);

			//For every slice of that circle
			for( int lon = 0; lon <= StimRes; lon++ )
			{
				//??? Figures out the other angle of that triangle
				float a2 = (2 * pi) * (float)(lon == StimRes ? 0 : lon) / StimRes;
				float sin2 = Mathf.Sin(a2);
				float cos2 = Mathf.Cos(a2);

				//Defines the index (which value in the vertices array) we're changing
				int vertexindex = lon + lat * (StimRes + 1) + 1;

				//ADDED
				//Distorts some of the vertices
				if (vertexindex==2000)
				{
					vertices[ vertexindex ] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * modradius;
				}
					else
				{
				//Adds the vertices of that triangle to the vertices array
					vertices[ vertexindex ] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * radius;
					Debug.Log ("Defining vertex # " + vertexindex + " as " + vertices[vertexindex]);
					}
			}
		}
		//Defines the LAST value in the vertices array as the bottom of the sphere;
		vertices[vertices.Length-1] = Vector3.up * -radius;



		//Creates the normals of the mesh. Normals are like poles sticking out of every vertex
		//Initializes a new vector3 array to hold the normals
		Vector3[] normales = new Vector3[vertices.Length];
		//For every vertex
		for( int n = 0; n < vertices.Length; n++ )
			//??? Not sure whty this is normalized
			normales[n] = vertices[n].normalized;


		//Creates the UVs
		//UV coordinates are a normalized coordinate system, where the origin is in the bottom left corner of the space.
		//Used to accurately map textures onto 3d objects
		//Initializes a 2d (0,0) vector array as long as the 3d one (65282)
		Vector2[] uvs = new Vector2[vertices.Length];
		//Defines the first entry in the UV array as 0,1
		uvs[0] = Vector2.up;
		//Defines the last entry in the UV array as 0,0
		uvs[uvs.Length-1] = Vector2.zero;
		//For every slice of the circle
		for( int lat = 0; lat < StimRes; lat++ )
			//For every slice of the circle again - ASSUMES USING DIRRENT lat and long #s. Is there a reason for that?
			for( int lon = 0; lon <= StimRes; lon++ )
				//Defines the current value in the UV array (which'll be 0 to 65282]
				uvs[lon + lat * (StimRes + 1) + 1] = new Vector2( (float)lon / StimRes, 1f - (float)(lat+1) / (StimRes+1) );

//Creates the triangles
		//Defines the number of faces (65282 by default)
		int nbFaces = vertices.Length;
		//Defines the number of triangles there'll be (130564 by default)
		int nbTriangles = nbFaces * 2;
		//Defines the number of indexes there will be (391692 by default)
		int nbIndexes = nbTriangles * 3;

		//??? Not sure what this is doing
		int[] triangles = new int[ nbIndexes ];
		
		//Top Cap
		int i = 0;
		//??? Not sure what this is doing
		for( int lon = 0; lon < StimRes; lon++ )
		{
			triangles[i++] = lon+2;
			triangles[i++] = lon+1;
			triangles[i++] = 0;
		}
		
		//Middle
		//For every slice of the circle
		for( int lat = 0; lat < StimRes - 1; lat++ )
		{
			//For every slice of the circle again
			for( int lon = 0; lon < StimRes; lon++ )
			{
				int current = lon + lat * (StimRes + 1) + 1;
				int next = current + StimRes + 1;
				//??? Not sure what this is doing
				triangles[i++] = current;
				triangles[i++] = current + 1;
				triangles[i++] = next + 1;
				
				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = next;
			}
		}
		
		//Bottom Cap
		for( int lon = 0; lon < StimRes; lon++ )
		{
			//??? Again, not really sure what this is doing
			triangles[i++] = vertices.Length - 1;
			triangles[i++] = vertices.Length - (lon+2) - 1;
			triangles[i++] = vertices.Length - (lon+1) - 1;
		}
		Debug.Log(vertices[0]);
		Debug.Log(vertices[1]);
		Debug.Log(vertices[2]);
		Debug.Log(vertices[3]);
		Debug.Log(vertices[4]);
		Debug.Log(vertices[5]);
		Debug.Log(vertices[6]);
		Debug.Log(vertices[7]);
		Debug.Log(vertices[8]);
		Debug.Log(vertices[9]);
		Debug.Log(vertices[10]);
		Debug.Log(vertices[11]);
		Debug.Log(vertices[12]);
		Debug.Log(vertices[13]);
		Debug.Log(vertices[14]);
		Debug.Log(vertices[15]);
		Debug.Log(vertices[16]);
		Debug.Log(vertices[17]);
		Debug.Log(vertices[18]);
		Debug.Log(vertices[19]);
		Debug.Log(vertices[20]);
		Debug.Log(vertices[21]);
		Debug.Log(vertices[22]);
		//Assigns all the things we created to the mesh we created earlier
		mesh.vertices = vertices;
		mesh.normals = normales;
		//mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

}
