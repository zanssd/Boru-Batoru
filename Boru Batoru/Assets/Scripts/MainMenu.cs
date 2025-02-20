using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject arButton;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ARCheck());
    }

    IEnumerator ARCheck()
    {
        if ((ARSession.state == ARSessionState.None) ||
           (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            arButton.SetActive(false);
        }
        else
        {
            // Start the AR session
        }
    }
    
    public void MoveScene(string scene)
    {
        ScenesManager.instance.MoveScene(scene);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
