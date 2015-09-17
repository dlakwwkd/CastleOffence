using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();

    Vector2 _createPos = Vector2.zero;


    void Start()
    {
        _createPos = GameManager.instance.enemyCastlePos;

        for(int i = 0; i < unitList.Count; ++i)
        {
            StartCoroutine("ProduceUnit", unitList[i]);
        }
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ProduceUnit(GameObject obj)
    {
        float coolTime = obj.GetComponent<ObjectStatus>().createTime;
        while(true)
        {
            yield return new WaitForSeconds(coolTime * 1.5f);

            var unit = ObjectManager.instance.Assign(obj.name);
            unit.transform.position = _createPos;

            for (int i = 0; i < unit.transform.childCount; ++i)
            {
                var child = unit.transform.GetChild(i);
                child.localRotation = Quaternion.identity;
            }

            var status = unit.GetComponent<ObjectStatus>();
            status.owner = PlayerType.ENEMY;
            status.dir = ObjectStatus.Direction.LEFT;
            unit.transform.localRotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
            unit.transform.FindChild("HpBar").localRotation = unit.transform.localRotation;

            GameManager.instance.enemyObjList.Add(unit);
        }
    }
}
