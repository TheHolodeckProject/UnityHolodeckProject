/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Leap;

public enum RecorderState {
  Idling = 0,
  Recording = 1,
  Playing = 2
}

public struct FrameData
{
    public bool isLeft;
    public Vector3 handPos, handRotAxis, handTrans;
    public float handRotX, handRotY, handRotZ;
    

    public FrameData(bool left, Vector3 pos, Vector3 axis, Vector3 trans, float x, float y, float z)
    {
        isLeft = left;
        handPos = pos;
        handRotAxis = axis;
        handTrans = trans;
        handRotX = x;
        handRotY = y;
        handRotZ = z;

    }





}

public class LeapRecorder {

  public float speed = 1.0f;
  public bool loop = true;
  public RecorderState state = RecorderState.Playing;
  public Frame sinceFrame;
  protected List<byte[]> frames_;
  protected float frame_index_;
  protected Frame current_frame_ = new Frame();
  private string savePath = "Assets/TreasureHunt/Logs/";
  private List<FrameData> dataFrames =  new List<FrameData>();
  
  public LeapRecorder() {
    Reset();
  }

  public void Stop() {
    state = RecorderState.Idling;
    frame_index_ = 0.0f;
  }

  public void Pause() {
    state = RecorderState.Idling;
  }

  public void Play() {
    state = RecorderState.Playing;
  }

  public void Record() {
    state = RecorderState.Recording;
  }
  
  public void Reset() {
    frames_ = new List<byte[]>();
    frame_index_ = 0;
  }
  
  public void SetDefault() {
    speed = 1.0f;
    loop = true;
  }

  public float GetProgress() {
    return frame_index_ / frames_.Count;
  }

  public int GetIndex() {
    return (int)frame_index_;
  }

  public void SetIndex(int new_index) { 
    if (new_index >= frames_.Count) {
      frame_index_ = frames_.Count - 1;
    }
    else {
      frame_index_ = new_index; 
    }
  }
  
  public void AddFrame(Frame frame) {
    frames_.Add(frame.Serialize);
    HandList hands = frame.Hands;

    
       
      
      if (dataFrames.Count ==0)
      {
          
          sinceFrame = frame;
      }
     
      
          foreach (Hand han in hands)
          {
                     
              dataFrames.Add(new FrameData(han.IsLeft, new Vector3(han.PalmPosition.x, han.PalmPosition.y, han.PalmPosition.z), new Vector3(han.RotationAxis(sinceFrame).x, han.RotationAxis(sinceFrame).y,
                  han.RotationAxis(sinceFrame).z), new Vector3(han.Translation(sinceFrame).x, han.Translation(sinceFrame).y, han.Translation(sinceFrame).z), han.RotationAngle(sinceFrame, Vector.XAxis),
                                                                                  han.RotationAngle(sinceFrame, Vector.YAxis), han.RotationAngle(sinceFrame, Vector.ZAxis)));
          }
         // SaveTrialData();
      
  }

  public Frame GetCurrentFrame() {
    return current_frame_;
  }
  
  public Frame NextFrame() {
    current_frame_ = new Frame();
    if (frames_.Count > 0) {
      if (frame_index_ >= frames_.Count && loop) {
        frame_index_ -= frames_.Count;
      }
      else if (frame_index_ < 0 && loop) {
        frame_index_ += frames_.Count;
      }
      if (frame_index_ < frames_.Count && frame_index_ >= 0) {
        current_frame_.Deserialize(frames_[(int)frame_index_]);
        frame_index_ += speed;
      }
    }
    return current_frame_;
  }
  
  public List<Frame> GetFrames() {
    List<Frame> frames = new List<Frame>();
    for (int i = 0; i < frames_.Count; ++i) {
      Frame frame = new Frame();
      frame.Deserialize(frames_[i]);
      frames.Add(frame);
    }
    return frames;
  }
  
  public int GetFramesCount() {
    return frames_.Count;
  }
  
  public string SaveToNewFile() {
    string path = Application.persistentDataPath + "/Recording_" +
                  System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".bytes";

    if (File.Exists(@path)) {
      File.Delete(@path);
    }

    FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
   
    for (int i = 0; i < frames_.Count; ++i) {
      byte[] frame_size = new byte[4];
      
          frame_size = System.BitConverter.GetBytes(frames_[i].Length);
     
     
          stream.Write(frame_size, 0, frame_size.Length);
      stream.Write(frames_[i], 0, frames_[i].Length);
    }

    stream.Close();
    return path;
  }

  
  public void Load(TextAsset text_asset) {
    byte[] data = text_asset.bytes;
     frame_index_ = 0;
    frames_.Clear();
    int i = 0;
    while (i < data.Length) {
      byte[] frame_size = new byte[4];
      Array.Copy(data, i, frame_size, 0, frame_size.Length);
      i += frame_size.Length;
      byte[] frame = new byte[System.BitConverter.ToUInt32(frame_size, 0)];
      Array.Copy(data, i, frame, 0, frame.Length);
      i += frame.Length;
      frames_.Add(frame);
    }
      }

  public void SaveTrialData()
  {
     
      string fileName = savePath + "_" + PlayerPrefs.GetInt("subjectIdentifier") + System.DateTime.Now.ToString("_yyyyMMdd_HHmm") + ".txt";
     
      if(!System.IO.File.Exists(fileName)){
      using ( FileStream fs = File.Create(fileName))
      {
          using( StreamWriter sr = new StreamWriter(fs))
          foreach (FrameData framDat in dataFrames)
          {
              string hand = (framDat.isLeft) ? "LeftHand" : "RighHand";
              sr.WriteLine(hand + ", " + framDat.handPos + ", " + framDat.handRotAxis + ", (" + framDat.handRotX + "," + framDat.handRotY + "," + framDat.handRotZ + "), " + framDat.handTrans + "\n\n");
          }
      }
      }
      else if (System.IO.File.Exists(fileName))
      {
         
              using (StreamWriter sr = File.AppendText(fileName))
                  foreach (FrameData framDat in dataFrames)
                  {
                      string hand = (framDat.isLeft) ? "LeftHand" : "RighHand";
                      sr.WriteLine(hand + ", " + framDat.handPos + ", " + framDat.handRotAxis + ", (" + framDat.handRotX + "," + framDat.handRotY + "," + framDat.handRotZ + "), " + framDat.handTrans + "\n\n");
                  }
          
      }
  

     
  }

  public void SaveTrialData(string header)
  {


    /*  string fileName = savePath + "_" + PlayerPrefs.GetInt("subjectIdentifier") + System.DateTime.Now.ToString("_yyyyMMdd_HHmm") + ".txt";

      if (!System.IO.File.Exists(fileName))
      {
          using (FileStream fs = File.Create(fileName))
          {
              using (StreamWriter sr = new StreamWriter(fs))
              {
                  sr.WriteLine(header + "\n");
                  foreach (FrameData framDat in dataFrames)
                  {
                      string hand = (framDat.isLeft) ? "LeftHand" : "RighHand";
                      sr.WriteLine(hand + ", " + framDat.handPos + ", " + framDat.handRotAxis + ", (" + framDat.handRotX + "," + framDat.handRotY + "," + framDat.handRotZ + "), " + framDat.handTrans + "\n\n");
                  }
              }
          }
      }
      else if (System.IO.File.Exists(fileName))
      { 

          using (StreamWriter sr = File.AppendText(fileName))
              foreach (FrameData framDat in dataFrames)
              {
                  string hand = (framDat.isLeft) ? "LeftHand" : "RighHand";
                  sr.WriteLine(hand + ", " + framDat.handPos + ", " + framDat.handRotAxis + ", (" + framDat.handRotX + "," + framDat.handRotY + "," + framDat.handRotZ + "), " + framDat.handTrans + "\n\n");
              }

      }*/



  }
    

}
