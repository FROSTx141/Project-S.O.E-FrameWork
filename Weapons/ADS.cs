using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class ADS : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    
    [SerializeField] private GameObject Gun;
    [SerializeField] private GameObject CrosshairCanvas;
    

    private bool ToggleADS;
    

    // Update is called once per frame
    void Update()
    {
        inputManager.onFoot.ADSPressed.performed += ctx => ADSToggle();
        
        
        
    }

    void ADSToggle()
    {
        if(ToggleADS = !ToggleADS)
        {
            CrosshairCanvas.SetActive(false);
            Gun.GetComponent<Animator>().SetBool("ADSToggle", ToggleADS);
        }
        else
        {
            CrosshairCanvas.SetActive(true);
            Gun.GetComponent<Animator>().SetBool("ADSToggle", ToggleADS);
        }
        
    }

    
}
