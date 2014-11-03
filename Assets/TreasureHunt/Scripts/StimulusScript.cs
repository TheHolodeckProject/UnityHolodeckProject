using UnityEngine;
using System.Collections;

public class StimulusScript : MonoBehaviour {

    SearchSpace searchScript;
    GameObject SearchSpaceObj;
    
   [SerializeField] private int foundPos;
   [SerializeField]private bool active = false;
  [SerializeField] private bool isFound = false;
  private int low = 150;
  private int high = 255;
private float granularityInt = 2.5f;
  
  
	// Use this for initialization
	void Start () {
        SearchSpaceObj = GameObject.Find("SearchSpace");
        searchScript = SearchSpaceObj.GetComponent("SearchSpace") as SearchSpace;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (!(other.gameObject.layer == 8) && active)
        {
            setThisInactive();
            print("Collision Detected");    /*For debugging purposes*/
            gameObject.renderer.enabled = true;
            gameObject.renderer.material.color = Color.yellow;
            isFound = true;
            
            foundPos = searchScript.getNumFound();
            searchScript.incNumFound();
        }
    }

    public byte getIntensity(Vector3 handPos, float threshold = 0f)
    {

        float dist = Vector3.Distance(handPos, gameObject.transform.position);
        print("Distance is: " + dist);
        float increment = searchScript.getMaxDist() / (high - low);
        float level = high - dist / increment;
        if (level > low) return System.Convert.ToByte(level);
        else return System.Convert.ToByte(low);
    }

    public void setThisActive( )
    {
        active = true;

    } 

    public void setThisInactive()
    {
        active = false;

    }

    public bool getFound()
    {
        return isFound;
    }


    public void hide()
    {
        gameObject.renderer.material.color = Color.blue;
        gameObject.renderer.enabled = false;
        
    }
    
}
