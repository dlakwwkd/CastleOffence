using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    static public ObjectManager _instance = null;
    public static ObjectManager instance { get { return _instance; } }

    Dictionary<string, ObjectPool> _poolList = new Dictionary<string, ObjectPool>();

    public List<GameObject> ObjectList = new List<GameObject>();

    void Start()
    {
        if (_instance == null)
            _instance = this;

        foreach (var obj in ObjectList)
        {
            var pool = gameObject.AddComponent<ObjectPool>();
            pool.Init(obj);

            _poolList.Add(obj.name, pool);
        }
    }

    public GameObject Assign(string objName)
    {
        return _poolList[objName].Assign();
    }

    public void Free(GameObject obj)
    {
        int idx = obj.name.IndexOf('_');
        var key = obj.name.Remove(idx);
        _poolList[key].Free(obj);
    }
}

public class ObjectPool : MonoBehaviour
{
    GameObject          _objectType     = null;
    List<GameObject>    _objectList     = new List<GameObject>();
    Queue<int>          _freeList       = new Queue<int>();
    int                 _allocInterval  = 5;

    public void Init(GameObject obj)
    {
        _objectType = obj;
        Alloc();
    }

    public GameObject Assign()
    {
        if (_freeList.Count < 1)
            Alloc();

        int idx = _freeList.Dequeue();
        var obj = _objectList[idx];
        obj.SetActive(true);
        return obj;
    }

    public void Free(GameObject obj)
    {
        obj.SetActive(false);

        int idx = obj.name.IndexOf('_');
        var key = obj.name.Substring(idx + 1);
        _freeList.Enqueue(int.Parse(key));
    }

    void Alloc()
    {
        for (int i = 0; i < _allocInterval; ++i)
        {
            var obj = Instantiate(_objectType) as GameObject;
            obj.name = _objectType.name + "_" + _objectList.Count;
            obj.SetActive(false);

            _freeList.Enqueue(_objectList.Count);
            _objectList.Add(obj);
        }
    }
}