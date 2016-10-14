using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // handler functions
    void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject.GetComponent<ObjectStatus>();
        if (obj)
            obj.Death();
    }
}
