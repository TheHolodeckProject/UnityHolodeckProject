using UnityEngine;
using System.Collections;

public class Transparency : MonoBehaviour {

    public Color color = Color.red;
    

	// Use this for initialization
	void Start () {
        this.gameObject.renderer.material.color = new Color(this.gameObject.renderer.material.color.r, this.gameObject.renderer.material.color.g, this.gameObject.renderer.material.color.b, 0.5f);

        //
   //         renderer.material.color.a = 0.5;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
