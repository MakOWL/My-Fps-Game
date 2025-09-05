using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightEffects : MonoBehaviour
{
    public bool isShadow = false;

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Trigger with: " + other.name);
        if (other.CompareTag("Player"))
        {
            isShadow = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        Debug.Log("Exited Trigger with: " + other.name);
        if (other.CompareTag("Player"))
        {
            isShadow = false;
        }
    }
        
    

 
}
