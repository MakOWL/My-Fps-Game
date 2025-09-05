using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowZoneDectector : MonoBehaviour
{
    public bool isShadow = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShadowZone"))
        {
           // Debug.Log("Entered Shadow Zone");
            isShadow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShadowZone"))
        {
           // Debug.Log("Exited Shadow Zone");
            isShadow = false;
        }
    }
}
