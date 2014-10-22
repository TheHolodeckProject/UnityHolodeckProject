using UnityEngine;
using System.Collections;

public class MoviePlayer : MonoBehaviour {
    public Material defaultMaterial = new Material("tv_default");
    public Material videoMaterial = new Material("");
    MovieTexture movie;
    AudioSource audio;
	// Use this for initialization
	void Start () {
        renderer.material = videoMaterial;
        movie = ((MovieTexture)(renderer.material.mainTexture));
        audio = GetComponent<AudioSource>();
        movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (movie.isPlaying == false && renderer.material != defaultMaterial)
            renderer.material = defaultMaterial;

        if (Input.GetKey(KeyCode.Tab))
            replay();
	}

    void replay()
    {
        renderer.material = videoMaterial;
        movie = ((MovieTexture)(renderer.material.mainTexture));
        movie.Stop();
        audio.Stop();
        movie.Play();
        audio.Play();
    }
}
