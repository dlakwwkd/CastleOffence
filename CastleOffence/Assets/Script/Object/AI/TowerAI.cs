using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TowerAI : MonoBehaviour
{
    public enum TowerFSM
    {
        IDLE,
        ATTACK,
        DEAD,
    }

    //-----------------------------------------------------------------------------------
    // inspector field
    public GameObject   MissileObj  = null;
    public TowerFSM     State       = TowerFSM.IDLE;
    public float        StateTime   = 0.0f;

    //-----------------------------------------------------------------------------------
    // handler functions
    void OnEnable()
    {
        StartCoroutine("SearchEnemy");
    }

    void OnDisable()
    {
        StopAllCoroutines();
        State = TowerFSM.IDLE;
        GameManager.instance.playerObjList.Remove(gameObject);
    }

    void Start()
    {
        objInfo = GetComponent<ObjectStatus>();

        dicState[TowerFSM.IDLE]    = Idle;
        dicState[TowerFSM.ATTACK]  = Attack;
        dicState[TowerFSM.DEAD]    = Dead;
    }

    void Update()
    {
        dicState[State]();
    }

    void Idle()
    {
        if (target && !target.GetComponent<ObjectStatus>().IsDead())
        {
            LookEnemy();
            var dist = Vector2.Distance(target.transform.position, transform.position);
            if (dist < objInfo.AttackRange)
            {
                State = TowerFSM.ATTACK;
            }
        }
        else
            target = null;
    }

    void Attack()
    {
        if (target && !target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Vector2.Distance(target.transform.position, transform.position);
            if (dist > objInfo.AttackRange)
            {
                State = TowerFSM.IDLE;
                return;
            }
            StateTime += Time.deltaTime;
            if (StateTime > objInfo.AttackFrontDelay)
            {
                LookEnemy();
                AttackProcess();
                StateTime = -(objInfo.AttackBackDelay + UnityEngine.Random.Range(0.0f, 0.2f));
            }
        }
        else
        {
            State = TowerFSM.IDLE;
            target = null;
        }
    }

    void Dead()
    {
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void Death()
    {
        State = TowerFSM.DEAD;
    }

    //-----------------------------------------------------------------------------------
    // private functions
    void LookEnemy()
    {
        if (objInfo.Type == ObjectStatus.ObjectType.CASTLE)
            return;

        var dir = ObjectStatus.Direction.RIGHT;
        var displacement = target.transform.position.x - transform.position.x;
        if (displacement < 0)
            dir = ObjectStatus.Direction.LEFT;

        objInfo.ChangeDir(dir);
    }

    void AttackProcess()
    {
        if (objInfo.AttackSounds.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, objInfo.AttackSounds.Count);
            AudioManager.instance.PlaySfx(objInfo.AttackSounds[rand], 1.5f);
        }
        var pivotGap = 2.0f;
        if (objInfo.Type == ObjectStatus.ObjectType.CASTLE)
            pivotGap += 3.0f;

        var missile = ObjectManager.instance.Assign(MissileObj.name);
        missile.transform.localPosition = transform.localPosition + Vector3.up * pivotGap;

        var info = missile.GetComponent<ObjectStatus>();
        info.Owner = objInfo.Owner;
        info.Damage = objInfo.Damage;

        var forceRatio = 25.0f;
        var forceGap = UnityEngine.Random.Range(450.0f, 550.0f) - pivotGap * forceRatio;
        var displacement = target.transform.localPosition - transform.localPosition;

        var fireForce = new Vector2(displacement.x * forceRatio, displacement.y * forceRatio + forceGap);
        var body = missile.GetComponent<Rigidbody2D>();
        body.AddForce(fireForce);

        //         var dirVector = fireForce.normalized;
        //         var angle = Vector2.Angle(Vector2.right, dirVector);
        //         if (dirVector.y < 0)
        //             angle = -angle;
        //         missile.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator SearchEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            var enemyList = GameManager.instance.enemyObjList;
            if (objInfo.Owner == PlayerStatus.PlayerType.ENEMY)
                enemyList = GameManager.instance.playerObjList;

            var unitPos = transform.localPosition.x;
            var closeEnemyDist = float.MaxValue;

            for (int i = 0; i < enemyList.Count; ++i)
            {
                var dist = Math.Abs(enemyList[i].transform.localPosition.x - unitPos);
                if (dist < closeEnemyDist)
                {
                    closeEnemyDist = dist;
                    target = enemyList[i];
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------
    // private field
    Dictionary<TowerFSM, Action>    dicState    = new Dictionary<TowerFSM, Action>();
    ObjectStatus                    objInfo     = null;
    GameObject                      target      = null;
}
