using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices; 

public class OvrvisionProperty {

	//OVRVISION Dll import
	//ovrvision_csharp.cpp
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

	//properties
	public int exposure;
	public int whitebalance;
	public int contrast;
	public int saturation;
	public int brightness;
	public int sharpness;
	public int gamma;

	//initialize value
	public OvrvisionProperty()
	{
		exposure = ovGetExposure();
		whitebalance = ovGetWhiteBalance();
		contrast = ovGetContrast();
		saturation = ovGetSaturation();
		brightness = ovGetBrightness();
		sharpness = ovGetSharpness();
		gamma = ovGetGamma();
	}
}
