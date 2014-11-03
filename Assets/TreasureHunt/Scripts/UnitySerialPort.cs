// <copyright file="UnitySerialPort.cs" company="dyadica.co.uk">
// Copyright (c) 2010, 2014 All Right Reserved, http://www.dyadica.co.uk

// This source is subject to the dyadica.co.uk Permissive License.
// Please see the http://www.dyadica.co.uk/permissive-license file for more information.
// All other rights reserved.

// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>

// <author>SJB</author>
// <email>SJB@dyadica.co.uk</email>
// <date>04.09.2013</date>
// <summary>A MonoBehaviour type class containing several functions which can be utilised 
// to perform serial communication within Unity3D</summary>

using UnityEngine;
using System.Collections;

using System.IO;
using System.IO.Ports;
using System;

using System.Threading;
using System.Linq;

public class UnitySerialPort : MonoBehaviour 
{
    // Init a static reference if script is to be accessed by others when used in a 
    // none static nature eg. its dropped onto a gameObject. The use of "Instance"
    // allows access to public vars as such as those available to the unity editor.
    public UnitySerialPort Instance;

    #region Properties

    // The serial port
    public SerialPort SerialPort;

    // The script update can run as either a seperate thread
    // or as a standard coroutine. This can be selected via 
    // the unity editor.

    public enum LoopUpdateMethod
    { Threading, Coroutine }

    // This is the public property made visible in the editor.
    public LoopUpdateMethod UpdateMethod = 
        LoopUpdateMethod.Threading;

    // Thread used to recieve and send serial data
    private Thread serialThread;

    // List of all baudrates available to the arduino platform
    private ArrayList baudRates =
        new ArrayList() { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };

    // List of all com ports available on the system
    private ArrayList comPorts =
        new ArrayList();

    // If set to true then open the port when the start
    // event is called.
    public bool OpenPortOnStart = false;

    // Holder for status report information
    private string portStatus = "";
    public string PortStatus
    {
        get { return portStatus; }
        set { portStatus = value; }
    }

    // Current com port and set of default
    public string ComPort = "COM4";

    // Current baud rate and set of default
	public int BaudRate = 38400;
	public int ReadTimeout = 1000;
    public int WriteTimeout = 1000;

	public string OutputString;
	private string PortOpenStatus;

	public const int startMessage = 10;
	/*private int m_thumbTip = 0;
	public int thumbTip
	{
		get { return m_thumbTip; }
		set { m_thumbTip = value;	}
	}*/

    public byte thumbTip = 2;
    public int indexTip = 3;
    public int middleTip = 4;
    public int ringTip = 5;
    public int pinkyTip = 6;
    /*
	public int thumbTip = 0;
	public int indexTip = 0;
	public int middleTip = 0;
	public int ringTip = 0;
	public int pinkyTip = 0;*/
	public const int endMessage = 7;
	public volatile byte[] handState = Enumerable.Repeat((byte)0x00, 7).ToArray();
	public byte[] oldHandState = Enumerable.Repeat((byte)0x01, 7).ToArray();
	public bool updateHandState;

	public const int START_MESSAGE = 0;
	public const int THUMB_TIP_BYTE = 1;
	public const int INDEX_TIP_BYTE = 2;
	public const int MIDDLE_TIP_BYTE = 3;
	public const int RING_TIP_BYTE = 4;
	public const int PINKY_TIP_BYTE = 5;
	public const int END_MESSAGE = 6;
	public const int FAILURE = -1;
	public int serialPortState = 0;

	/*
	public byte[] handState = {0x00,0x00};
	public byte[] thumbFinger = {0x10};
	public byte[] indexFinger = {0x08};
	public byte[] middleFinger = {0x04};
	public byte[] ringFinger = {0x02};
	public byte[] pinkyFinger = {0x01};
	public byte[] high = {0xff};
	public int intensity = 255;
	*/

    // Property used to run/keep alive the serial thread loop
    private bool isRunning = false;
    public bool IsRunning
    {
        get { return isRunning; }
        set { isRunning = value; }
    }

    // Set the gui to show ready
    private string rawData = "Ready";
    public string RawData
    {
        get { return rawData; }
        set { rawData = value; }
    }
    
    // Storage for parsed incoming data
    private string[] chunkData;
    public string[] ChunkData
    {
        get { return chunkData; }
        set { chunkData = value; }
    }

    // Refs populated by the editor inspector for default gui
    // functionality if script is to be used in a non-static
    // context.
    public GameObject ComStatusText;

    public GameObject RawDataText;

    #endregion Properties

    #region Unity Frame Events

    /// <summary>
    /// The awake call is used to populate refs to the gui elements used in this 
    /// example. These can be removed or replaced if needed with bespoke elements.
    /// This will not affect the functionality of the system. If we are using awake
    /// then the script is being run non staticaly ie. its initiated and run by 
    /// being dropped onto a gameObject, thus enabling the game loop events to be 
    /// called e.g. start, update etc.
    /// </summary>
    void Awake()
    {
        // Define the script Instance
        Instance = this;

        // If we have used the editor inspector to populate any included gui
        // elements then lets initiate them and set some default values.

        // Details if the port is open or closed
        if (ComStatusText != null)
        { ComStatusText.guiText.text = "ComStatus: Closed"; }
    }

    void GameObjectSerialPort_DataRecievedEvent(string[] Data, string RawData)
    {
        print("Data Recieved: " + RawData);
    }

    /// <summary>
    /// The start call is used to populate a list of available com ports on the
    /// system. The correct port can then be selected via the respective guitext
    /// or a call to UpdateComPort();
    /// </summary>
    void Start()
    {
		//handState[0] = startMessage;
		//handState[6] = endMessage;

        handState[0] = (byte) 42;
        handState[6] = (byte) 45;

        // Population of comport list via system.io.ports
        PopulateComPorts();

        // If set to true then open the port. You must 
        // ensure that the port is valid etc. for this! 
        if (OpenPortOnStart) { OpenSerialPort(); }
    }

    /// <summary>
    /// The update frame call is used to provide caps for sending data to the arduino
    /// triggered via keypress. This can be replaced via use of the static functions
    /// SendSerialData() & SendSerialDataAsLine(). Additionaly this update uses the
    /// RawData property to update the gui. Again this can be removed etc.
    /// </summary>
    void Update()
    {
        // Check if the serial port exists and is open
        if (SerialPort == null || SerialPort.IsOpen == false) 
		{ 
			PortOpenStatus = "Open Port";
			return; 
		}

		switch (SerialPort.IsOpen)
		{
			case true: PortOpenStatus = "Close Port"; break;
			case false: PortOpenStatus = "Open Port"; break;
		}
		
		// Example calls from system to the arduino. For more detail on the
        // structure of the calls see: http://www.dyadica.co.uk/journal/simple-serial-string-parsing/
        try
        {
			UpdateHand();
        }
        catch (Exception ex)
        {
            // Failed to send serial data
            Debug.Log("Error 6: " + ex.Message.ToString());
        }

        try
        {
            // If we have set a GUI Text object then update it. This can only be
            // run on the thread that initialised the object thus cnnot be run
            // in the ParseSerialData() call below... Unless run as a coroutine!

            // I have also included a raw data example which is called from a
            // seperate script... see RawDataExample.cs

            if (RawDataText != null)
                RawDataText.guiText.text = RawData;
        }
        catch (Exception ex)
        {
            // Failed to update serial data
            Debug.Log("Error 7: " + ex.Message.ToString());
        }
    }

    /*
	void OnGUI ()
	{
		// If we have defined a GUI Skin via the unity 
		// editor then apply it.
		
		//if (GUISkin != null) { GUI.skin = GUISkin; }
		
		// Draw an area to hold the GUI content.
		
		GUILayout.BeginArea(new Rect(10, 10, 200, 200), "", GUI.skin.box);
		
		// Draw a button that can be used to open or
		// close the serial port.
		
		if (GUILayout.Button(PortOpenStatus, GUILayout.Height(30)))
		{
			if (SerialPort == null)
			{ OpenSerialPort(); return; }
			
			switch (SerialPort.IsOpen)
			{
			case true: CloseSerialPort(); break;
			case false: OpenSerialPort(); break;
			}
		}
		
		// Draw a title for the input textfield
		
		GUILayout.Label("Input string");
		
		// Draw a textfield that can be used to show 
		// data sent via the serial port to unity.
		
		GUILayout.TextField(RawData,GUILayout.Height(20));
		
		// Provide some padding to seperate
		
		GUILayout.Space(20);
		
		// Draw a title for the output textfield
		
		GUILayout.Label("Output string");
		
		// Draw a textfield that can be used to define 
		// data to be sent via the serial port
		
		OutputString = GUILayout.TextField(OutputString, GUILayout.Height(20));
		
		// Draw a button that can be used to send serial
		// data from the unity environment.
		
		/*if(unitySerialPort.SerialPort.IsOpen) {
			if (Input.GetKeyDown (KeyCode.N))
			{
				print ("RightThumbDown");
				unitySerialPort.vibrate("rightHannnd", "rightThumb",255);
			
			}
			if (Input.GetKeyUp (KeyCode.N))
			{
				print ("RightThumbUp");
				unitySerialPort.vibrate("rightHand", "rightThumb",0);
			}
		}
		if (GUILayout.Button("Send Data", GUILayout.Height(30)))
		{
			if (SerialPort.IsOpen)
				//unitySerialPort.vibrate("rightHand", "rightThumb",0);
				SendSerialData(OutputString);
			
		}
		
		// Thats it we are finished so lets close the area
		
		GUILayout.EndArea();
	}*/
	
	/// <summary>
	/// Clean up the thread and close the port on application close event.
	/// </summary>
	void OnApplicationQuit()
	{
		// Call to cloase the serial port
		CloseSerialPort();
		
		Thread.Sleep(500);

        if (UpdateMethod == LoopUpdateMethod.Threading)
        {
            // Call to end and cleanup thread
            StopSerialThread();
        }

        if (UpdateMethod == LoopUpdateMethod.Coroutine)
        {
            // Call to end and cleanup coroutine
            StopSerialCoroutine();
        }

        Thread.Sleep(500);
    }

    #endregion Unity Frame Events

	#region Leap Glove Functions

	public void UpdateHand()
	{
		updateHandState = false;

		if (thumbTip != oldHandState[1])
		{
			handState[1] = (byte)thumbTip;
			updateHandState = true;
		}
		if (indexTip != oldHandState[2])
		{
			handState[2] = (byte)indexTip;
			updateHandState = true;
		}
		if (middleTip != oldHandState[3])
		{
			handState[3] = (byte)middleTip;
				updateHandState = true;
		}
		if (ringTip != oldHandState[4])
		{
			handState[4] = (byte)ringTip;
			updateHandState = true;
		}
		if (pinkyTip != oldHandState[5])
		{
			handState[5] = (byte)pinkyTip;
			updateHandState = true;
		}


		if(updateHandState)
		{
			SerialPort.Write (handState, 0, handState.Length);
		}
			
		oldHandState[1] = handState[1];
		oldHandState[2] = handState[2];
		oldHandState[3] = handState[3];
		oldHandState[4] = handState[4];
		oldHandState[5] = handState[5];
	}

	public bool ConnectionIsOpen()
	{
		return SerialPort.IsOpen;
	}

	#endregion Leap Glove Functions

    #region Object Serial Port

    /// <summary>
    /// Opens the defined serial port and starts the serial thread used
    /// to catch and deal with serial events.
    /// </summary>
    public void OpenSerialPort()
    {
        try
        {
            // Initialise the serial port
            SerialPort = new SerialPort(ComPort, BaudRate);

            SerialPort.ReadTimeout = ReadTimeout;

            SerialPort.WriteTimeout = WriteTimeout;

            // Open the serial port
            SerialPort.Open();

            // Update the gui if applicable
            if (Instance != null && Instance.ComStatusText != null)
            { Instance.ComStatusText.guiText.text = "ComStatus: Open"; }

            if (UpdateMethod == LoopUpdateMethod.Threading)
            {
                // If the thread does not exist then start it
                if (serialThread == null)
                {
                    StartSerialThread();
                }
            }

            if (UpdateMethod == LoopUpdateMethod.Coroutine)
            {
                if (isRunning == false)
                {
                    StartSerialCoroutine();
                }
                else
                {
                    isRunning = false;

                    // Give it chance to timeout
                    Thread.Sleep(100);

                    try
                    {
                        // Kill it just in case
                        StopCoroutine("SerialCoroutineLoop");
                    }
                    catch(Exception ex)
                    {
                        print("Error N: " + ex.Message.ToString());
                    }

                    // Restart it once more
                    StartSerialCoroutine();
                }
            }

            print("SerialPort successfully opened!");

        }
        catch (Exception ex)
        {
            // Failed to open com port or start serial thread
            Debug.Log("Error 1: " + ex.Message.ToString());
        }
    }

    /// <summary>
    /// Cloases the serial port so that changes can be made or communication
    /// ended.
    /// </summary>
    public void CloseSerialPort()
    {
        try
        {
            // Close the serial port
            SerialPort.Close();

            // Update the gui if applicable
            if (Instance.ComStatusText != null)
            { Instance.ComStatusText.guiText.text = "ComStatus: Closed"; }
        }
        catch (Exception ex)
        {
            if (SerialPort == null || SerialPort.IsOpen == false)
            {
                // Failed to close the serial port. Uncomment if
                // you wish but this is triggered as the port is
                // already closed and or null.

                // Debug.Log("Error 2A: " + "Port already closed!");
            }
            else
            {
                // Failed to close the serial port
                Debug.Log("Error 2B: " + ex.Message.ToString());
            }
        }

        print("Serial port closed!");
    }

    #endregion Object Serial Port

    #region Serial Coroutine

    /// <summary>
    /// Function used to start coroutine for reading serial 
    /// data.
    /// </summary>
    public void StartSerialCoroutine()
    {
        isRunning = true;

        StartCoroutine("SerialCoroutineLoop");
    }

    /// <summary>
    /// A Coroutine used to recieve serial data thus not 
    /// affecting generic unity playback etc.
    /// </summary>
    public IEnumerator SerialCoroutineLoop()
    {
        while (isRunning)
        {
            GenericSerialLoop();
            yield return null;
        }

        print("Ending Coroutine!");
    }

    /// <summary>
    /// Function used to stop the coroutine and kill
    /// off any instance
    /// </summary>
    public void StopSerialCoroutine()
    {
        isRunning = false;

        Thread.Sleep(100);

        try
        {
            StopCoroutine("SerialCoroutineLoop");
        }
        catch (Exception ex)
        {
            print("Error 2A: " + ex.Message.ToString());
        }

        // Reset the serial port to null
        if (SerialPort != null)
        { SerialPort = null; }

        // Update the port status... just in case :)
        portStatus = "Ended Serial Loop Coroutine!";

        print("Ended Serial Loop Coroutine!");
    }

    #endregion Serial Coroutine

    #region Serial Thread

    /// <summary>
    /// Function used to start seperate thread for reading serial 
    /// data.
    /// </summary>
    public void StartSerialThread()
    {
        try
        {
            // define the thread and assign function for thread loop
            serialThread = new Thread(new ThreadStart(SerialThreadLoop));
            // Boolean used to determine the thread is running
            isRunning = true;
            // Start the thread
            serialThread.Start();

            print("Serial thread started!");
        }
        catch (Exception ex)
        {
            // Failed to start thread
            Debug.Log("Error 3: " + ex.Message.ToString());
        }
    }

    /// <summary>
    /// The serial thread loop. A Seperate thread used to recieve
    /// serial data thus not affecting generic unity playback etc.
    /// </summary>
    private void SerialThreadLoop()
    {
        while (isRunning)
        {
            GenericSerialLoop();
        }

        print("Ending Thread!");
    }

    /// <summary>
    /// Function used to stop the serial thread and kill
    /// off any instance
    /// </summary>
    public void StopSerialThread()
    {
        // Set isRunning to false to let the while loop
        // complete and drop out on next pass
        isRunning = false;

        // Pause a little to let this happen
        Thread.Sleep(100);

        // If the thread still exists kill it
        // A bit of a hack using Abort :p
        if (serialThread != null)
        {
            serialThread.Abort();
            // serialThread.Join();
            Thread.Sleep(100);
            serialThread = null;
        }

        // Reset the serial port to null
        if (SerialPort != null)
        { SerialPort = null; }

        // Update the port status... just in case :)
        portStatus = "Ended Serial Loop Thread";

        print("Ended Serial Loop Thread!");
    }

    #endregion Serial Thread 

    #region Static Functions

    /// <summary>
    /// Function used to send string data over serial with
    /// an included line return
    /// </summary>
    /// <param name="data">string</param>
    public void SendSerialDataAsLine(string data)
    {
        if (SerialPort != null)
        { SerialPort.WriteLine(data); }

        print("Sent data: " + data);
    }

    /// <summary>
    /// Function used to send string data over serial without
    /// a line return included.
    /// </summary>
    /// <param name="data"></param>
    public void SendSerialData(string data)
    {
        if (SerialPort != null)
        { SerialPort.Write(data); }

        print("Sent data: " + data);
    }

    #endregion Static Functions

    /// <summary>
    /// The serial thread loop & the coroutine loop both utilise
    /// the same code with the exception of the null return on 
    /// the coroutine, so we share it here.
    /// </summary>
    private void GenericSerialLoop()
    {
        try
        {
            // Check that the port is open. If not skip and do nothing
            if (SerialPort.IsOpen)
            {
                // Read serial data until a '\n' character is recieved
             
				int rDataByte = SerialPort.ReadByte();
                /*string rData = SerialPort.ReadLine();

                // If the data is valid then do something with it
                if (rData != null && rData != "")
                {
                    // Store the raw data
                    RawData = rData;
                    // split the raw data into chunks via ',' and store it
                    // into a string array
                    ChunkData = RawData.Split(',');

                    // Or you could call a function to do something with
                    // data e.g.
                    ParseSerialData(ChunkData, RawData);
                }*/

				if (rDataByte != null)
				{
					// Store the raw data
					print ("Received Byte: " + rDataByte + "\tSerialState: " + serialPortState);
                    
					switch(serialPortState)
					{
						case START_MESSAGE:
							if(rDataByte == 10)
							{
								print ("rDataByte == 10");
								serialPortState = THUMB_TIP_BYTE;
							}
								//print("Garbage data was received. Waiting for Start Message...");
							break;
						case THUMB_TIP_BYTE:
							if(rDataByte == 1)
							{
								print ("rDataByte == 1");
								serialPortState = INDEX_TIP_BYTE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at THUMB_TIP_BYTE");
							}
							else
							{
								serialPortState = FAILURE;
								print("Failed packet at THUMB_TIP_BYTE");
							}
							break;
						case INDEX_TIP_BYTE:
							if(rDataByte == 2)
							{
								print ("rDataByte == 2");
								serialPortState = MIDDLE_TIP_BYTE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at INDEX_TIP_BYTE");
							}
							else
							{
								serialPortState = FAILURE;
								print("Failed packet at INDEX_TIP_BYTE");
							}
							break;
						case MIDDLE_TIP_BYTE:
							if(rDataByte == 3)
							{
								print ("rDataByte == 3");
								serialPortState = RING_TIP_BYTE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at MIDDLE_TIP_BYTE");
							}
							else
							{
								serialPortState = FAILURE;
								print("Failed packet at MIDDLE_TIP_BYTE");
							}
							break;
						case RING_TIP_BYTE:
							if(rDataByte == 4)
							{
								print ("rDataByte == 4");
								serialPortState = PINKY_TIP_BYTE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at RING_TIP_BYTE");
							}
							else
							{
								serialPortState = FAILURE;
								print("Failed packet at RING_TIP_BYTE");
							}
							break;
						case PINKY_TIP_BYTE:
							if(rDataByte == 5)
							{
								print ("rDataByte == 5");
								serialPortState = END_MESSAGE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at PINKY_TIP_BYTE");
							}
							else
							{
								serialPortState = FAILURE;
								print("Failed packet at PINKY_TIP_BYTE");
							}
							break;
						case END_MESSAGE:
							if(rDataByte == 7)
							{
								print ("rDataByte == 7");
								serialPortState = START_MESSAGE;
							}
							else if(rDataByte == 0xff)
							{
								serialPortState = FAILURE;
								print("Glove Failure at END_MESSAGE");
							}
							else 
							{
								serialPortState = FAILURE;
								print("Failed packet at END_MESSAGE");
							}
							break;
						case FAILURE:
							//print ("FAILURE OCCURRED");
						default:
							serialPortState = START_MESSAGE;
							print("Restart State Machine and wait for Start Message");
							break;
					}
				}
			}
		}
		catch (TimeoutException timeout)
		{
			// This will be triggered lots with the coroutine method
		}
		catch (Exception ex)
        {
            // This could be thrown if we close the port whilst the thread 
            // is reading data. So check if this is the case!
            if (SerialPort.IsOpen)
            {
                // Something has gone wrong!
                Debug.Log("Error 4: " + ex.Message.ToString());
            }
            else
            {
                // Error caused by closing the port whilst in use! This is 
                // not really an error but uncomment if you wish.

                // Debug.Log("Error 5: Port Closed Exception!");
            }
        }
    }

    /// <summary>
    /// Function used to filter and act upon the data recieved. You can add
    /// bespoke functionality here.
    /// </summary>
    /// <param name="data">string[] of raw data seperated into chunks via ','</param>
    /// <param name="rawData">string of raw data</param>
    /*private void ParseSerialData(string[] data, string rawData)
    {
        // Examples of reading a value from the recieved data
        // for use if required - remove or replase with bespoke
        // functionality etc

        if (data.Length == 2)
        { int ReceviedValue = int.Parse(data[1]); }
        else { print(rawData); }

        if (data == null || data.Length != 2)
        { print(rawData); }

        // The following can be run if the code is run via the coroutine method.

        //if (RawDataText != null)
        //    RawDataText.guiText.text = RawData;
    }
	*/
    /// <summary>
    /// Function that utilises system.io.ports.getportnames() to populate
    /// a list of com ports available on the system.
    /// </summary>
    public void PopulateComPorts()
    {
        // Loop through all available ports and add them to the list
        foreach (string cPort in System.IO.Ports.SerialPort.GetPortNames())
        {
            comPorts.Add(cPort); // Debug.Log(cPort.ToString());
        }

        // Update the port status just in case :)
        portStatus = "ComPort list population complete";
    }

    /// <summary>
    /// Function used to update the current selected com port
    /// </summary>
    public string UpdateComPort()
    {
        // If open close the existing port
        if (SerialPort != null && SerialPort.IsOpen)
        { CloseSerialPort(); }

        // Find the current id of the existing port within the
        // list of available ports
        int currentComPort = comPorts.IndexOf(ComPort);

        // check against the list of ports and get the next one.
        // If we have reached the end of the list then reset to zero.
        if (currentComPort + 1 <= comPorts.Count - 1)
        {
            // Inc the port by 1 to get the next port
            ComPort = (string)comPorts[currentComPort + 1];
        }
        else
        {
            // We have reached the end of the list reset to the
            // first available port.
            ComPort = (string)comPorts[0];
        }

        // Update the port status just in case :)
        portStatus = "ComPort set to: " + ComPort.ToString();

        // Return the new ComPort just in case
        return ComPort;
    }

    /// <summary>
    /// Function used to update the current baudrate
    /// </summary>
    public int UpdateBaudRate()
    {
        // If open close the existing port
        if (SerialPort != null && SerialPort.IsOpen)
        { CloseSerialPort(); }

        // Find the current id of the existing rate within the
        // list of defined baudrates
        int currentBaudRate = baudRates.IndexOf(BaudRate);

        // check against the list of rates and get the next one.
        // If we have reached the end of the list then reset to zero.
        if (currentBaudRate + 1 <= baudRates.Count - 1)
        {
            // Inc the rate by 1 to get the next rate
            BaudRate = (int)baudRates[currentBaudRate + 1];
        }
        else
        {
            // We have reached the end of the list reset to the
            // first available rate.
            BaudRate = (int)baudRates[0];
        }

        // Update the port status just in case :)
        portStatus = "BaudRate set to: " + BaudRate.ToString();

        // Return the new BaudRate just in case
        return BaudRate;
    }

}
