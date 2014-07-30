using UnityEngine;
using System.Collections;

public class NBackGenerate : MonoBehaviour {

    public GameObject cube;

	// Use this for initialization
	void Start () {
        Instantiate(cube, new Vector3(1, 1, 5), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
