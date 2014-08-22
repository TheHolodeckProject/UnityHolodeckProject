using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolodeckStimulusManager : MonoBehaviour
{

    public int numberOfStimuli = 3; //Static value representing number of stimuli for task
    public float Transparency = .9f;
    private Texture blobTex;
    private GameObject blob;
    private int stimNum;
    //public GameObject[] stimuli; //List of game objects (populated on Start())
    //private GameObject[] activeStimuli; //List of active stimuli indicies
    ////Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
    public Vector3 resetPositionP0; //Position for obj0
    public Vector3 resetPositionP1; //Position for objN
    //Random Bounds determines the 3D box which bounds the possible random positions of each object
    //The point P0 determines one corner of the box while the point P1 determines the opposite corner
    private Vector3 randomBoundsP0 = new Vector3(-6f, -2f, -4f); //One corner of the bounding box being used
    private Vector3 randomBoundsP1 = new Vector3(6f, 4f, 2f); //The opposite corner of the bounding box being used
    private Vector3 stimLocation;
    //    public Vector3 overlapPaddingFactor = new Vector3(0.5f, 0.5f, 0.5f);

    /// /////////////////////////////////////////
    /// INITIALIZATION CODE /////////////////////
    /// /////////////////////////////////////////

    void Start()
    {
        ////Creates the material to be used for the blobs
//        Material blobMat = new Material(Shader.Find("Transparent/Diffuse"));
        Material blobMat = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
 
        //Grabs a texture for the blob
        blobTex = Resources.Load("Red", typeof(Texture2D)) as Texture;
        blobMat.mainTexture = blobTex;

        ////Makes the color slightly transparent
      blobMat.color= new Color(blobMat.color.r, blobMat.color.g, blobMat.color.b, Transparency);

        for (int i = 0; i < numberOfStimuli; i++)
        {
            //Defines a random location within the stimulus bounds
            stimLocation = new Vector3(
    Random.Range(randomBoundsP0.x, randomBoundsP1.x),
    Random.Range(randomBoundsP0.y,randomBoundsP1.y),
    Random.Range(randomBoundsP0.z,randomBoundsP1.z)
    );
            //Debug.Log("stimLocation: " + stimLocation)
        //Defines a random integer between 1 and 100
            stimNum = Random.Range(1, 100);
            //Creates a game object using that simulus #
            GameObject blob = Resources.Load("Blob" + stimNum) as GameObject;
            //Assigns the material we created earlier to the object
            blob.gameObject.renderer.material = blobMat;    
            //Gives the blob the tag to make it grabbable
            blob.gameObject.tag = "Grabbable";
            //Instantiates that GameObject at the location defined earlier
            GameObject = Instantiate(blob, stimLocation, Quaternion.identity);
            //Makes the object a child of the StimulusManager parent
            blob.transform.parent = this.gameObject.transform;


            
        }
    }
//        //Create a knuth shuffle index list of random indicies within the range of possible stimuli
//        int[] knuthShuffleList = new int[stimuli.Length];
//        for (int i = 0; i < knuthShuffleList.Length; i++)
//            knuthShuffleList[i] = i;
//        for (int i = 0; i < stimuli.Length; i++)
//        {
//            int index = Random.Range(i, stimuli.Length - 1);
//            int tmp = knuthShuffleList[index];
//            knuthShuffleList[index] = knuthShuffleList[i];
//            knuthShuffleList[i] = tmp;
//        }

//        for (int i = 0; i < numberOfStimuli; i++)
//        {
//            activeStimuli[i] = (GameObject)Instantiate(stimuli[knuthShuffleList[i]]);
//            activeStimuli[i].transform.localScale = stimuliScale;
//            activeStimuli[i].transform.parent = this.gameObject.transform;
//            activeStimuli[i].transform.GetChild(0).gameObject.AddComponent<SphereCollider>();
//            activeStimuli[i].AddComponent<SimpleObjectLogger>();
//            activeStimuli[i].AddComponent<StimuliBehavior>().touched = false;
//        }
//    }

//    private int numRetries = 100;
//    void generateRandomPositions()
//    {
//        //Position stimuli randomly according to settings (no overlaps)
//        List<Rect> overlapCheckList = new List<Rect>();
//        int retries = numRetries;
//        for (int i = 0; i < activeStimuli.Length; i++)
//        {
//            activeStimuli[i].transform.localPosition = 
//            //Check for overlapping boxes and regenerate box location if overlap occurs
//            Rect newBox = new Rect(activeStimuli[i].transform.localPosition.x,
//                                   activeStimuli[i].transform.localPosition.z,
//                                   activeStimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x * activeStimuli[i].transform.localScale.x,
//                                   activeStimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z * activeStimuli[i].transform.localScale.z);
//            if (boxesOverlapArray(newBox, overlapCheckList))
//            {
//                retries--;
//                if (retries <= 0)
//                {
//                    //Force quit and overlap to prevent hang
//                    retries = numRetries;
//                    overlapCheckList.Add(newBox);
//                    Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
//                }
//                else i--;
//            }
//            else
//            {
//                retries = numRetries;
//                overlapCheckList.Add(newBox);
//            }
//        }
//    }

    bool boxesOverlapArray(Rect box, List<Rect> boxArray)
    {
        for (var i = 0; i < boxArray.Count; i++)
            if (boxesOverlap(box, boxArray[i])) return true;
        return false;
    }

    //Reference: http://gamemath.com/2011/09/detecting-whether-two-boxes-overlap/
    bool boxesOverlap(Rect box0, Rect box1)
    {
        if (box0.x + box0.width < box1.x) return false;
        if (box0.x > box1.x + box1.width) return false;
        if (box0.y + box0.height < box1.y) return false;
        if (box0.y > box1.y + box1.height) return false;
        return true;
    }
}

    //    private void decideRandomStimuli()