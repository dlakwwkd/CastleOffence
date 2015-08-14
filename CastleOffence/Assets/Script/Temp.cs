using UnityEngine;
using System.Collections.Generic;

public class Temp : MonoBehaviour
{
    void Update ()
    {
        if (Input.GetKeyDown("1"))
        {
            ObjectManager.instance.FreeAll();
        }
    }
}
