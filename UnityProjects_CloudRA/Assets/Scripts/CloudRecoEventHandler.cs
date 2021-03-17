using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour
{
	#region PRIVATE_MEMBER_VARIABLES

	// CloudRecoBehaviour reference to avoid lookups
	private CloudRecoBehaviour mCloudRecoBehaviour;
	// ImageTracker reference to avoid lookups
	private ObjectTracker mImageTracker;

	private bool mIsScanning = false;

	private string mTargetMetadata = "";

	#endregion // PRIVATE_MEMBER_VARIABLES



	#region EXPOSED_PUBLIC_VARIABLES

	/// <summary>
	/// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
	/// </summary>
	public ImageTargetBehaviour ImageTargetTemplate;

	#endregion

	#region UNTIY_MONOBEHAVIOUR_METHODS

	/// <summary>
	/// register for events at the CloudRecoBehaviour
	/// </summary>

	void Awake()
	{
		// register this event handler at the cloud reco behaviour
		CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		
			cloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
			cloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
			cloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
			cloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
			cloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
			cloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
		

		// remember cloudRecoBehaviour for later
		mCloudRecoBehaviour = cloudRecoBehaviour;	

	}
	
	//Unregister cloud reco callbacks when the handler is destroyed
	void OnDestroy()
	{
		mCloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
		mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
		mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
		mCloudRecoBehaviour.UnregisterOnStateChangedEventHandler(OnStateChanged);
		mCloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
	}


	#endregion // UNTIY_MONOBEHAVIOUR_METHODS


	#region ICloudRecoEventHandler_IMPLEMENTATION

	/// <summary>
	/// called when TargetFinder has been initialized successfully
	/// </summary>
	public void OnInitialized(TargetFinder targetFinder)
	{
		// get a reference to the Image Tracker, remember it
		mImageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
		Debug.Log("Cloud Reco initialized");
	}

	/// <summary>
	/// visualize initialization errors
	/// </summary>
	public void OnInitError(TargetFinder.InitState initError)
	{
	}

	/// <summary>
	/// visualize update errors
	/// </summary>
	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
	}

	/// <summary>
	/// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
	/// </summary>
	public void OnStateChanged(bool scanning)
	{
		mIsScanning = scanning;
		if (scanning)
		{
			// clear all known trackables
			var tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.GetTargetFinder<ImageTargetFinder>().ClearTrackables(false);
		}
	}

	/// <summary>
	/// Handles new search results
	/// </summary>
	/// <param name="targetSearchResult"></param>
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
	{
		// duplicate the referenced image target
		GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;

		GameObject augmentation = null;

		TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
		string model_name = cloudRecoSearchResult.MetaData;


		if (augmentation != null)
        {
			augmentation.transform.parent = newImageTarget.transform;
			Debug.Log("Cloud Reco initialized");
		}


		// enable the new result with the same ImageTargetBehaviour:
		//ImageTargetAbstractBehaviour imageTargetBehaviour = mImageTracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, newImageTarget);
		ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, newImageTarget);

		//tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject);

		Debug.Log("Metadata value is " + model_name);
		mTargetMetadata = model_name;


		switch (model_name)
		{

			case "cube":
				Destroy(newImageTarget.gameObject.transform.Find("Sphere").gameObject);
				
				break;

			case "sphere":

				Destroy(newImageTarget.gameObject.transform.Find("Cube").gameObject);

				break;

		}



		if (!mIsScanning)
		{
			// stop the target finder
			mCloudRecoBehaviour.CloudRecoEnabled = true;
		}
	}


	#endregion // ICloudRecoEventHandler_IMPLEMENTATION

	void OnGUI()
	{
		GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
	}


}