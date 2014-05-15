using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; 

/// <summary>
/// This class provides main interface to the Ovrvision Ex
/// </summary>
public class OvrvisionEx
{
	//OVRVISION Ex Dll import
	//ovrvision_ex_csharp.cpp
	//Main system
	[DllImport("ovrvision_ex", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovExSetImage(System.IntPtr pImgSrc, int w, int h);
	[DllImport("ovrvision_ex", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern void ovExRender();
	[DllImport("ovrvision_ex", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
	static extern int ovExGetData(System.IntPtr mdata, int datasize);
    [DllImport("ovrvision_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovExSetMarkerSize(int value);
    [DllImport("ovrvision_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovExGetMarkerSize();

    //Macro define
	private const int MARKERGET_MAXNUM10 = 100; 
	private const int MARKERGET_ARG10 = 10;
    private const int MARKERGET_RECONFIGURE_NUM = 10;

	// ------ Function ------
	
	//Renderer
	public int Render(System.IntPtr pImgSrc)
	{
		float[] markerGet = new float[MARKERGET_MAXNUM10];
		GCHandle marker = GCHandle.Alloc(markerGet, GCHandleType.Pinned);

		ovExSetImage (pImgSrc, 640, 480);
		ovExRender ();

		//Get marker data
		int ri = ovExGetData(marker.AddrOfPinnedObject(), MARKERGET_MAXNUM10);

		OvrvisionTracker[] otobjs = GameObject.FindObjectsOfType(typeof(OvrvisionTracker)) as OvrvisionTracker[];
		foreach (OvrvisionTracker otobj in otobjs) {
            otobj.UpdateTransformNone();
			for (int i=0; i < ri; i++) {
				if(otobj.markerID == (int)markerGet[i*MARKERGET_ARG10]) {
					otobj.UpdateTransform(markerGet,i);
					break;
				}
			}
		}

		marker.Free ();

		return ri;
	}

    //IPD Reconfigure system
    public int RenderIPDReconfigure(System.IntPtr pImgSrc1, System.IntPtr pImgSrc2)
    {
        int ri;
        float[] markerGet = new float[MARKERGET_RECONFIGURE_NUM];
        GCHandle marker = GCHandle.Alloc(markerGet, GCHandleType.Pinned);

        ovExSetImage(pImgSrc1, 640, 480);
        ovExRender();

        //Get marker data
        ri = ovExGetData(marker.AddrOfPinnedObject(), MARKERGET_RECONFIGURE_NUM);



        ovExSetImage(pImgSrc2, 640, 480);
        ovExRender();

        //Get marker data
        ri = ovExGetData(marker.AddrOfPinnedObject(), MARKERGET_RECONFIGURE_NUM);

        marker.Free();

        return ri;
    }

}
