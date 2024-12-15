using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGameObjects : MonoBehaviour
{
   public GameObject[] gameObjectToEnable;

   private void OnTriggerEnter(Collider other) {
    

    // Check if the object entering the trigger is tagged as "Player" or any specific tag
        if (other.CompareTag("Player"))
        {
            // Loop through each object and disable it
            foreach (GameObject obj in gameObjectToEnable)
            {
                obj.SetActive(true);
            }
        }
   }
}
