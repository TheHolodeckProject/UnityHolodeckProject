using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; 

/// <summary>
/// This class provides main interface to the Ovrvision
/// </summary>
public class Ovrvision : MonoBehaviour
{
	//Ovrvision Dll import
	//ovrvision_csharp.cpp
	//Main system
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovOpen(int locationID);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovClose();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovGetCamImage(System.IntPtr img, int eye);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovGetCamImageForUnityColor32(System.IntPtr pImagePtr_Left, System.IntPtr pImagePtr_Right, System.IntPtr pImagePtr_LeftUndistort, System.IntPtr pImagePtr_RightUndistort);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetPixelSize();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetBufferSize();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetImageWidth();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetImageHeight();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetImageRate();

	//Set camera propartys
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetExposure(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetWhiteBalance(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetContrast(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetSaturation(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetBrightness(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetSharpness(int value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetGamma(int value);
	//Get camera propartys
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetExposure();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetWhiteBalance();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetContrast();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetSaturation();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetBrightness();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetSharpness();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovGetGamma();

	//Ovrvision config ipd
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetIPDHorizontal(double value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovSetIPDVertical(double value);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern double ovGetIPDHorizontal();
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern double ovGetIPDVertical();
	//Ovrvision config read write
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int SetParamXMLfromFile(byte[] filename);
	[DllImport("ovrvision", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int SaveParamXMLtoFile(byte[] filename);
	
	//camera select define
	private const int OV_CAMEYE_LEFT = 0;
	private const int OV_CAMEYE_RIGHT = 1;
	private const int OV_SET_AUTOMODE = (-1);

	//Camera GameObject
	private Camera go_cameraLeft;
	private Camera go_cameraRight;
	private GameObject go_cameraPlaneLeft;
	private GameObject go_cameraPlaneRight;
	//Camera texture
	private Texture2D go_CamTexLeft;
	private Texture2D go_CamTexRight;
	private Color32[] go_pixelsColorLeft;
	private Color32[] go_pixelsColorRight;
	private GCHandle go_pixelsHandleLeft;
	private GCHandle go_pixelsHandleRight;
	private System.IntPtr go_pixelsPointerLeft = System.IntPtr.Zero;
	private System.IntPtr go_pixelsPointerRight = System.IntPtr.Zero;
	
	//public setting var
	//Camera status
	public bool camStatus;
    
    //OvrvisionEx = Augmented Reality System
	public OvrvisionEx go_ovrvisionEx;
    public bool useOvrvisionEx;
    //Chroma-key system
    public int camViewShader;
    public Vector2 chroma_hue;         //x=max y=min (0.0f-1.0f)
    public Vector2 chroma_saturation;  //x=max y=min (0.0f-1.0f)
    public Vector2 chroma_brightness;  //x=max y=min (0.0f-1.0f)

	// ------ Function ------

	// Use this for initialization
	void Awake() {
		//Open camera
		if (ovOpen (0) == 0) {
			camStatus = true;
			go_ovrvisionEx = new OvrvisionEx();
		} else {
			camStatus = false;
			Debug.LogError ("Ovrvision open error!!");
		}

	}

	// Use this for initialization
	void Start()
	{
		// initialize camera plane object(Left)
		go_cameraLeft = transform.FindChild ("DeviceCameraLeft").camera;
		go_cameraPlaneLeft = transform.FindChild ("DeviceCameraLeft").FindChild("CameraPlane").gameObject;
		go_cameraPlaneLeft.transform.localPosition = new Vector3 (1.0f, 0.0f, 1.0f);	//Default
		go_cameraPlaneLeft.transform.localScale = new Vector3 (-1.0f, 1.0f, 0.75f);
		// initialize camera plane object(Right)
		go_cameraRight = transform.FindChild ("DeviceCameraRight").camera;
		go_cameraPlaneRight = transform.FindChild ("DeviceCameraRight").FindChild("CameraPlane").gameObject;
		go_cameraPlaneRight.transform.localPosition = new Vector3 (-1.0f, 0.0f, 1.0f);
		go_cameraPlaneRight.transform.localScale = new Vector3 (-1.0f, 1.0f, 0.75f);

		//Setting cameras
		go_cameraLeft.transform.position = Vector3.zero;
		go_cameraLeft.transform.rotation = Quaternion.identity;
		go_cameraLeft.orthographicSize = 7.0f;
		go_cameraRight.transform.position = Vector3.zero;
		go_cameraRight.transform.rotation = Quaternion.identity;
		go_cameraRight.orthographicSize = 7.0f;

		//Create cam texture
		go_CamTexLeft = new Texture2D(ovGetImageWidth(), ovGetImageHeight(), TextureFormat.RGB24, false);
		go_CamTexRight = new Texture2D(ovGetImageWidth(), ovGetImageHeight(), TextureFormat.RGB24, false);
		//Cam setting
		go_CamTexLeft.wrapMode = TextureWrapMode.Clamp;
		go_CamTexRight.wrapMode = TextureWrapMode.Clamp;

        if (camViewShader == 0)
        {   //Normal shader
            go_cameraPlaneLeft.renderer.material.shader = Shader.Find("Ovrvision/ovTexture");
            go_cameraPlaneRight.renderer.material.shader = Shader.Find("Ovrvision/ovTexture");
        }
        else if (camViewShader == 1)
        {   //Chroma-key shader
            go_cameraPlaneLeft.renderer.material.shader = Shader.Find("Ovrvision/ovChromaticMask");
            go_cameraPlaneRight.renderer.material.shader = Shader.Find("Ovrvision/ovChromaticMask");

            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxh", chroma_hue.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_minh", chroma_hue.y);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxs", chroma_saturation.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_mins", chroma_saturation.y);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxv", chroma_brightness.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_minv", chroma_brightness.y);

            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxh", chroma_hue.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_minh", chroma_hue.y);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxs", chroma_saturation.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_mins", chroma_saturation.y);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxv", chroma_brightness.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_minv", chroma_brightness.y);
        }

		if (!camStatus)
			return;

		//Camera open only

		//Get texture pointer
		go_pixelsColorLeft = go_CamTexLeft.GetPixels32();
		go_pixelsColorRight = go_CamTexRight.GetPixels32();
		go_pixelsHandleLeft = GCHandle.Alloc(go_pixelsColorLeft, GCHandleType.Pinned);
		go_pixelsHandleRight = GCHandle.Alloc(go_pixelsColorRight, GCHandleType.Pinned);
		go_pixelsPointerLeft = go_pixelsHandleLeft.AddrOfPinnedObject();
		go_pixelsPointerRight = go_pixelsHandleRight.AddrOfPinnedObject();

		go_cameraPlaneLeft.renderer.material.mainTexture = go_CamTexLeft;
		go_cameraPlaneRight.renderer.material.mainTexture = go_CamTexRight;
	}

	// Update is called once per frame
	void Update ()
	{
		//camStatus
		if (!camStatus)
			return;

		if (go_CamTexLeft == null || go_CamTexRight == null)
			return;



        if (!useOvrvisionEx)
        {
            //Not use the OvrvisionEx.

            //Get the camera image.
            ovGetCamImageForUnityColor32(go_pixelsPointerLeft, go_pixelsPointerRight, System.IntPtr.Zero, System.IntPtr.Zero);
        }
        else
        {
            byte[] undis = new byte[ovGetBufferSize()];
            GCHandle undis_handle = GCHandle.Alloc(undis, GCHandleType.Pinned);

            //Get the camera image.
            ovGetCamImageForUnityColor32(go_pixelsPointerLeft, go_pixelsPointerRight, undis_handle.AddrOfPinnedObject(), System.IntPtr.Zero);
            //ARMarker Renderer
            go_ovrvisionEx.Render (undis_handle.AddrOfPinnedObject());
            undis_handle.Free();
        }

		//Apply
		go_CamTexLeft.SetPixels32(go_pixelsColorLeft);
		go_CamTexLeft.Apply();
		go_CamTexRight.SetPixels32(go_pixelsColorRight);
		go_CamTexRight.Apply();

		//Key Input
		CameraViewKeySetting ();
	}

	//GUI view
	void OnGUI() {

		//Error
		if (!camStatus) {
			GUIStyle guiStyle = new GUIStyle();
			guiStyle.normal.textColor = Color.red;	//error color
			//ovrvision not found.
			GUI.Label (new Rect (20, 20, 300, 40), "[Error] Ovrvision not found.", guiStyle);
		}
	}
	
	//CameraViewKeySetting method
	void CameraViewKeySetting()
	{
		//Camera View Setting
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			go_cameraLeft.orthographicSize -= 0.1f;
			go_cameraRight.orthographicSize -= 0.1f;
		}
		
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			go_cameraLeft.orthographicSize += 0.1f;
			go_cameraRight.orthographicSize += 0.1f;
		}
		
		//Camera View Setting
		if (Input.GetKeyDown (KeyCode.Z)) {
			go_cameraPlaneRight.transform.localPosition += new Vector3(0.0f,0.05f,0.0f);
			go_cameraPlaneLeft.transform.localPosition += new Vector3(0.0f,-0.05f,0.0f);
		}
		
		if (Input.GetKeyDown (KeyCode.X)) {
			go_cameraPlaneRight.transform.localPosition += new Vector3(0.0f,-0.05f,0.0f);
			go_cameraPlaneLeft.transform.localPosition += new Vector3(0.0f,0.05f,0.0f);
		}
		
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			go_cameraPlaneRight.transform.localPosition += new Vector3(0.1f,0.0f,0.0f);
			go_cameraPlaneLeft.transform.localPosition += new Vector3(-0.1f,0.0f,0.0f);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			go_cameraPlaneRight.transform.localPosition += new Vector3(-0.1f,0.0f,0.0f);
			go_cameraPlaneLeft.transform.localPosition += new Vector3(0.1f,0.0f,0.0f);
		}
	}

	// Quit
	void OnApplicationQuit()
	{
		if (!camStatus)
			return;

		//Close camera
		if(ovClose () != 0)
			Debug.LogError ("Ovrvision close error!!");

		//free
		go_pixelsHandleLeft.Free ();
		go_pixelsHandleRight.Free ();
	}

	//Public methods.
	//UpdateOvrvisionSetting method
	public void UpdateOvrvisionSetting(OvrvisionProperty prop)
	{
		if (!camStatus)
			return;

		ovSetExposure (prop.exposure);
		ovSetWhiteBalance (prop.whitebalance);
		ovSetContrast (prop.contrast);
		ovSetSaturation (prop.saturation);
		ovSetBrightness (prop.brightness);
		ovSetSharpness (prop.sharpness);
		ovSetGamma (prop.gamma);

        //change shader
        if (camViewShader == 0)
        {   //Normal shader
            go_cameraPlaneLeft.renderer.material.shader = Shader.Find("Ovrvision/ovTexture");
            go_cameraPlaneRight.renderer.material.shader = Shader.Find("Ovrvision/ovTexture");
        }
        else if (camViewShader == 1)
        {   //Chroma-key shader
            go_cameraPlaneLeft.renderer.material.shader = Shader.Find("Ovrvision/ovChromaticMask");
            go_cameraPlaneRight.renderer.material.shader = Shader.Find("Ovrvision/ovChromaticMask");

            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxh", chroma_hue.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_minh", chroma_hue.y);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxs", chroma_saturation.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_mins", chroma_saturation.y);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_maxv", chroma_brightness.x);
            go_cameraPlaneLeft.renderer.material.SetFloat("_Color_minv", chroma_brightness.y);

            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxh", chroma_hue.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_minh", chroma_hue.y);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxs", chroma_saturation.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_mins", chroma_saturation.y);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_maxv", chroma_brightness.x);
            go_cameraPlaneRight.renderer.material.SetFloat("_Color_minv", chroma_brightness.y);
        }
	}
}
