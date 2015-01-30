using UnityEngine;
using System.Collections;
using System;
using System.IO;
//using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Logger : MonoBehaviour {
    //TODO: Add ability to disable raw logger
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

    public bool EnableRawFile = true;

    public bool EnableTrialNumberInSummaryFile = true;
    public bool EnableDifficultyInSummaryFile = true;
    public bool EnableNameOfObjectInSummaryFile = true;
    public bool EnablePositionInSummaryFile = true;
    public bool EnableRotationInSummaryFile = true;
    //ADD NEW ENABLE DISABLE SUMMARY FILE OPTION VARIABLES HERE

	public void Pause(){
		paused = true;
	}
	public void Resume(){
		paused = false;
	}
	
	// Update is called once per frame
	public void Update () {
        if (loggableObjects != null && (!EnableRawFile || rawWriter != null) && loggableObjects.Length > 0 && !paused)
        {
						//Write a timestamp for data stability
						string timestamp = DateTime.Now.ToBinary () + "";
                        if (EnableRawFile)
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
                            if (EnableRawFile)
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

    public void BeginLogging()
    {
        // !!! Had to move this down here because it was runnnig before Start, for some reason
        subID = PlayerPrefs.GetInt("SubjectNumber");
        sessID = PlayerPrefs.GetInt("SessionNumber");
        if (extension[0] != '.')
            extension = "." + extension;
        // !!! COMMENTED OUT!
        //if (!Directory.Exists(loggerDir))
        //    Directory.CreateDirectory(loggerDir);
        string substring = ("Sub" + subID.ToString("D4") + "_Sess" + sessID.ToString("D2") + "_Timestamp");


        GenerateLoggableObjectsList();

        //Debug.Log ("Found " + loggableObjects.Length + " ILoggable objects.");

        //Create the appropriate filename given the options
        currentFileTimestamp = DateTime.Now.ToString(dateTimeFormat);
        if (EnableRawFile)
        {
            string rawFilename = loggerDir + "RawLog" + extension;
            rawFilename = appendTextToFilename(rawFilename, substring);
            rawFilename = appendTextToFilename(rawFilename, currentFileTimestamp);

            //Debug.Log("Raw Filename: " + rawFilename);
            //Create the file writer
            rawWriter = new StreamWriter(rawFilename, false);
            rawWriter.AutoFlush = true;
        }
        //Create the appropriate filename given the options
        string summaryFilename = loggerDir + "SummaryLog" + extension;
        summaryFilename = appendTextToFilename(summaryFilename, substring);
        summaryFilename = appendTextToFilename(summaryFilename, currentFileTimestamp);

        //Create the file writer
        summaryWriter = new StreamWriter(summaryFilename);

        // ??? How to change what variables get logged? EndRotXYZ shouldn't be there, but I want to add difficulty (called "diff" in movetask)

        StringBuilder summaryHeaderBuilder = new StringBuilder();
        
        if (EnableTrialNumberInSummaryFile)
            summaryHeaderBuilder.Append("Trial\t");
        if (EnableDifficultyInSummaryFile)
            summaryHeaderBuilder.Append("Difficulty\t");
        if (EnableNameOfObjectInSummaryFile)
            summaryHeaderBuilder.Append("Stimulus\tStimulusID\t");
        if (EnablePositionInSummaryFile)
            summaryHeaderBuilder.Append("StartPosXYZ\tEndPosXYZ\t");
        if (EnableRotationInSummaryFile)
            summaryHeaderBuilder.Append("StartRotXYZ\tEndRotXYZ\t");
        //ADD NEW ENABLE OR DISABLE HEADER INFO HERE

        summaryHeaderBuilder.AppendLine();
        summaryWriter.WriteLine(summaryHeaderBuilder.ToString().Trim());
    }

    public void RegenerateStimuliInSameFile()
    {
        if (EnableRawFile)
            rawWriter.WriteLine("----------New Stimuli List Generated----------");
        GenerateLoggableObjectsList();
        Resume();
    }

    // !!!! GET IT TO OUTPUT DIFFICULTY
	public void FinishTrial(int trialNum, int difficulty = -1){
		string[] trialLines = firstTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		string[] trialLinesEnd = lastTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 1; i < trialLines.Length; i++) {
			string[] lineSplit = trialLines[i].Split (new string[]{":",","},StringSplitOptions.None);
			string[] endLineSplit = trialLinesEnd[i].Split(new string[]{":",","},StringSplitOptions.None);
            StringBuilder sampleString = new StringBuilder();
            if (EnableTrialNumberInSummaryFile)
            {
                sampleString.Append(trialNum);
                sampleString.Append("\t");
            }
            if (EnableDifficultyInSummaryFile)
            {
                sampleString.Append(difficulty);
                sampleString.Append("\t");
            }
            if (EnableNameOfObjectInSummaryFile)
            {
                sampleString.Append(i);
                sampleString.Append("\t");
                sampleString.Append(lineSplit[0]);
                sampleString.Append("\t");
            }
            if (EnablePositionInSummaryFile)
            {
                sampleString.Append(lineSplit[1]);
                sampleString.Append(",");
                sampleString.Append(lineSplit[2]);
                sampleString.Append(",");
                sampleString.Append(lineSplit[3]);
                sampleString.Append("\t");
                sampleString.Append(endLineSplit[1]);
                sampleString.Append(",");
                sampleString.Append(endLineSplit[2]);
                sampleString.Append(",");
                sampleString.Append(endLineSplit[3]);
                sampleString.Append("\t");
            }
            if (EnableRotationInSummaryFile)
            {
                sampleString.Append(lineSplit[4]);
                sampleString.Append(",");
                sampleString.Append(lineSplit[5]);
                sampleString.Append(",");
                sampleString.Append(lineSplit[6]);
                sampleString.Append("\t");
                sampleString.Append(endLineSplit[4]);
                sampleString.Append(",");
                sampleString.Append(endLineSplit[5]);
                sampleString.Append(",");
                sampleString.Append(endLineSplit[6]);
                sampleString.Append("\t");
            }
            //ADD NEW SAMPLE RENDER CODE HERE

            summaryWriter.WriteLine(sampleString.ToString().Trim());
		}
		firstTickOutput = "";
		lastTickOutput = "";
		previousTickOutput = "";
        if (EnableRawFile)
        {
            rawWriter.WriteLine("End of Trial " + trialNum);
            rawWriter.Flush();
        }
        summaryWriter.Flush();
        Pause();
	}

	public void Finish(){
        if (EnableRawFile)
        {
            rawWriter.WriteLine("End of File");
            rawWriter.Close();
        }
		summaryWriter.Close ();
	}

	//When quitting, attempt to close the streams
	void OnApplicationQuit() {
        try { 
            if (EnableRawFile)
                rawWriter.Close(); 
        }
        catch (Exception) { }
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
