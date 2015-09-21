using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public List<GameObject> unitList    = new List<GameObject>();
    public List<GameObject> towerList   = new List<GameObject>();

    PlayerStatus    _status     = null;
    Vector2         _createPos  = Vector2.zero;


    void Start()
    {
        _status = GameManager.instance.enemy;
        _createPos = GameManager.instance.enemyCastlePos;

        for(int i = 0; i < unitList.Count; ++i)
            StartCoroutine("ProduceUnit", unitList[i]);
        for (int i = 0; i < towerList.Count; ++i)
            StartCoroutine("ProduceTower", towerList[i]);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator ProduceUnit(GameObject obj)
    {
        var objInfo = obj.GetComponent<ObjectStatus>();
        var coolTime = objInfo.createTime * 1.5f;
        while(true)
        {
            yield return new WaitForSeconds(coolTime);

            if(_status.Purchase(objInfo.cost))
            {
                var unit = ObjectManager.instance.Assign(obj.name);
                unit.transform.position = _createPos;

                var status = unit.GetComponent<ObjectStatus>();
                status.owner = PlayerStatus.PlayerType.ENEMY;
                status.ChangeDir(ObjectStatus.Direction.LEFT);

                GameManager.instance.enemyObjList.Add(unit);
            }
        }
    }
    IEnumerator ProduceTower(GameObject obj)
    {
        var objInfo = obj.GetComponent<ObjectStatus>();
        var coolTime = 1.0f;
        while (true)
        {
            yield return new WaitForSeconds(coolTime);

            if (_status.Purchase(objInfo.cost))
            {
                var createPos = _createPos + Vector2.left * Random.Range(3.0f, 9.0f);

                var unit = ObjectManager.instance.Assign(obj.name);
                unit.transform.position = createPos;

                var status = unit.GetComponent<ObjectStatus>();
                status.owner = PlayerStatus.PlayerType.ENEMY;
                status.ChangeDir(ObjectStatus.Direction.LEFT);

                GameManager.instance.enemyObjList.Add(unit);
            }
        }
    }
}
