using UnityEngine;
using System.Collections;
using System;
using System.IO;
//using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Logger : MonoBehaviour {
    public string loggerDir = "";
    public string extension = ".HoloLog";

	private ILoggable[] loggableObjects; //This collection contains objects whose state should be logged on Update
	private const string dateTimeFormat = "HH_mm_ss_dd-MM-yyyy"; //This string represents the DateTime output format for the filename
	private StreamWriter rawWriter = null; //This writer is used to write to file
	private StreamWriter summaryWriter = null;
	private string currentFileTimestamp = "";
	private int subID;
    private int sessID;
    private bool enableLogging;
	string firstTickOutput = "";
	string lastTickOutput = "";
	string previousTickOutput = "";
	private bool paused = false;

	public void Pause(){
		paused = true;
	}
	public void Resume(){
		paused = false;
	}
	public void Start() {
		subID = PlayerPrefs.GetInt ("SubjectNumber");
        sessID = PlayerPrefs.GetInt("SessionNumber");
        if (extension[0] != '.')
            extension = "." + extension;
	}
	
	// Update is called once per frame
	public void Update () {
        if (loggableObjects != null && rawWriter != null && loggableObjects.Length > 0 && !paused)
        {
						//Write a timestamp for data stability
						string timestamp = DateTime.Now.ToBinary () + "";
						rawWriter.WriteLine (timestamp);
						//Output all object information
						StringBuilder tickOutputBuilder = new StringBuilder();
						for (int i = 0; i < loggableObjects.Length; i++) {
								tickOutputBuilder.Append (loggableObjects [i].getObjectStateLogData ());
								tickOutputBuilder.Append ("\r\n");
						}
						string tickOutput = tickOutputBuilder.ToString ();
						if(!tickOutput.Equals(previousTickOutput)){
							if(firstTickOutput=="")
								firstTickOutput = timestamp+"\r\n"+tickOutput;
							rawWriter.Write(tickOutput);
							lastTickOutput = timestamp+"\r\n"+tickOutput;
						}
						previousTickOutput = tickOutput;
				}
		}

	public void GenerateLoggableObjectsList(){
		//Get the list of objects - this is a one-time function. If new objects are created, there is currently no way to log them without creating a new logger.
        List<ILoggable> output = new List<ILoggable>();
        HelperFunctions.GetScriptObjectsInScene<ILoggable>(out output);
		loggableObjects = output.ToArray ();
	}

	public void BeginLogging(){
        // !!! COMMENTED OUT!
        //if (!Directory.Exists(loggerDir))
        //    Directory.CreateDirectory(loggerDir);
		string substring = ("Sub" + subID.ToString("D4") + "Sess" + sessID.ToString("D2"));

		GenerateLoggableObjectsList ();

		//Debug.Log ("Found " + loggableObjects.Length + " ILoggable objects.");
		
		//Create the appropriate filename given the options
        string rawFilename = loggerDir + "RawLog" + extension;
		currentFileTimestamp = DateTime.Now.ToString (dateTimeFormat);
		rawFilename = appendTextToFilename (rawFilename,substring);
		rawFilename = appendTextToFilename (rawFilename,currentFileTimestamp);

        //Debug.Log("Raw Filename: " + rawFilename);
		//Create the file writer
		rawWriter = new StreamWriter (rawFilename, false);
		rawWriter.AutoFlush = true;

		//Create the appropriate filename given the options
        string summaryFilename = loggerDir + "SummaryLog" + extension;
		summaryFilename = appendTextToFilename (summaryFilename,substring);
		summaryFilename = appendTextToFilename (summaryFilename,currentFileTimestamp);

		//Create the file writer
		summaryWriter = new StreamWriter (summaryFilename);
		summaryWriter.WriteLine ("Trial\tStimulus\tStimulusID\tStartPosXYZ\tEndPosXYZ\tStartRotXYZ\tEndRotXYZ");
	}

    public void RegenerateStimuliInSameFile()
    {
        rawWriter.WriteLine("----------New Stimuli List Generated----------");
        GenerateLoggableObjectsList();
        Resume();
    }

	public void FinishTrial(int trialNum){
		string[] trialLines = firstTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		string[] trialLinesEnd = lastTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 1; i < trialLines.Length; i++) {
			string[] lineSplit = trialLines[i].Split (new string[]{":",","},StringSplitOptions.None);
			string[] endLineSplit = trialLinesEnd[i].Split(new string[]{":",","},StringSplitOptions.None);
			summaryWriter.Write (trialNum + "\t");
			summaryWriter.Write ((i)+"\t"+lineSplit[0]+"\t");
			summaryWriter.Write (lineSplit[1]+","+lineSplit[2]+","+lineSplit[3]+"\t");
			summaryWriter.Write (endLineSplit[1]+","+endLineSplit[2]+","+endLineSplit[3]+"\t");
			summaryWriter.Write (lineSplit[4]+","+lineSplit[5]+","+lineSplit[6]+"\t");
			summaryWriter.WriteLine (endLineSplit[4]+","+endLineSplit[5]+","+endLineSplit[6]);
		}
		firstTickOutput = "";
		lastTickOutput = "";
		previousTickOutput = "";
		rawWriter.WriteLine ("End of Trial " + trialNum);
        Pause();
	}

	public void Finish(){
		rawWriter.WriteLine ("End of File");
		rawWriter.Close ();
		summaryWriter.Close ();
	}

	//When quitting, attempt to close the streams
	void OnApplicationQuit() {
		try{rawWriter.Close ();}catch(Exception){}
		try{summaryWriter.Close ();}catch(Exception){}
	}

	private string appendTextToFilename(string filename, string text){
		string[] fileTokens = filename.Split(new string[]{"."},System.StringSplitOptions.None);
		string output = "";
		for (int i = 0; i < fileTokens.Length - 1; i++)
			output += fileTokens [i] + "_";
		return output + text + "." + fileTokens [fileTokens.Length - 1];
	}
}
