using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Video;
public class CloudHandler : MonoBehaviour
{
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;
    private string mTargetMetadata = "";
    //public GameObject MainPlayer;
    public ImageTargetBehaviour ImageTargetTemplate;

    public void OnInitError(TargetFinder.InitState initError)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {     

        GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
        GameObject augmentation = null;

        if (augmentation != null)
        {
          augmentation.transform.parent = newImageTarget.transform;            
        }

        
        Debug.Log("Player encontrado");
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult =
        (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        // do something with the target metadata
        mTargetMetadata = cloudRecoSearchResult.MetaData;
        Debug.Log("URL: " + mTargetMetadata);

        // Build augmentation based on target         
        if (ImageTargetTemplate)
        {
            // enable the new result with the same ImageTargetBehaviour: 
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, newImageTarget);
            //tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject);
            GameObject MainPlayer = newImageTarget.gameObject.transform.Find("Player").gameObject;
            MainPlayer.GetComponent<VideoPlayer>().url = mTargetMetadata.Trim(); 
            MainPlayer.GetComponent<VideoPlayer>().Play();            
        }



        if (!mIsScanning)
        {
            // stop the target finder
            mCloudRecoBehaviour.CloudRecoEnabled = true;
           // MainPlayer.GetComponent<VideoPlayer>().Stop();
            
        }
    }

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

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }


    //void OnGUI()
    //{
        // Display current 'scanning' status
      //  GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        //GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
        // If not scanning, show button
        // so that user can restart cloud scanning
        //if (!mIsScanning)
        //{
        //    if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
        //    {
                // Restart TargetFinder
              //  mCloudRecoBehaviour.CloudRecoEnabled = true;
        //    }
        //}
    //}
}

