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

 

    void Update()
    {
        if (!isPlaced && arPlaneManager.trackables.count > 0)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    PlaceObject(plane);
                    isPlaced = true;
                    arPlaneManager.enabled = false;
                    break;
                }
            }
        }
    }

    void PlaceObject(ARPlane plane)
    {
        objectToPlace.transform.position = plane.center;
        objectToPlace.SetActive(true);
    }
}
