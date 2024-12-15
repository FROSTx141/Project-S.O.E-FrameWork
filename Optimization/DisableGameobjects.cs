using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameobjects : MonoBehaviour
{
    // Array to store the objects to disable
    public GameObject[] objectsToDisable;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is tagged as "Player" or any specific tag
        if (other.CompareTag("Player"))
        {
            // Loop through each object and disable it
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        }
    }
}