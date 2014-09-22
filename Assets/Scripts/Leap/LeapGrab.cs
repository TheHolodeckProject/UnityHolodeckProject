/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Leap Motion hand script that detects pinches and grabs the
// closest rigidbody with a spring force if it's within a given range.
public class LeapGrab : MonoBehaviour
{

    private const float TRIGGER_DISTANCE_RATIO = 0.7f;

    private bool pinching_;
    private Collider grabbed_;
    private Collider rightpinched;
    //private Collider rightthumbtipcollider;
    private Finger rightpinchfinger;
    private Bone rightpinchbone;

    void Start()
    {
        pinching_ = false;
        grabbed_ = null;
    }

    void OnPinch(Vector3 pinch_position)
    {
        pinching_ = true;

        // Check if we pinched a movable object and grab the closest one that's not part of the hand.
        //Collider[] close_things = Physics.OverlapSphere(pinch_position, magnetDistance);
        //Vector3 distance = new Vector3(magnetDistance, 0.0f, 0.0f);

        //for (int j = 0; j < close_things.Length; ++j)
        //{
        //    Vector3 new_distance = pinch_position - close_things[j].transform.position;
        //    if (close_things[j].rigidbody != null && new_distance.magnitude < distance.magnitude &&
        //        !close_things[j].transform.IsChildOf(transform))
        //    {
        //        grabbed_ = close_things[j];
        //        distance = new_distance;
        //    }
        //}
    }

    void OnRelease()
    {
        grabbed_ = null;
        pinching_ = false;
    }

    void Update()
    {
        HandModel hand_model = GetComponent<HandModel>();
        Hand leap_hand = hand_model.GetLeapHand();

        if (leap_hand == null)
            return;

        // Scale trigger distance by thumb proximal bone length.
        Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
        float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
        float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;
        
        //Finds the tip of the thumb, which is its own game object
        GameObject rightthumbtip = GameObject.Find("MagneticPinchHand(Clone)/thumb/bone1");
        //If the thumb is in the scene
        if (rightthumbtip != null)
           // Collider rightthumbtip = 
            //Checks if its collider is hitting anything with the tag "StrechPoint"
        //    if rightthumbtip.collider.
           

        //else
        //    Debug.Log("Thumb's not here");
    

        //Collider thumbcollider = leap_hand.Fingers[1].

        // Check if thumb is colliding with a strechable point
        //if 
        //leap_hand.

        //!!! Could maybe make this more efficient by not doing it for the thumb
        for (int i = 1; i < HandModel.NUM_FINGERS; ++i)
        {
            Finger finger = leap_hand.Fingers[i];


            //!!! Could maybe make this more efficient by only looking at the fingertip bone
            for (int j = 0; j < FingerModel.NUM_BONES; ++j)
            {
                Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
                if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
                {
                    //rightpinchbone = finger.Bone((Bone.BoneType)j);
                    rightpinchfinger = leap_hand.Fingers[i];
                    //!!! Could maybe make this more efficient by not going through the rest of the fingers if one of them is pinching
                }
            }
        }

        //If pinching, check if the two fingers pinching are colliding with one of the colliders
            //Collider rightpinched = collider;


            ////Converts the Leap position vector to a unity Vector3
            //Vector3 pinchposition = UnityVectorExtension.ToUnityScaled(rightpinchfinger.TipPosition);
            //if (Physics.CheckSphere(pinchposition, 0.01f))
            //    Debug.Log("Pinching something...");
            ////Collision.Equals(rightpinch)
    }

        //Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();

        // Only change state if it's different.
        //if (trigger_pinch && !pinching_)
        //    OnPinch(pinch_position);
        //else if (!trigger_pinch && pinching_)
        //    OnRelease();

        //// Accelerate what we are grabbing toward the pinch.
        //if (grabbed_ != null)
        //{
        //    Vector3 distance = pinch_position - grabbed_.transform.position;
        //    grabbed_.rigidbody.AddForce(forceSpringConstant * distance);
        //}
    }
