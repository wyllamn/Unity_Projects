using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class SimpleCloudHandler : MonoBehaviour
{
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;
    private string mTargetMetadata = "";
    public ImageTargetBehaviour ImageTargetTemplate;
    private ObjectTracker mImageTracker;
    //public GameObject mainPlayer;

    // Register cloud reco callbacks
    void Awake()
    {             
            mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
            mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
            mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
            mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
            mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
            mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);           

    }
    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized(TargetFinder targetFinder)
    {        
        Debug.Log("Cloud Reco initialized");
    }
    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }
    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetBehaviour, 
    /// then delete all trackables
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
        //GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
        //GameObject augmentation = null;
        //if (augmentation != null)
        //{
         //   augmentation.transform.SetParent(newImageTarget.transform);
        //}

        // do something with the target metadata
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        string model_name = cloudRecoSearchResult.MetaData;

        //mCloudRecoBehaviour.CloudRecoEnabled = false;
        // Build augmentation based on target 
        if (ImageTargetTemplate)
        {
            // stop the target finder (i.e. stop scanning the cloud)
          
            // enable the new result with the same ImageTargetBehaviour: 
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject);
            Debug.Log("Metadata value is " + model_name);
            mTargetMetadata = model_name;
            switch (mTargetMetadata)
            {

                case "sphere":

                    //Destroy(ImageTargetTemplate.gameObject.transform.Find("Cube").gameObject);
                    ImageTargetTemplate.gameObject.transform.Find("Cube").gameObject.SetActive(false);
                    ImageTargetTemplate.gameObject.transform.Find("Sphere").gameObject.SetActive(true);

                    break;

                case "cube":

                    //Destroy(ImageTargetTemplate.gameObject.transform.Find("Sphere").gameObject);
                    ImageTargetTemplate.gameObject.transform.Find("Cube").gameObject.SetActive(true);
                    ImageTargetTemplate.gameObject.transform.Find("Sphere").gameObject.SetActive(false);

                    break;
            }

            //mCloudRecoBehaviour.CloudRecoEnabled = true;            
        }


        //mCloudRecoBehaviour.CloudRecoEnabled = true;
        //if (mCloudRecoBehaviour.CloudRecoInitialized && !mCloudRecoBehaviour.CloudRecoEnabled)
        //{
          //  mCloudRecoBehaviour.CloudRecoEnabled = true;
        //}

    }

    void CloudRecoRestart()
    {
        if (mCloudRecoBehaviour)
        {
            if (!mCloudRecoBehaviour.CloudRecoEnabled)
                mCloudRecoBehaviour.CloudRecoEnabled = true;
        }
    }


        void SetCloudActivityIconVisible(bool visible)
        {
            /*
            if (!m_CloudActivityIcon) return;

            m_CloudActivityIcon.enabled = visible; */
        }
    

    void OnGUI()
        {
            GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
        }
    
}
