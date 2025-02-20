using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARManager : MonoBehaviour
{
    public GameObject objectToPlace;
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private ARPlaneManager arPlaneManager;
    private bool isPlaced = false;

    [SerializeField] private GameObject objectToPlace2; 
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager arAnchorManager;


    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private bool isARActive = true;
    [SerializeField] 
    private ARCameraManager arCameraManager; 
    [SerializeField] 
    private GameObject arBackground; 

    //private void Awake()
    //{
    //    arPlaneManager.planesChanged += PlaneChanged;
    //}
    // void PlaneChanged(ARPlanesChangedEventArgs args)
    //{
    //    if (args.added != null && !isPlaced)
    //    {
    //        isPlaced = true;
    //        ARPlane arPlane = args.added[0];
    //        //arPlane.transform.position = new Vector3(arPlane.transform.position.x, arPlane.transform.position.y - 20, arPlane.transform.position.z);
    //        GameObject placeObj = Instantiate(objectToPlace, arPlane.transform.position, objectToPlace.transform.rotation);
    //        GameManager.Instance.uiManager.logTxt.text = "Location " + placeObj.transform.position + " Rotation " + placeObj.transform.rotation;
    //        StartCoroutine(GameManager.Instance.uiManager.CountdownRoutine());
    //        DisableARPlanes();
    //    }
    //}

    public void ToggleARCamera()
    {
        isARActive = !isARActive;
        arCameraManager.enabled = isARActive;

        //if (isARActive)
        //{
        //    arBackground.SetActive(false); 
        //}
        //else
        //{
        //    arBackground.SetActive(true);
        //}
    }
    void Update()
    {
    
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    ARPlane plane = arPlaneManager.GetPlane(hits[0].trackableId);
                    if (!isPlaced)
                    {
                        ARAnchor anchor = plane.gameObject.AddComponent<ARAnchor>();

                        isPlaced = true;
                        GameObject placeObj = Instantiate(objectToPlace, hitPose.position, objectToPlace.transform.rotation);

                        StartCoroutine(GameManager.Instance.uiManager.CountdownRoutine());
                        DisableARPlanes();
                    }

                }
            }
        }

    }
    void DisableARPlanes()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        arPlaneManager.enabled = false;
    }
}
