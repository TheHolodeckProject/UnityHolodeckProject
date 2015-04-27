/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;
using System.IO;
using System.Text;

// Leap Motion hand script that detects pinches and grabs the
// closest rigidbody with a spring force if it's within a given range.
public class MagneticPinch2 : MonoBehaviour {

  private const float TRIGGER_DISTANCE_RATIO = 0.8f;

  public float forceSpringConstant = 100.0f;
  public float magnetDistance = 5f;
  
 

  private bool pinching_;
  private Collider grabbed_;
  string[] datapoints = new string[50];
  private SearchSpace ssScript;
  private float tThresh = 2f;
  private float tstart = 0f;
  private bool wait = false;
  private bool fromRelease = false;
  private bool open_hand = true;
  private string path;
  public float confidence;
  void Start() {

      ssScript = GameObject.FindObjectOfType(typeof(SearchSpace)) as SearchSpace;
       path = ssScript.getPathToSubDataFile();
    pinching_ = false;
    grabbed_ = null;
  }

  void OnPinch(Vector3 pinch_position) {

      if (pinching_ == false)
      {
          tstart = Time.time;
          pinching_ = true;
          print("Pinch recognized");
      }
      
      
      if (!(ssScript.pinchCount == ssScript.markLocations.Length) && pinching_)
      {
         ssScript.statusMssg = "Pinched for: " + (Time.time - tstart).ToString();
          if (Time.time - tstart > tThresh)
          {
              
              
                  string s1 = " Point recorded at " + Time.time.ToString() + "\n";
                  ssScript.stimMarkTimes.Add(s1);
                  

              
              
              pinch_position.x = pinch_position.x * 14.869f - .1087f;
              pinch_position.y = pinch_position.y * 14.781f + .8396f;
              pinch_position.z = pinch_position.z * 14.656f + .2221f;
              ssScript.markLocations[ssScript.pinchCount] = pinch_position;
              ssScript.pinchCount++;
              print("Point recorded");
              wait = true;
              print("Wait is: " + wait);
              tstart = Time.time;
              ssScript.statusMssg = "Please release and wait to be instructed for next pinch...";
              ssScript.riftTextPlane2.GetComponent<TextMesh>().text = "Please release and wait to be instructed for next pinch...";
              print("waiting....");
              open_hand = false;
              
          }
          
      }

     
      
  }

  void OnRelease() {
    grabbed_ = null;
    pinching_ = false;
    fromRelease = true;
   
    
  }

  void Update() {

      
      if (fromRelease && wait){
          wait = false;
          tstart = Time.time;
      }
      if(fromRelease && Time.time - tstart > 5f){
          fromRelease = false;
          ssScript.statusMssg = "Ok for next pinch....pinch for 2 secs";
          ssScript.riftTextPlane2.GetComponent<TextMesh>().text = "Ok for next pinch....pinch for 2 secs";
      }
    bool trigger_pinch = false;
                          
    HandModel hand_model = GetComponent<HandModel>();
    Hand leap_hand = hand_model.GetLeapHand();
      confidence = leap_hand.Confidence;
    if (leap_hand == null)
      return;

    // Scale trigger distance by thumb proximal bone length.
    Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
    float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
    float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i)
    {
        Finger finger = leap_hand.Fingers[i];
                                                                                                      // see if we are pinching, if so trigger pinch is true
        for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j)
        {
            Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
            if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
                trigger_pinch = true;
        }
    }

    if( !open_hand)
    {
        Finger finger = leap_hand.Fingers[1];
        
        // see if we are pinching, if so trigger pinch is true
        
            Vector leap_joint_position = finger.Bone((Bone.BoneType)2).NextJoint;
            if (leap_joint_position.DistanceTo(leap_thumb_tip) > trigger_distance)
                open_hand = true;
        
    }


    Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();

    // Only change state if it's different.
    if (trigger_pinch && open_hand &&  confidence > .75)
      OnPinch(pinch_position);
    else if (!trigger_pinch && pinching_)
      OnRelease();

    // Accelerate what we are grabbing toward the pinch.
    
  }

 
}
