using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public List<GameObject> UnitList    = new List<GameObject>();
    public List<GameObject> TowerList   = new List<GameObject>();

    PlayerStatus    _status     = null;
    Vector2         _createPos  = Vector2.zero;


    void OnDisable()
    {
        StopAllCoroutines();
    }

    void Start()
    {
        _status = GameManager.instance.mEnemy;
        _createPos = GameManager.instance.mEnemyCastlePos;

        for (int i = 0; i < UnitList.Count; ++i)
        {
            StartCoroutine("ProduceUnit", UnitList[i]);
        }
        for (int i = 0; i < TowerList.Count; ++i)
        {
            StartCoroutine(ProduceTower(TowerList[i], (TowerList.Count - i) * 3.0f));
        }
        StartCoroutine("IncomeUpgrade");
    }



    IEnumerator ProduceUnit(GameObject obj)
    {
        var objInfo = obj.GetComponent<ObjectStatus>();
        var coolTime = objInfo.createTime * 1.5f;
        while (true)
        {
            yield return new WaitForSeconds(coolTime);

            if (_status.Purchase(objInfo.cost))
            {
                var unit = ObjectManager.instance.Assign(obj.name);
                unit.transform.position = _createPos;

                var status = unit.GetComponent<ObjectStatus>();
                status.owner = PlayerStatus.PlayerType.ENEMY;
                status.ChangeDir(ObjectStatus.Direction.LEFT);

                GameManager.instance.mEnemyObjList.Add(unit);
            }
        }
    }

    IEnumerator ProduceTower(GameObject obj, float rate)
    {
        var objInfo = obj.GetComponent<ObjectStatus>();
        var coolTime = rate;
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

                GameManager.instance.mEnemyObjList.Add(unit);
            }
        }
    }

    IEnumerator IncomeUpgrade()
    {
        var coolTime = 2.0f;
        while (true)
        {
            yield return new WaitForSeconds(coolTime);

            _status.IncomeUp(gameObject);
            _status.SpeedUp(gameObject);
        }
    }
}
