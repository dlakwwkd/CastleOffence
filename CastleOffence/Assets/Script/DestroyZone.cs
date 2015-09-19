using UnityEngine;
using System.Collections;

public class DestroyZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject.GetComponent<ObjectStatus>();
        if (obj)
            obj.Death();
    }
}
