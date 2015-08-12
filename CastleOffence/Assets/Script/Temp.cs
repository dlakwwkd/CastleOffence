using UnityEngine;
using System.Collections.Generic;

public class Temp : MonoBehaviour
{
    public GameObject a;
    public GameObject b;

    public List<GameObject> aa;
    public List<GameObject> bb;

    void Update ()
    {
        if (Input.GetKeyDown("1"))
        {
            aa.Add(ObjectManager.instance.Assign(a.name));
        }
        if (Input.GetKeyDown("2"))
        {
            if(aa.Count > 0)
            {
                ObjectManager.instance.Free(aa[aa.Count - 1]);
                aa.RemoveAt(aa.Count - 1);
            }
        }
        if (Input.GetKeyDown("3"))
        {
            bb.Add(ObjectManager.instance.Assign(b.name));
            bb.Add(ObjectManager.instance.Assign(b.name));
            bb.Add(ObjectManager.instance.Assign(b.name));
        }
        if (Input.GetKeyDown("4"))
        {
            foreach(var asd in bb)
            {
                ObjectManager.instance.Free(asd);
            }
            bb.Clear();
        }
    }
}
