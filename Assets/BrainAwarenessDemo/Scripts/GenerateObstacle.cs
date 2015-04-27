using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;



public class GenerateObstacle : MonoBehaviour
{
    public GameObject cloud;
    public bool gen = true;
    public float seconds = 0;
    public float tstart;
    private float threshold = 3f;
    private IEnumerable<int> match = new int[] { 0, 0, 0, 0, 0 };
    private float max = 1000f;
    private int randNum;
    private int cloudcount;
    public Vector2 playerPosition = new Vector2(0, 0);
    public GameObject Heli;
    private Vector3 RandCloudPoint;
    private string gameMode = "New Game";
    private string[] gameModes = { "New Game", "Game Over" };
    private int gameModeInt = 2;
    private int counter = 0;
    private string pName;
    private string highPName;
    private float score;
    private string highScoreText = "CURRENT HIGH SCORE:  ";
    private float highScoreNum;
    private bool flash = false;
    private string comPortFieldString = "COM1";
    public List<GameObject> clouds = new List<GameObject>();
    private System.Random rand = new System.Random();
    


    // Use this for initialization


    void Awake()
    {
        
    } 
    void Start()
    {
       
        pName = "Enter Name";
        score = 0;
        PlayerPrefs.SetFloat("High Score", 0);
        highScoreNum = PlayerPrefs.GetFloat("High Score");

        match.ToArray()[0] = rand.Next(0, (int)max);
    
      

    }
    void Update()
    {
        print(cloudcount);
        if (cloudcount > 70    )
        {
            cloudcount = 0;
            clouds.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));
            foreach (GameObject cld in clouds) Destroy(cld);
        }

      
        if (Input.GetKeyUp("space") && gameModeInt != 0)
        {
            PlayerPrefs.SetString("Player Name", pName);
            gameModeInt = 0;
            tstart = Time.time;
            flash = false;
            Heli.transform.position = new Vector3(9.65f, 0,0);
        }
       
        
        if (gameModeInt == 0)
        {


            if (max != 100f)
            {
                if (max > 100f)
                {
                    max = max - .1f;

                }
                else max = 100f;
            }

            Heli.transform.position = new Vector3(Heli.transform.position.x, GameObject.Find("RigidHandLeft(Clone)").transform.Find("palm").transform.position.y, Heli.transform.position.z);
            Heli.transform.rigidbody2D.velocity = Vector3.zero;
            for (int i = 0; i < match.ToList().Count; i++)
            {
                match.ToArray()[i] = rand.Next(0, (int)max);

            }

            randNum = Random.Range(0, (int)max);



            

            if (gen)
            {
                gen = false;
                GenerateNewObstacle();

            }

            if (match.Contains(randNum))
            {
                gen = true;

            }

            if (Heli.transform.position.x < -12.29f) gameModeInt = 1;

        }
        else if(gameModeInt ==1)
        {
            pName = "Enter Name";
            
            if ( Time.time - tstart > PlayerPrefs.GetFloat("High Score") )
            {
                
                PlayerPrefs.SetFloat("High Score", Time.time - tstart);
                highScoreNum = PlayerPrefs.GetFloat("High Score");
                PlayerPrefs.SetString("High Score Name", pName);
            }
            flash = true;
            gameModeInt = 2;
        }

    }



    void GenerateNewObstacle()
    {

        RandCloudPoint = new Vector3(13.00f, Random.Range(4, -5), 0);
        Instantiate(cloud, RandCloudPoint, Quaternion.identity);
        cloudcount++;
    }




      void OnGUI()
    {
        GUIStyle messageStyle = new GUIStyle();
        messageStyle.normal.textColor = Color.black;
        messageStyle.fontSize = 16;
        GUIStyle hsText = new GUIStyle(GUI.skin.button);
        hsText.normal.textColor = Color.red;
        hsText.fontStyle = FontStyle.Bold;
        messageStyle.fontStyle = FontStyle.Bold;
        hsText.fontSize = 16;


        if (gameModeInt == 2)
        {
            gameMode = "Press Start";
        }



      

        
        GUI.Label(new Rect(50, 0, 350, 20), gameMode, messageStyle);

        if (gameModeInt == 0)
        {
            gameMode = "New Game";
            GUI.Label(new Rect(450, 0, 350, 20), (Time.time - tstart).ToString(), messageStyle);
        }

        GUI.Label(new Rect(775, 0, 350, 20), highScoreText + highScoreNum.ToString(), hsText);


        }


      void FlashLabel()
      {
          counter = counter + 1;

          if( counter % 100 ==0){
          if (string.Compare(gameMode, "") == 0  )
          {
              string temp = gameMode;
              gameMode = gameModes[1];
          }
          else gameMode = "";
      }
      }




}