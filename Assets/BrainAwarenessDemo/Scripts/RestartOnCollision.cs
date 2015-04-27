using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using Leap;

public class RestartOnCollision : MonoBehaviour {
    private byte[] WriteToLeftGlove;
    private string tempS1;
    private int exceptionCount = 0; 
    private byte tempB;
    private string boneTemp;
    private int LchannelsPerFinger = 1;
    private bool LpalmChannel = false;
    public static string COMPort = "COM3";
    private int LnumChannelFingers = 5;
    private Color[] colors;
    private Dictionary<string, int> ChannelDist = new Dictionary<string, int>{    //use dictionary to alter channels if needed
                    {"palm", 0}, {"palmR", 0},

                    {"thumb1", 0}, {"thumb2", 2}, {"thumb3", 0},

                    {"index1", 1},{"index2", 5},  {"index3", 1},
                  
                    {"middle1", 2},{"middle2", 8},{"middle3", 2},           //order of handpoints in dictionary should omatch order of points in handPOints list 

                    {"ring1", 3},  {"ring2", 11}, {"ring3", 3},

                    {"pinky1", 4}, {"pinky2", 14}, {"pinky3", 4},

};

    private List<string> handPoints = new List<string> { "palm", "thumb", "index", "middle", "ring", "pinky", };
    public static SerialPort _SerialPort = new SerialPort(COMPort, 9600);
    public static bool Errorhandling = false;
	// Use this for initialization
	void Start () {
       
        OpenConnection();
        WriteToLeftGlove = new byte[(LpalmChannel ? 1 : 0) + LchannelsPerFinger * LnumChannelFingers];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other)
	{
       
        try { 
        if (LnumChannelFingers > 0)
        {
           
            for (int j = 1; j < LnumChannelFingers+1 ; j++)
            {
                string tempS = handPoints[j];
                if (LchannelsPerFinger != 1)
                {
                  
                    for (int z = 1; z < LchannelsPerFinger; z++)
                    {

                        boneTemp = System.String.Concat("bone", z.ToString());
                         tempS1 = System.String.Concat(handPoints[j], j.ToString());
                       
                        tempB = System.Convert.ToByte((ChannelDist[tempS1]) * 16 + 5);

                        WriteToLeftGlove[j*z] =tempB;
                    }
                }
                else
                {
                    print("in try");
                     boneTemp = System.String.Concat("bone", 3.ToString());
                     tempS1 = System.String.Concat(handPoints[j], 3.ToString());

                     tempB = System.Convert.ToByte((ChannelDist[tempS1]) * 16 + 12);

                    if (LpalmChannel) WriteToLeftGlove[j] = tempB;
                    else WriteToLeftGlove[j - 1] = tempB;
                }

            }
        }
        }
        catch (System.NullReferenceException e) { exceptionCount++; }
        _SerialPort.Write(WriteToLeftGlove, 0, (WriteToLeftGlove.Length));
    }

    void OnCollisionExit2D(Collision2D other)
    {

      stopAll();
}

    void stopAll()
    {
        byte[] test = new byte[16];
        for (int i = 0; i < 16; i++)
        {

            test[i] = System.Convert.ToByte(i * 16 + 0);

        }
        _SerialPort.Write(test, 0, 16);
       
    }

    void OnApplicationQuit()
    {
        _SerialPort.Close();
    }



    public static bool OpenConnection()
    {
        print("in openconnection");
        print(_SerialPort);
        if (_SerialPort != null)
        {
            if (_SerialPort.IsOpen)
            {
                byte[] test = { 0 };
                _SerialPort.Write(test, 0, 1);
                //_SerialPort.Close();
                print("closing port, because it was already open!");
                return false;
            }
            else
            {
                try
                {
                    print("trying to open");
                    _SerialPort.Open();
                    if (!Errorhandling)
                    {
                        print("port open = " + _SerialPort.IsOpen);
                    }
                }
                catch (System.IO.IOException e)
                {
                    if (e.Source != null)
                        print("could not pair with bt device!");
                    return false;
                }
            }
        }
        else
        {
            if (_SerialPort.IsOpen)
            {
                print("port is already open");
            }
            else
            {
                print("port == null");
                return false;
            }
        }
        return true;
    }
}
