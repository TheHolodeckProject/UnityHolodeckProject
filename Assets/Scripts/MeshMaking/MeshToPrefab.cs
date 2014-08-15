using UnityEngine;
using System.Collections;
using System.IO;

public class MeshToPrefab : MonoBehaviour {

    private GameObject emptyPrefabWithMeshRenderer; 
    //public string meshPath; 
    private GameObject spawnedPrefab;
    //private GameObject blob;


	// Use this for initialization
	void Start () {

        Mesh importedMesh;

        //Instantiates the GameObject
        GameObject blob = new GameObject("blob");

        //Defines the path to the mesh .obj
        string meshPath = "C:/Users/Aldi/Documents/GitHub/UnityHolodeckProject/Assets/Stimuli/Blooob.obj";

        importedMesh = GetComponent<ObjImporter>().ImportFile("foewjfoiewj");

        GameObject spawnedPrefab = Instantiate(emptyPrefabWithMeshRenderer, transform.position, transform.rotation) as GameObject; 
        //spawnedPrefab.GetComponent(MeshFilter).mesh = importedMesh;

        blob.AddComponent(typeof(MeshFilter));

        //Adds a mesh
	}
	
	
}
