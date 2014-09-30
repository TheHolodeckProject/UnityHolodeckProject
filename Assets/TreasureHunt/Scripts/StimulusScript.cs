//using UnityEngine;
//using System.Collections;

//public class StimulusScript : MonoBehaviour {

//    SearchSpace searchScript;
//    GameObject SearchSpaceObj;
    
//   [SerializeField] private int foundPos;
//   [SerializeField]private bool active = false;
//  [SerializeField] private bool isFound = false;
//    // Use this for initialization
//    void Start () {
	
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (!(other.gameObject.layer == 8) && this.active)
//        {
//            //print("Collision Detected");    /*For debugging purposes*/
//            gameObject.renderer.enabled = true;
//            gameObject.renderer.material.color = Color.yellow;
//            isFound = true;
//            SearchSpaceObj = GameObject.Find("SearchSpace");
//            searchScript = SearchSpaceObj.GetComponent("SearchSpace") as SearchSpace;
//            foundPos = searchScript.getNumFound();
//            searchScript.incNumFound();
//        }
//    }

//    public float getDistance(Vector3 handPos)
//    {

//        // We will probably need to do intensity levels in here instead of after the value has been returned; TODO: discuss w/ Kevin & Andre
//        // May also need to change this depending on how we use difficulty setting
//        return Vector3.Distance(handPos, gameObject.transform.position);
//    }

//    public void toggleActive()
//    {
//        active = !active;

//    }

//    public bool getFound()
//    {
//        return isFound;
//    }


//    public void hide()
//    {
//        gameObject.renderer.material.color = Color.blue;
//        gameObject.renderer.enabled = false;
        
//    }
    
//}
