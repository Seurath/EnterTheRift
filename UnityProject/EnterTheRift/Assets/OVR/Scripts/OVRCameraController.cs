﻿/************************************************************************************

Filename    :   OVRCameraController.cs
Content     :   Camera controller interface. 
				This script is used to interface the OVR cameras.
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------
// ***** OVRCameraController
//
// OVRCameraController is a component that allows for easy handling of the lower level cameras.
// It is the main interface between Unity and the cameras. 
// This is attached to a prefab that makes it easy to add a Rift into a scene.
//
// All camera control should be done through this component.
//
public class OVRCameraController : OVRComponent
{	
	// PRIVATE MEMBERS
	private bool   UpdateCamerasDirtyFlag = false;	
	private Camera CameraLeft, CameraRight = null;
	private float  IPD = 0.064f; 							// in millimeters
	private float  LensOffsetLeft, LensOffsetRight = 0.0f;  // normalized screen space
	private float  VerticalFOV = 90.0f;	 					// in degrees
	private float  AspectRatio = 1.0f;						
	private float  DistK0, DistK1, DistK2, DistK3 = 0.0f; 	// lens distortion parameters
	
	// Initial orientation of the camera, can be used to always set the zero orientation of
	// the cameras to follow a set forward facing orientation.
	private Quaternion OrientationOffset = Quaternion.identity;	
	// Set Y rotation here; this will offset the y rotation of the cameras. 
	private float   YRotation = 0.0f;
	
	// Shared orientation, shared by cameras
	private Quaternion SharedOrientation = Quaternion.identity;
	
	// PUBLIC MEMBERS
	// Camera positioning:
	// From root of camera to neck (translation only)
	public Vector3 		NeckPosition      = new Vector3(0.0f, 0.7f,  0.0f);	
	// From neck to eye (rotation and translation; x will be different for each eye)
	public Vector3 		EyeCenterPosition = new Vector3(0.0f, 0.15f, 0.09f);
	
	// Set this transform with an objec that the camera orientation should follow.
	// NOTE: Best not to set this with the OVRCameraController IF TrackerRotatesY is
	// on, since this will lead to uncertain output
	public Transform 	FollowOrientation = null;
	
	// Set to true if we want the rotation of the camera controller to be influenced by tracker
	public bool  		TrackerRotatesY	= false;
	// Use this to turn on/off Prediction
	public bool			PredictionOn 	= true;
	// Use this to decide where tracker sampling should take place
	// Setting to true allows for better latency, but some systems
	// (such as Pro water) will break
	public bool			CallInPreRender = false;
	// Use this to turn on wire-mode
	public bool			WireMode  		= false;

	// Turn lens distortion on/off; use Chromatic Aberration in lens distortion calculation
	public bool 		LensCorrection  = true;
	public bool 		Chromatic		= true;


	// UNITY CAMERA FIELDS
	// Set the background color for both cameras
	public Color 		BackgroundColor = new Color(0.192f, 0.302f, 0.475f, 1.0f);
	// Set the near and far clip plane for both cameras
	public float 		NearClipPlane   = 0.15f;
	public float 		FarClipPlane    = 1000.0f;
	
	// * * * * * * * * * * * * *
		
	// Awake
	new void Awake()
	{
		base.Awake();
	}

	// Start
	new void Start()
	{
		base.Start();
		
		// Get the cameras
		Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
		
		for (int i = 0; i < cameras.Length; i++)
		{
			if(cameras[i].name == "CameraLeft")
				CameraLeft = cameras[i];
			
			if(cameras[i].name == "CameraRight")
				CameraRight = cameras[i];
		}
		
		if((CameraLeft == null) || (CameraRight == null))
			Debug.LogWarning("WARNING: Unity Cameras in OVRCameraController not found!");
		
		// Get the required Rift infromation needed to set cameras
		InitCameraControllerVariables();
		
		// Initialize the cameras
		UpdateCamerasDirtyFlag = true;
		UpdateCameras();
		
		SetMaximumVisualQuality();
		
	}
		
	// Update 
	new void LateUpdate()
	{
		base.Update();		
		UpdateCameras();
	}
		
	// InitCameraControllerVariables
	// Made public so that it can be called by classes that require information about the
	// camera to be present when initing variables in 'Start'
	public void InitCameraControllerVariables()
	{
		// Get the IPD value (distance between eyes in meters)
		OVRDevice.GetIPD(ref IPD);

		// Get the values for both IPD and lens distortion correction shift. We don't normally
		// need to set the PhysicalLensOffset once it's been set here.
		OVRDevice.CalculatePhysicalLensOffsets(ref LensOffsetLeft, ref LensOffsetRight);
		
		// Using the calculated FOV, based on distortion parameters, yeilds the best results.
		// However, public functions will allow to override the FOV if desired
		VerticalFOV = OVRDevice.VerticalFOV();
		
		// Store aspect ratio as well
		AspectRatio = OVRDevice.CalculateAspectRatio();
		
		OVRDevice.GetDistortionCorrectionCoefficients(ref DistK0, ref DistK1, ref DistK2, ref DistK3);
				
		// Get our initial world orientation of the cameras from the scene (we can grab it from 
		// the set FollowOrientation object or this OVRCameraController gameObject)
		if(FollowOrientation != null)
			OrientationOffset = FollowOrientation.rotation;
		else
			OrientationOffset = transform.rotation;
	}
	
	// InitCameras
	void UpdateCameras()
	{
		// Values that influence the stereo camera orientation up and above the tracker
		if(FollowOrientation != null)
			OrientationOffset = FollowOrientation.rotation;
				
		if(UpdateCamerasDirtyFlag == false)
			return;
		
		float distOffset = 0.5f + (LensOffsetLeft * 0.5f);
		float perspOffset = LensOffsetLeft;
		float eyePositionOffset = -IPD * 0.5f;
		ConfigureCamera(ref CameraLeft, distOffset, perspOffset, eyePositionOffset);
		
		distOffset = 0.5f + (LensOffsetRight * 0.5f);
		perspOffset = LensOffsetRight;
		eyePositionOffset = IPD * 0.5f;
		ConfigureCamera(ref CameraRight, distOffset, perspOffset, eyePositionOffset);
		
		UpdateCamerasDirtyFlag = false;
	}
	
	// SetCamera
	bool ConfigureCamera(ref Camera camera, float distOffset, float perspOffset, float eyePositionOffset)
	{
		Vector3 PerspOffset = Vector3.zero;
		Vector3 EyePosition = EyeCenterPosition;
				
		// Vertical FOV
		camera.fov = VerticalFOV;
			
		// Aspect ratio 
		camera.aspect = AspectRatio;
			
		// Centre of lens correction
		camera.GetComponent<OVRLensCorrection>()._Center.x = distOffset;
		ConfigureCameraLensCorrection(ref camera);

		// Perspective offset for image
		PerspOffset.x = perspOffset;
		camera.GetComponent<OVRCamera>().SetPerspectiveOffset(ref PerspOffset);
			
		// Set camera variables that pertain to the neck and eye position
		// NOTE: We will want to add a scale vlue here in the event that the player 
		// grows or shrinks in the world. This keeps head modelling behaviour
		// accurate
		camera.GetComponent<OVRCamera>().NeckPosition = NeckPosition;
		EyePosition.x = eyePositionOffset; 
			
		camera.GetComponent<OVRCamera>().EyePosition = EyePosition;		
					
		// Background color
		camera.backgroundColor = BackgroundColor;
		
		// Clip Planes
		camera.nearClipPlane = NearClipPlane;
		camera.farClipPlane = FarClipPlane;
			
		return true;
	}
	
	// SetCameraLensCorrection
	void ConfigureCameraLensCorrection(ref Camera camera)
	{
		// Get the distortion scale and aspect ratio to use when calculating distortion shader
		float distortionScale = 1.0f / OVRDevice.DistortionScale();
		float aspectRatio     = OVRDevice.CalculateAspectRatio();
		
		// These values are different in the SDK World Demo; Unity renders each camera to a buffer
		// that is normalized, so we will respect this rule when calculating the distortion inputs
		float NormalizedWidth  = 1.0f;
		float NormalizedHeight = 1.0f;
		
		OVRLensCorrection lc = camera.GetComponent<OVRLensCorrection>();
		
		lc._Scale.x     = (NormalizedWidth  / 2.0f) * distortionScale;
		lc._Scale.y     = (NormalizedHeight / 2.0f) * distortionScale * aspectRatio;
		lc._ScaleIn.x   = (2.0f / NormalizedWidth);
		lc._ScaleIn.y   = (2.0f / NormalizedHeight) / aspectRatio;
		lc._HmdWarpParam.x = DistK0;		
		lc._HmdWarpParam.y = DistK1;
		lc._HmdWarpParam.z = DistK2;
	}
	
	///////////////////////////////////////////////////////////
	// PUBLIC FUNCTIONS
	///////////////////////////////////////////////////////////
	
	// SetCameras - Should we want to re-target the cameras
	public void SetCameras(ref Camera cameraLeft, ref Camera cameraRight)
	{
		CameraLeft = cameraLeft;
		CameraRight = cameraRight;
		UpdateCamerasDirtyFlag = true;
	}
	
	// Get/SetIPD 
	public void GetIPD(ref float ipd)
	{
		ipd = IPD;
	}
	public void SetIPD(float ipd)
	{
		IPD = ipd;
		UpdateCamerasDirtyFlag = true;
		
	}
			
	//Get/SetVerticalFOV
	public void GetVerticalFOV(ref float verticalFOV)
	{
		verticalFOV = VerticalFOV;
	}
	public void SetVerticalFOV(float verticalFOV)
	{
		VerticalFOV = verticalFOV;
		UpdateCamerasDirtyFlag = true;
	}
	
	//Get/SetAspectRatio
	public void GetAspectRatio(ref float aspecRatio)
	{
		aspecRatio = AspectRatio;
	}
	public void SetAspectRatio(float aspectRatio)
	{
		AspectRatio = aspectRatio;
		UpdateCamerasDirtyFlag = true;
	}
	
	// Get/SetDistortionCoefs
	public void GetDistortionCoefs(ref float distK0, 
								   ref float distK1, 
								   ref float distK2, 
		                           ref float distK3)
	{
		distK0 = DistK0;
		distK1 = DistK1;
		distK2 = DistK2;
		distK3 = DistK3;
	}
	public void SetDistortionCoefs(float distK0, 
								   float distK1, 
								   float distK2, 
								   float distK3)
	{
		DistK0 = distK0;
		DistK1 = distK1;
		DistK2 = distK2;
		DistK3 = distK3;
		UpdateCamerasDirtyFlag = true;
	}
	
	// Get/SetNeckPosition
	public void GetNeckPosition(ref Vector3 neckPosition)
	{
		neckPosition = NeckPosition;
	}
	public void SetNeckPosition(Vector3 neckPosition)
	{
		NeckPosition = neckPosition;
		UpdateCamerasDirtyFlag = true;
	}
	
	// Get/SetEyeCenterPosition
	public void GetEyeCenterPosition(ref Vector3 eyeCenterPosition)
	{
		eyeCenterPosition = EyeCenterPosition;
	}
	public void SetEyeCenterPosition(Vector3 eyeCenterPosition)
	{
		EyeCenterPosition = eyeCenterPosition;
		UpdateCamerasDirtyFlag = true;
	}
	
	// Get/SetOrientationOffset
	public void GetOrientationOffset(ref Quaternion orientationOffset)
	{
		orientationOffset = OrientationOffset;
	}
	public void SetOrientationOffset(Quaternion orientationOffset)
	{
		OrientationOffset = orientationOffset;
	}
	
	// Get/SetYRotation
	public void GetYRotation(ref float yRotation)
	{
		yRotation = YRotation;
	}
	public void SetYRotation(float yRotation)
	{
		YRotation = yRotation;
	}
	
	// Get/SetSharedOrientation
	public void GetSharedOrientation(ref Quaternion sharedOrientation)
	{
		sharedOrientation = SharedOrientation;
	}
	public void SetSharedOrientation(Quaternion sharedOrientation)
	{
		SharedOrientation = sharedOrientation;
	}
	
	// Get/SetTrackerRotatesY
	public void GetTrackerRotatesY(ref bool trackerRotatesY)
	{
		trackerRotatesY = TrackerRotatesY;
	}
	public void SetTrackerRotatesY(bool trackerRotatesY)
	{
		TrackerRotatesY = trackerRotatesY;
	}
	
	// GetCameraOrientationEulerAngles
	public bool GetCameraOrientationEulerAngles(ref Vector3 angles)
	{
		if(CameraLeft == null)
			return false;
		
		angles = CameraLeft.transform.rotation.eulerAngles;
		return true;
	}
	
	// GetCameraOrientation
	public bool GetCameraOrientation(ref Quaternion quaternion)
	{
		if(CameraLeft == null)
			return false;
		
		quaternion = CameraLeft.transform.rotation;
		return true;
	}
	
	// SetMaximumVisualQuality
	public void SetMaximumVisualQuality()
	{
		QualitySettings.softVegetation  = 		true;
		QualitySettings.maxQueuedFrames = 		0;
		QualitySettings.anisotropicFiltering = 	AnisotropicFiltering.ForceEnable;
		QualitySettings.vSyncCount = 			1;
	}
	
}

