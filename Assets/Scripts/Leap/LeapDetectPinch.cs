// Detects when a pinching gesture is made and where the thumb tip is while pinching

using UnityEngine;
using System.Collections;
using Leap;

public class LeapDetectPinch : MonoBehaviour {
  public const float pinchDistanceRatio = 0.7f;
  //protected bool pinching;
    //?? This stops being accessible if it's made private. You mentioned making variables public wasn't the best way to access them. Better way?
  public bool pinching;
  public Vector3 thumbPosition;
  public Vector3 indexPosition;
  public Vector3 thumbRotation;
  public Vector3 indexRotation;

  void Start() {
    pinching = false;
  }
  void OnPinch()
  {
      pinching = true;
  }
  void OnRelease()
  {
      pinching = false;
  }
  void Update() {
    
      
      bool trigger_pinch = false;
    HandModel hand_model = GetComponent<HandModel>();
    Hand leap_hand = hand_model.GetLeapHand();
          if (leap_hand == null)
              return;

    // Scale trigger distance by thumb proximal bone length.
    Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
    Vector leap_index_tip = leap_hand.Fingers[1].TipPosition;
    Vector leapThumbDirection = leap_hand.Fingers[0].Direction;
    Vector leapIndexDirection = leap_hand.Fingers[1].Direction;
               //Defines the position of the pinch as the thumb tip
           thumbPosition = leap_thumb_tip.ToUnityScaled(true);
           indexPosition = leap_index_tip.ToUnityScaled(true);
           thumbRotation = leapThumbDirection.ToUnity() * 90;
           indexRotation = leapIndexDirection.ToUnity() * 90;

    float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
    float trigger_distance = proximal_length * pinchDistanceRatio;
  
    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i)
    {
        Finger finger = leap_hand.Fingers[i];
        for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j)
        {
            Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
            if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
            {
                trigger_pinch = true;
                Debug.Log("trigger distance = " + trigger_distance + ". Finger distance = " + leap_joint_position.DistanceTo(leap_thumb_tip));
            }
        }
    }



    // Only change state if it's different.
    if (trigger_pinch && !pinching)
    OnPinch();
        else if (!trigger_pinch && pinching)
      OnRelease();
  }
}
