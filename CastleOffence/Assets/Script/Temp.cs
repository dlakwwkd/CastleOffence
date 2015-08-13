using UnityEngine;
using System.Collections.Generic;

public class Temp : MonoBehaviour
{
    public GameObject a;
    public GameObject b;
    public GameObject c;

    public List<GameObject> aa;
    public List<GameObject> bb;
    public List<GameObject> cc;

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
        if (Input.GetKeyDown("5"))
        {
            foreach (var asd in cc)
            {
                ObjectManager.instance.Free(asd);
            }
            cc.Clear();
        }

        if (Input.touchCount == 0)
            return;
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    cc.Add(ObjectManager.instance.Assign(c.name));
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }
    }
}
