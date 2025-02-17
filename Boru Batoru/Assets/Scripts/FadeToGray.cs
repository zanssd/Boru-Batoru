using System.Collections;
using UnityEngine;

public class FadeToGray : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private Material grayScale;

    [SerializeField] private Soldier soldier;

    [SerializeField] private Material[] originMat;


    private void Start()
    {
        GetOriginMat();
    }

    void GetOriginMat()
    {
        originMat = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originMat[i] = renderers[i].material; 
        }
    }

    public void ChangeToGray()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = grayScale;
        }
    }
    public void RestoreOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && i < originMat.Length)
            {
                renderers[i].material = originMat[i]; 
            }
        }
    }



}
