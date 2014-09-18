using UnityEngine;
using System.Collections;

public class NBackGenerate : MonoBehaviour {

    //Declare variables to be used in the script
    public GameObject cube;
    private Vector3 cubePosition;
    private float stimNumber;

	// Use this for initialization
	void Start () {


        //For every cube in the grid
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    stimNumber = x + y + z;
                    Debug.Log(stimNumber);
                    //Defines the position of the cube in the series
                    cubePosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
                    Instantiate(cube, cubePosition, Quaternion.identity);
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
