﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Leap Motion hand script that detects pinches and grabs the
// closest rigidbody with a spring force if it's within a given range.
public class LeapDetectPinch : MonoBehaviour
{

    private const float TRIGGER_DISTANCE_RATIO = 0.7f;
    private bool pinching;
    private Collider grabbed_;
    private HandModel hand_model;
    private Hand lep_hand;

    void Start()
    {
        pinching = false;
    }

    void OnPinch(Vector3 pinch_position)
    {
        pinching = true;
        Debug.Log("Pinching");
    }

    void OnRelease()
    {
        pinching = false;
    }

    void Update() {
        if (GameObject.Find("RiggedLeftHand(Clone)") != null)
        {
            Debug.Log("Found the left hand!");
            bool trigger_pinch = false;
            HandModel hand_model = GetComponent<HandModel>();
            Hand leap_hand = hand_model.GetLeapHand();

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

                for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j)
                {
                    Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
                    if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
                        trigger_pinch = true;
                }
            }

            Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();

            // Only change state if it's different.
            if (trigger_pinch && !pinching)
                OnPinch(pinch_position);
            else if (!trigger_pinch && pinching)
                OnRelease();
        }
        else Debug.Log("No left hand in the scene");
  }
}
