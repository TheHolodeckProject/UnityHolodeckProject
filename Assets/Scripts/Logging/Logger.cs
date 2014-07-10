using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Logger : MonoBehaviour {
	
	public string logFilename = "log.dat"; //The filename for logging this file will always be overwritten if name is identical (enable appendDateTimeToFilename to ensure filename independence)
	public bool appendDateTimeToFilename = true; //When true, the DateTime will be appended to the filename so it remains unique

	private ILoggable[] loggableObjects; //This collection contains objects whose state should be logged on Update
	private const string dateTimeFormat = "_hh-mm-ss_dd-MM-yyyy"; //This string represents the DateTime output format for the filename
	private StreamWriter writer = null; //This writer is used to write to file
	private string currentFileTimestamp = "";

	public void Reset() {
		writer.Close ();
		writer = null;
		currentFileTimestamp = "";
		firstTickOutput = "";
		lastTickOutput = "";
		previousTickOutput = "";
		Start ();
	}

	void Start() {

		//Get the list of objects - this is a one-time function. If new objects are created, there is currently no way to log them without creating a new logger.
		List<ILoggable> logObjs = new List<ILoggable> ();
		GameObject[] objs = (GameObject[])FindObjectsOfType (typeof(GameObject));
		//Debug.Log ("Searching " + objs.Length + " GameObject objects for ILoggable interfaces.");
		for (int i = 0; i < objs.Length; i++) {
						List<ILoggable> logScripts = new List<ILoggable> ();
						GetInterfaces<ILoggable> (out logScripts, objs [i]);
						if (logScripts.Count > 0)
								logObjs.AddRange (logScripts);
				}

		loggableObjects = logObjs.ToArray ();
		//Debug.Log ("Found " + loggableObjects.Length + " ILoggable objects.");

		//Create the appropriate filename given the options
		string filename = logFilename;
		if (appendDateTimeToFilename){
			currentFileTimestamp = DateTime.Now.ToString (dateTimeFormat);
			filename = appendTextToFilename (filename,currentFileTimestamp);
		}

		//Create the file writer
		writer = new StreamWriter (filename, false);
		writer.AutoFlush = true;
	}

	string firstTickOutput = "";
	string lastTickOutput = "";

	string previousTickOutput = "";
	// Update is called once per frame
	void Update () {
				if (!(loggableObjects == null || loggableObjects.Length <= 0 || writer == null)) {
						//Write a timestamp for data stability
						string timestamp = DateTime.Now.ToBinary () + "";
						writer.WriteLine (timestamp);
						//Output all object information
						StringBuilder tickOutputBuilder = new StringBuilder();
						for (int i = 0; i < loggableObjects.Length; i++) {
								tickOutputBuilder.AppendLine (loggableObjects [i].getObjectStateLogData ());
						}
						string tickOutput = tickOutputBuilder.ToString ();
						if(!tickOutput.Equals(previousTickOutput)){
							if(firstTickOutput=="")
								firstTickOutput = timestamp+"\r\n"+tickOutput;
							writer.Write(tickOutput);
							lastTickOutput = timestamp+"\r\n"+tickOutput;
						}
						previousTickOutput = tickOutput;
				}
		}

	public void GenerateSummaryFile(){
		//Create the appropriate filename given the options
		string filename = logFilename;
		if (appendDateTimeToFilename)
			filename = appendTextToFilename (filename,currentFileTimestamp);
		filename = appendTextToFilename (filename, "_Summary");

		StreamWriter summaryWriter = new StreamWriter (filename);
		summaryWriter.WriteLine ("-------Initial Position-------");
		summaryWriter.WriteLine (firstTickOutput);
		summaryWriter.WriteLine ("-------Final Position-------");
		summaryWriter.WriteLine (lastTickOutput);
		summaryWriter.Close ();
	}

	//When quitting, attempt to close the stream
	void OnApplicationQuit() {
		try{writer.Close ();}catch(Exception){}
		}

	private string appendTextToFilename(string filename, string text){
		string[] fileTokens = filename.Split(new string[]{"."},System.StringSplitOptions.None);
		string output = "";
		for (int i = 0; i < fileTokens.Length - 1; i++)
			output += fileTokens [i] + ".";
		return output + text + "." + fileTokens [fileTokens.Length - 1];
	}

	public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T: class {
		MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
		resultList = new List<T>();
		foreach(MonoBehaviour mb in list){
			if(mb is T){
				//found one
				resultList.Add((T)((System.Object)mb));
			}
		}
	}
}
