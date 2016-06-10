using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
    GameObject          _objectType     = null;
    List<GameObject>    _objectList     = new List<GameObject>();
    Stack<int>          _freeList       = new Stack<int>();
    int                 _allocInterval  = 0;


    public void Init(GameObject obj, int size)
    {
        _objectType = obj;
        _allocInterval = size;
        Alloc();
    }

    public GameObject Assign()
    {
        if (_freeList.Count < 1)
            Alloc();

        int idx = _freeList.Pop();
        var obj = _objectList[idx];
        obj.SetActive(true);
        return obj;
    }

    public void Free(GameObject obj)
    {
        obj.SetActive(false);

        int idx = obj.name.IndexOf('_');
        var key = obj.name.Substring(idx + 1);
        _freeList.Push(int.Parse(key));
    }

    public void FreeAll()
    {
        for (int i = 0; i < _objectList.Count; ++i)
        {
            var obj = _objectList[i];
            if (!obj.activeSelf) continue;
            Free(obj);
        }
    }



    void Alloc()
    {
        for (int i = 0; i < _allocInterval; ++i)
        {
            var obj = GameObject.Instantiate(_objectType) as GameObject;
            obj.name = _objectType.name + "_" + _objectList.Count;
            obj.SetActive(false);

            _freeList.Push(_objectList.Count);
            _objectList.Add(obj);
        }
    }
}



public class ObjectManager : MonoBehaviour
{
    public static ObjectManager     instance { get; private set; }

    // inspector
    public List<GameObject>         objectList  = new List<GameObject>();
    public List<int>                sizeList    = new List<int>();

    // private
    Dictionary<string, ObjectPool>  _poolList   = new Dictionary<string, ObjectPool>();


    void Start()
    {
        if (instance == null)
            instance = this;

        if (objectList.Count != sizeList.Count)
            Debug.LogError("ObjectPool size is invalied!");

        for (int i = 0; i < objectList.Count; ++i)
        {
            var pool = new ObjectPool();
            pool.Init(objectList[i], sizeList[i]);
            _poolList.Add(objectList[i].name, pool);
        }
    }



    public GameObject Assign(string objName)
    {
        return _poolList[objName].Assign();
    }

    public void Free(GameObject obj)
    {
        if (!obj.activeSelf) return;

        int idx = obj.name.IndexOf('_');
        var key = obj.name.Remove(idx);
        _poolList[key].Free(obj);
    }

    public void FreeAll()
    {
        foreach (var pool in _poolList)
            pool.Value.FreeAll();
    }

    public void FreeAfter(GameObject obj, float after)
    {
        StartCoroutine(After(obj, after));
    }



    IEnumerator After(GameObject obj, float after)
    {
        yield return new WaitForSeconds(after);
        Free(obj);
    }
}
