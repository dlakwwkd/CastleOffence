using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
    //-----------------------------------------------------------------------------------
    // public functions
    public void Init(GameObject obj, int size)
    {
        objectType = obj;
        allocInterval = size;
        Alloc();
    }

    public GameObject Assign()
    {
        if (freeList.Count < 1)
            Alloc();

        int idx = freeList.Pop();
        var obj = objectList[idx];
        obj.SetActive(true);
        return obj;
    }

    public void Free(GameObject obj)
    {
        obj.SetActive(false);

        int idx = obj.name.IndexOf('_');
        var key = obj.name.Substring(idx + 1);
        freeList.Push(int.Parse(key));
    }

    public void FreeAll()
    {
        for (int i = 0; i < objectList.Count; ++i)
        {
            var obj = objectList[i];
            if (!obj.activeSelf) continue;
            Free(obj);
        }
    }

    //-----------------------------------------------------------------------------------
    // private functions
    void Alloc()
    {
        for (int i = 0; i < allocInterval; ++i)
        {
            var obj = GameObject.Instantiate(objectType) as GameObject;
            obj.name = objectType.name + "_" + objectList.Count;
            obj.SetActive(false);

            freeList.Push(objectList.Count);
            objectList.Add(obj);
        }
    }

    //-----------------------------------------------------------------------------------
    // private field
    GameObject          objectType      = null;
    List<GameObject>    objectList      = new List<GameObject>();
    Stack<int>          freeList        = new Stack<int>();
    int                 allocInterval   = 0;
}

public class ObjectManager : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // property
    public static ObjectManager instance { get; private set; }

    //-----------------------------------------------------------------------------------
    // inspector field
    [FormerlySerializedAs("objectList")]
    public List<GameObject>     ObjectList  = new List<GameObject>();
    [FormerlySerializedAs("sizeList")]
    public List<int>            SizeList    = new List<int>();

    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        if (instance == null)
            instance = this;

        if (ObjectList.Count != SizeList.Count)
            Debug.LogError("ObjectPool size is invalied!");

        for (int i = 0; i < ObjectList.Count; ++i)
        {
            var pool = new ObjectPool();
            pool.Init(ObjectList[i], SizeList[i]);
            poolList.Add(ObjectList[i].name, pool);
        }
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public GameObject Assign(string objName)
    {
        return poolList[objName].Assign();
    }

    public void Free(GameObject obj)
    {
        if (!obj.activeSelf) return;

        int idx = obj.name.IndexOf('_');
        var key = obj.name.Remove(idx);
        poolList[key].Free(obj);
    }

    public void FreeAll()
    {
        foreach (var pool in poolList)
            pool.Value.FreeAll();
    }

    public void FreeAfter(GameObject obj, float after)
    {
        StartCoroutine(After(obj, after));
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator After(GameObject obj, float after)
    {
        yield return new WaitForSeconds(after);
        Free(obj);
    }
    
    //-----------------------------------------------------------------------------------
    // private field
    Dictionary<string, ObjectPool> poolList = new Dictionary<string, ObjectPool>();
}
