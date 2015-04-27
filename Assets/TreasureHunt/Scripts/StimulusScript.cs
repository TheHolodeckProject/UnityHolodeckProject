using UnityEngine;
using System.Collections;

public class StimulusScript : MonoBehaviour {

    SearchSpace searchScript;
    GameObject SearchSpaceObj;
    
   [SerializeField] private int foundPos;
   [SerializeField]private bool active = false;
  [SerializeField] private bool isFound = false;
  private int low = 0;
  private int high = 15;
  private int granularityInt = 16;
  private float foundTime;
  public bool hittingOtherSphere = false;
  
  
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


        if (other.gameObject.layer == 8)
        {
            hittingOtherSphere = true;
        }
        
        else if(active)
        {

            
            foundTime = Time.time;
            print("Collision Detected");    /*For debugging purposes*/
            gameObject.renderer.enabled = true;
            isFound = true;
            setThisInactive();
            foundPos = searchScript.getNumFound();
            searchScript.incNumFound();
        }
    }

    public int getIntensity(Vector3 handPos)
    {

        float dist = Vector3.Distance(handPos, gameObject.transform.position);
        float increment = searchScript.getMaxDist() / granularityInt;
        int bin = Mathf.FloorToInt( (dist-1) / increment);
        int level = high - bin;

        if (level < low)
        {
            level = low;
        }
        if (level > high) level=high;
        print("Level is: " + level);
        
        return level;
        
        
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

    public string getFoundTime()
    {
        return foundTime.ToString();
    }
    
}
