using UnityEngine;
using System.Collections;

public class MoveObstacle : MonoBehaviour {

	public Vector2 velocity = new Vector2(-3, 0);
	// Use this for initialization
	void Start () {
		rigidbody2D.velocity=velocity;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
