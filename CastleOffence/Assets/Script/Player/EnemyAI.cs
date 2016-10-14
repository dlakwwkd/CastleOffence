using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // inspector field
    [FormerlySerializedAs("unitList")]
    public List<GameObject> UnitList    = new List<GameObject>();
    [FormerlySerializedAs("towerList")]
    public List<GameObject> TowerList   = new List<GameObject>();

    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        player = GameManager.instance.enemy;
        createPos = GameManager.instance.enemyCastlePos;

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

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator ProduceUnit(GameObject obj)
    {
        var objInfo = obj.GetComponent<ObjectStatus>();
        var coolTime = objInfo.CreateTime * 1.5f;
        while (true)
        {
            yield return new WaitForSeconds(coolTime);

            if (player.Purchase(objInfo.Cost))
            {
                var unit = ObjectManager.instance.Assign(obj.name);
                unit.transform.position = createPos;

                var status = unit.GetComponent<ObjectStatus>();
                status.Owner = PlayerStatus.PlayerType.ENEMY;
                status.ChangeDir(ObjectStatus.Direction.LEFT);

                GameManager.instance.enemyObjList.Add(unit);
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

            if (player.Purchase(objInfo.Cost))
            {
                var createPos = this.createPos + Vector2.left * Random.Range(3.0f, 9.0f);

                var unit = ObjectManager.instance.Assign(obj.name);
                unit.transform.position = createPos;

                var status = unit.GetComponent<ObjectStatus>();
                status.Owner = PlayerStatus.PlayerType.ENEMY;
                status.ChangeDir(ObjectStatus.Direction.LEFT);

                GameManager.instance.enemyObjList.Add(unit);
            }
        }
    }

    IEnumerator IncomeUpgrade()
    {
        var coolTime = 2.0f;
        while (true)
        {
            yield return new WaitForSeconds(coolTime);

            player.IncomeUp(gameObject);
            player.SpeedUp(gameObject);
        }
    }

    //-----------------------------------------------------------------------------------
    // private field
    PlayerStatus    player      = null;
    Vector2         createPos   = Vector2.zero;
}
