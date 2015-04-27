using UnityEngine;
using System.Collections;

public class Webcam : MonoBehaviour 
{

/*	public MeshRenderer[] UseWebcamTexture;

	public WebCamTexture webcamTexture;
	private Quaternion baseRotation;
	private string cameraName;
	private OVRCameraController CameraController;


	// Use this for initialization
	void Start () 
	{
		/*WebCamDevice[] devices = WebCamTexture.devices;
		webcamTexture = new WebCamTexture();
		print ("devices: " + devices.Length);
		if (devices.Length > 1)
		{
			webcamTexture.deviceName = devices [1].name;
		}
		else
		{
			webcamTexture.deviceName = devices [0].name;
		}

		foreach(MeshRenderer r in UseWebcamTexture)
		{
			r.material.mainTexture = webcamTexture;
		}

		// Find camera controller
		OVRCameraController[] CameraControllers;
		CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController>();
		
		if(CameraControllers.Length == 0)
			Debug.LogWarning("OVRMainMenu: No OVRCameraController attached.");
		else if (CameraControllers.Length > 1)
			Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraController attached.");
		else{
			CameraController = CameraControllers[0];
		}

		if (gameObject.ToString () == "RightCamera (UnityEngine.GameObject)") {
			cameraName = "Right";
			webcamTexture = new WebCamTexture("UI325xLE-C_4102750497");
			transform.rotation = Quaternion.Euler (0, 0, -90) * transform.rotation;
		} else {
			cameraName = "Left";
			webcamTexture = new WebCamTexture("UI325xLE-C_4102750498");
			transform.rotation = Quaternion.Euler (0, 0, 90) * transform.rotation;
		}

		renderer.material.mainTexture = webcamTexture;

		webcamTexture.Play();
	}

	/*void Update() {
		transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);
	}

	void OnGUI()
	{
		if (webcamTexture.isPlaying)
		{
			//GUI.Box (new Rect (200, 10, 100, 170), "Menu");
			if (GUILayout.Button("Pause"))
			{
				webcamTexture.Pause();
			}
			if (GUILayout.Button("Stop"))
			{
				webcamTexture.Stop();
			}
		}
		else
		{
			if (GUILayout.Button("Play"))
			{
				webcamTexture.Play();
			}
		}
	} */
}