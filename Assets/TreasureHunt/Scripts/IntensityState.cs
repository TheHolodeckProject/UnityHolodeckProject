using UnityEngine;
using System.Collections;

public class IntensityState : MonoBehaviour {

	public GameObject MagneticPinchHand;
	public UnitySerialPort PortScript;

	// Use this for initialization
	void Start () {
		MagneticPinchHand = GameObject.Find ("RigidHandRight");
		PortScript = MagneticPinchHand.GetComponent ("UnitySerialPort") as UnitySerialPort;
		//PortScript.middleTip
		//PortScript = (UnitySerialPort) GameObject.FindObjectOfType (typeof(UnitySerialPort));
		//GUIScript = GameObject.FindGameObjectWithTag("PlayerAnimation").GetComponent<MyAnimationScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(UnityEngine.Collision hit)
	{
		if (hit.gameObject.ToString() == "Cube (UnityEngine.GameObject)")
		{
			if(PortScript != null && PortScript.ConnectionIsOpen())
			{
				if(this.transform.parent.gameObject.ToString() == "thumb (UnityEngine.GameObject)")
				{
					PortScript.thumbTip = 255;
					//PortScript.thumbTip = 255;
					print ("ThumbIn");
				}
				if(this.transform.parent.gameObject.ToString() == "index (UnityEngine.GameObject)")
				{
					PortScript.indexTip = 255;
					print ("IndexIn");
				}
				if(this.transform.parent.gameObject.ToString() == "middle (UnityEngine.GameObject)")
				{
					PortScript.middleTip = 255;
					print ("MiddleIn");
				}
				if(this.transform.parent.gameObject.ToString() == "ring (UnityEngine.GameObject)")
				{
					PortScript.ringTip = 255;
					print ("RingIn");
				}
				if(this.transform.parent.gameObject.ToString() == "pinky (UnityEngine.GameObject)")
				{
					PortScript.pinkyTip = 255;
					print ("PinkyIn");
				}
			}
		}
	}

	void OnCollisionExit(UnityEngine.Collision hit)
	{
		if (hit.gameObject.ToString() == "Cube (UnityEngine.GameObject)")
		{
			if(1==1)//if(PortScript != null && PortScript.ConnectionIsOpen())
			{
				if(this.transform.parent.gameObject.ToString() == "thumb (UnityEngine.GameObject)")
				{
					PortScript.thumbTip = 0;
					print ("ThumbOut");
				}
				if(this.transform.parent.gameObject.ToString() == "index (UnityEngine.GameObject)")
				{
					PortScript.indexTip = 0;
					print ("IndexOut");
				}
				if(this.transform.parent.gameObject.ToString() == "middle (UnityEngine.GameObject)")
				{
					PortScript.middleTip = 0;
					print ("MiddleOut");
				}
				if(this.transform.parent.gameObject.ToString() == "ring (UnityEngine.GameObject)")
				{
					PortScript.ringTip = 0;
					print ("RingOut");
				}
				if(this.transform.parent.gameObject.ToString() == "pinky (UnityEngine.GameObject)")
				{
					PortScript.pinkyTip = 0;
					print ("PinkyOut");
				}
			}
		}
	}
}
