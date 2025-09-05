using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billBoardEffect : MonoBehaviour
{

    public bool isFreezedXZ = true;
    void Update()
    {
        if (isFreezedXZ == true)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }   
    }
}
